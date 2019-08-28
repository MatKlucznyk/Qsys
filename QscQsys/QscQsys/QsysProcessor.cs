using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
using Crestron.SimplSharp.CrestronIO;
using ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCP_Client;

namespace QscQsys
{
    /// <summary>
    /// Static processor for Q-Sys Cores.
    /// </summary>
    public static class QsysProcessor
    {
        private static CrestronQueue<string> commandQueue;
        private static CrestronQueue<string> responseQueue;
        private static CTimer commandQueueTimer;
        private static CTimer responseQueueTimer;
        private static CTimer heartbeatTimer;
        private static TCPClientDevice client;

        private static bool isInitialized;
        private static bool isDisposed;
        private static bool debug;

        //internal static Dictionary<string, InternalEvents> Controls = new Dictionary<string, InternalEvents>();
        internal static Dictionary<Component, InternalEvents> Components = new Dictionary<Component, InternalEvents>();
        internal static Dictionary<string, SimplEvents> SimplClients = new Dictionary<string, SimplEvents>();

        /// <summary>
        /// Processor initialzation state.
        /// </summary>
        public static bool IsInitialized { get { return isInitialized; } }

        /// <summary>
        /// Processor disposed state.
        /// </summary>
        public static bool IsDisposed { get { return isDisposed; } }

        public static bool IsConnected { get; set; }

        public static bool IsDebugMode { get { return debug; } }

        static internal bool RegisterComponent(Component component)
        {
            try
            {
                lock (Components)
                {
                    if (!Components.ContainsKey(component))
                    {
                        Components.Add(component, new InternalEvents());

                        if (isInitialized && IsConnected)
                        {
                            AddControlToChangeGroup addControl;

                            addControl = new AddControlToChangeGroup();
                            addControl.method = "ChangeGroup.AddComponentControl";
                            addControl.ComponentParams = new AddComponentToChangeGroupParams();
                            addControl.ComponentParams.Component = component;
                            commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error registering QsysClient to the QsysProcessor: {0}", e.Message);
                return false;
            }
        }

        public static bool RegisterSimplClient(string id)
        {
            try
            {
                lock (SimplClients)
                {
                    if(!SimplClients.ContainsKey(id))
                    {
                        SimplClients.Add(id, new SimplEvents());
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Initialzes all methods that are required to setup the class. Connection is established on port 1702.
        /// </summary>
        public static void Initialize(string host, ushort port)
        {
            if (!isInitialized)
            {
                ErrorLog.Notice("QsysProcessor is initializing.");
                commandQueue = new CrestronQueue<string>();
                responseQueue = new CrestronQueue<string>();
                commandQueueTimer = new CTimer(CommandQueueDequeue, null, 0, 50);
                responseQueueTimer = new CTimer(ResponseQueueDequeue, null, 0, 50);

                client = new TCPClientDevice();
                client.ID = 1;
                client.ConnectionStatus += new StatusEventHandler(client_ConnectionStatus);
                client.ResponseString += new ResponseEventHandler(client_ResponseString);
                client.Connect(host, port);
            }
        }

        public static void Debug(ushort value)
        {
            debug = Convert.ToBoolean(value);
        }

        static void client_ResponseString(string response, int id)
        {
            ParseResponse(response);
        }

        static void client_ConnectionStatus(int status, int id)
        {
            if (status == 2)
            {
                ErrorLog.Notice("QsysProcessor is connected.");
                IsConnected = true;
                foreach (var item in SimplClients)
                {
                    item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsConnected, "true", 1));
                }

                CrestronEnvironment.Sleep(1500);

                commandQueue.Enqueue(JsonConvert.SerializeObject(new GetComponents()));

                AddControlToChangeGroup addControl;


                foreach (var item in Components)
                {
                    addControl = new AddControlToChangeGroup();
                    addControl.method = "ChangeGroup.AddComponentControl";
                    addControl.ComponentParams = new AddComponentToChangeGroupParams();
                    addControl.ComponentParams.Component = item.Key;
                    commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
                }

                commandQueue.Enqueue(JsonConvert.SerializeObject(new CreateChangeGroup()));

                heartbeatTimer = new CTimer(SendHeartbeat, null, 0, 15000);

                ErrorLog.Notice("QsysProcessor is initialized.");
                isInitialized = true;

                foreach (var item in SimplClients)
                {
                    item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRegistered, "true", 1));
                }
            }
            else
            {
                ErrorLog.Error("QsysProcessor disconnected!");
                IsConnected = false;
                isInitialized = false;
                heartbeatTimer.Dispose();
                foreach (var item in SimplClients)
                {
                    item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRegistered, "false", 0));
                    item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsConnected, "false", 0));
                }
            }
        }

