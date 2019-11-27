using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;                          				// For Basic SIMPL# Classes
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
        private static bool isRedundant;
        private static bool isEmulator;

        private static string designName;

        //internal static Dictionary<string, InternalEvents> Controls = new Dictionary<string, InternalEvents>();
        internal static Dictionary<Component, InternalEvents> Components = new Dictionary<Component, InternalEvents>();
        internal static Dictionary<Control, InternalEvents> Controls = new Dictionary<Control, InternalEvents>();
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
                            AddComoponentToChangeGroup addControl;

                            addControl = new AddComoponentToChangeGroup();
                            addControl.method = "ChangeGroup.AddComponentControl";
                            addControl.ComponentParams = new AddComponentToChangeGroupParams();
                            addControl.ComponentParams.Component = component;
                            commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));

                            if(debug)
                                CrestronConsole.PrintLine("Registered {0} Component", component.Name);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if(debug)
                    ErrorLog.Error("Error registering QsysClient to the QsysProcessor: {0}", e.Message);
                return false;
            }
        }

        static internal bool RegisterControl(Control control)
        {
            try
            {
                lock (Controls)
                {
                    if (!Controls.ContainsKey(control))
                    {
                        Controls.Add(control, new InternalEvents());

                        if (isInitialized && IsConnected)
                        {
                            AddControlToChangeGroup addControl;

                            addControl = new AddControlToChangeGroup();
                            addControl.method = "ChangeGroup.AddControl";
                            addControl.ControlParams = new AddControlToChangeGroupParams();
                            addControl.ControlParams.Controls = new List<string>();
                            addControl.ControlParams.Controls.Add(control.Name);
                            commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));

                            if (debug)
                                CrestronConsole.PrintLine("Registered {0} Control", control.Name);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if(debug)
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

                        if (debug)
                            CrestronConsole.PrintLine("Registered {0} SimplClient", id);
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                if (debug)
                    ErrorLog.Error("Error registering SimplClient to the QsysProcessor: {0}", e.Message);
                return false;
            }
        }

        public static bool IsRedundant { get { return isRedundant; } }

        public static bool IsEmulator { get { return isEmulator; } }

        public static string DesignName { get { return designName; } }
        
        /// <summary>
        /// Initialzes all methods that are required to setup the class. Connection is established on port 1702.
        /// </summary>
        public static void Initialize(string host, ushort port)
        {
            if (!isInitialized)
            {
                if(debug)
                    ErrorLog.Notice("QsysProcessor is initializing.");
                commandQueue = new CrestronQueue<string>();
                responseQueue = new CrestronQueue<string>();
                commandQueueTimer = new CTimer(CommandQueueDequeue, null, 0, 50);
                responseQueueTimer = new CTimer(ResponseQueueDequeue, null, 0, 50);

                client = new TCPClientDevice();
                client.ID = 1;
                client.RepeatConnectionAttempTime = 15000;
                client.ConnectionStatus += new StatusEventHandler(client_ConnectionStatus);
                client.ResponseString += new ResponseEventHandler(client_ResponseString);
                client.Connect(host, port);
            }
        }

        public static void Debug(ushort value)
        {
            debug = Convert.ToBoolean(value);

            if (debug)
                CrestronConsole.PrintLine("********Qsys Debug Mode Active********");
            else
                CrestronConsole.PrintLine("********Qsys Debug Mode Disabled********");
        }

        static void client_ResponseString(string response, int id)
        {
            ParseResponse(response);
        }

        static void client_ConnectionStatus(int status, int id)
        {
            if (status == 2 && !IsConnected)
            {
                if(debug)
                    ErrorLog.Notice("QsysProcessor is connected.");
                IsConnected = true;
                foreach (var item in SimplClients)
                {
                    item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsConnected, "true", 1));
                }

                CrestronEnvironment.Sleep(1500);

                //commandQueue.Enqueue(JsonConvert.SerializeObject(new GetComponents()));

                AddComoponentToChangeGroup addComponent;


                foreach (var item in Components)
                {
                    addComponent = new AddComoponentToChangeGroup();
                    addComponent.method = "ChangeGroup.AddComponentControl";
                    addComponent.ComponentParams = new AddComponentToChangeGroupParams();
                    addComponent.ComponentParams.Component = item.Key;
                    commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));
                }

                AddControlToChangeGroup addControl;

                foreach (var item in Controls)
                {
                    addControl = new AddControlToChangeGroup();
                    addControl.method = "ChangeGroup.AddControl";
                    addControl.ControlParams = new AddControlToChangeGroupParams();
                    addControl.ControlParams.Controls = new List<string>();
                    addControl.ControlParams.Controls.Add(item.Key.Name);
                    commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
                }

                commandQueue.Enqueue(JsonConvert.SerializeObject(new CreateChangeGroup()));

                heartbeatTimer = new CTimer(SendHeartbeat, null, 0, 15000);

                if(debug)
                    ErrorLog.Notice("QsysProcessor is initialized.");
                isInitialized = true;

                foreach (var item in SimplClients)
                {
                    item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRegistered, "true", 1));
                }
            }
            else if(IsConnected && status != 2)
            {
                if(debug)
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

                            if (debug)
                                CrestronConsole.PrintLine("Response found ** {0} **", data);

                            ParseInternalResponse(data);
                        }

                        busy = false;
                    }
                }
                catch (Exception e)
                {
                    busy = false;
                    if (debug)
                        ErrorLog.Error("Error in QsysProcessor ResponseQueueDequeue: {0}", e.Message);
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

                            ChangeResult changeResult = JsonConvert.DeserializeObject<ChangeResult>(change.ToString(), new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

                            if (changeResult.Component != null)
                            {
                                foreach (var item in Components)
                                {
                                    List<string> choices;

                                    if (changeResult.Choices != null)
                                        choices = changeResult.Choices.ToList();
                                    else
                                        choices = new List<string>();

                                    if (item.Key.Name == changeResult.Component)
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.Position, changeResult.String, choices));
                                }
                            }
                            else if(changeResult.Name != null)
                            {
                                List<string> choices;

                                if (changeResult.Choices != null)
                                    choices = changeResult.Choices.ToList();
                                else
                                    choices = new List<string>();

                                foreach (var item in Controls)
                                {
                                    if (item.Key.Name.Contains(changeResult.Name))
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.Position, changeResult.String, choices));
                                }
                            }
                        }
                    }
                    else if (returnString.Contains("EngineStatus"))
                    {
                        if (response["params"] != null)
                        {
                            JToken engineStatus = response["params"];

                            if (engineStatus["DesignName"] != null)
                            {
                                designName = engineStatus["DesignName"].ToString();
                                
                                
                            }

                            if (engineStatus["IsRedundant"] != null)
                            {
                                isRedundant = Convert.ToBoolean(engineStatus["IsRedundant"].ToString());
                            }

                            if (engineStatus["IsEmulator"] != null)
                            {
                                isEmulator = Convert.ToBoolean(engineStatus["IsEmulator"].ToString());
                            }

                            foreach (var item in SimplClients)
                            {
                                item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.NewCoreStatus, string.Empty, 1));
                            }
                        }
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
                                            //item.Value.Fire(new QsysInternalEventsArgs("max_gain", Convert.ToDouble(prop.Value), 0, string.Empty));
                                        }
                                        if ((prop = props.Find(x => x.Name == "min_gain")) != null)
                                        {
                                            //item.Value.Fire(new QsysInternalEventsArgs("min_gain", Convert.ToDouble(prop.Value), 0, string.Empty));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    if(debug)
                        ErrorLog.Error("Error in QsysProcessor ParseInternalResponse: {0}:\r\n{1}", e.Message, returnString);
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

        internal static double ScaleUp(double level)
        {
            double scaleLevel = level;
            double levelScaled = (scaleLevel * 65535.0);
            return levelScaled;
        }

        internal static double ScaleDown(double level)
        {
            double scaleLevel = level;
            double levelScaled = (scaleLevel / 65535.0);
            return levelScaled;
        }
    }
}


 