        private static void SendHeartbeat(object o)
        {
            commandQueue.Enqueue(JsonConvert.SerializeObject(new Heartbeat()));
        }

        /// <summary>
        /// Cleans up all resources.
        /// </summary>
        public static void Dispose()
        {
            if (IsInitialized)
            {
                client.Disconnect();
                commandQueue.Dispose();
                commandQueueTimer.Stop();
                commandQueueTimer.Dispose();

                if (!heartbeatTimer.Disposed)
                {
                    heartbeatTimer.Stop();
                    heartbeatTimer.Dispose();
                }
                
                isDisposed = true;
            }
        }

        private static void CommandQueueDequeue(object o)
        {
            if (!commandQueue.IsEmpty)
            {
                var data = commandQueue.Dequeue();

                client.SendCommand(data + "\x00");
            }
        }

        static StringBuilder RxData = new StringBuilder();
        static bool busy = false;
        static int Pos = -1;
        private static void ResponseQueueDequeue(object o)
        {
            if (!responseQueue.IsEmpty)
            {
                try
                {
                    // removes string from queue, blocks until an item is queued
                    string tmpString = responseQueue.Dequeue();

                    RxData.Append(tmpString); //Append received data to the COM buffer

                    if (!busy)
                    {
                        busy = true;
                        while (RxData.ToString().Contains("\x00"))
                        {
                            Pos = RxData.ToString().IndexOf("\x00");
                            var data = RxData.ToString().Substring(0, Pos);
                            var garbage = RxData.Remove(0, Pos + 1); // remove data from COM buffer

                            ParseInternalResponse(data);
                        }

                        busy = false;
                    }
                }
                catch (Exception e)
                {
                    busy = false;
                    ErrorLog.Exception(e.Message, e);
                }

                //ParseInternalResponse(responseQueue.Dequeue());
            }
        }

        private static void ParseInternalResponse(string returnString)
        {
            if (returnString.Length > 0)
            {
                try
                {
                    JObject response = JObject.Parse(returnString);

                    if (returnString.Contains("Changes"))
                    {
                        IList<JToken> changes = response["params"]["Changes"].Children().ToList();

                        IList<ChangeResult> changeResults = new List<ChangeResult>();

                        foreach (JToken change in changes)
                        {
                            //ChangeResult changeResult = (ChangeResult)change.Cast<ChangeResult>();

                            ChangeResult changeResult = JsonConvert.DeserializeObject<ChangeResult>(change.ToString());

                            if (changeResult.Component != null)
                            {
                                foreach (var item in Components)
                                {
                                    if (item.Key.Name == changeResult.Component)
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.String));
                                }
                            }
                            /*else
                            {
                                foreach (var item in Controls)
                                {
                                    if (item.Key == changeResult.Name)
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value));
                                }
                            }*/
                        }
                    }
                    else if (returnString.Contains("EngineStatus"))
                    {
                    }
                    else if (returnString.Contains("Properties"))
                    {
                        IList<JToken> components = response["result"].Children().ToList();

                        IList<ComponentResults> componentResults = new List<ComponentResults>();

                        foreach (var component in components)
                        {
                            ComponentResults result = JsonConvert.DeserializeObject<ComponentResults>(component.ToString());

                            if (result.Type == "gain")
                            {
                                foreach (var item in Components)
                                {
                                    if (item.Key.Name == result.Name)
                                    {
                                        List<ComponentProperties> props = result.Properties.ToList();
                                        ComponentProperties prop;
                                        if ((prop = props.Find(x => x.Name == "max_gain")) != null)
                                        {
                                            item.Value.Fire(new QsysInternalEventsArgs("max_gain", Convert.ToDouble(prop.Value), string.Empty));
                                        }
                                        if ((prop = props.Find(x => x.Name == "min_gain")) != null)
                                        {
                                            item.Value.Fire(new QsysInternalEventsArgs("min_gain", Convert.ToDouble(prop.Value), string.Empty));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //ErrorLog.Error("Error is QsysProcessor: {0}:\r\n{1}", e.Message, returnString);
                }
            }
        }

        internal static void Enqueue(string data)
        {
            commandQueue.Enqueue(data);
        }

        //private static MemoryStream _memStream = new MemoryStream();
        /// <summary>
        /// Parse response from Q-Sys Core.
        /// </summary>
        /// <param name="data"></param>
        public static void ParseResponse(string data)
        {
            //gather.Gather(data);
            try
            {
                //CrestronConsole.PrintLine(data);
                responseQueue.Enqueue(data);
            }
            catch (Exception e)
            {
            }
        }
    }
}


 
