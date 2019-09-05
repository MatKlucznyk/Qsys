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
        private static bool debug;

        private static bool isInitialized;
        private static bool isDisposed;

        private static bool loggedIn;
        private static string loginUser;
        private static string loginPass;
        private static eCoreState coreState;
        private static string platform;
        private static string designName;
        private static string designCode;
        private static bool isRedundant;
        private static bool isEmulator;
        private static int statusCode;
        private static string statusString;

        internal static Dictionary<string, InternalEvents> Controls = new Dictionary<string, InternalEvents>();
        internal static Dictionary<Component, InternalEvents> Components = new Dictionary<Component, InternalEvents>();
        internal static Dictionary<string, SimplEvents> SimplClients = new Dictionary<string, SimplEvents>();

        /// <summary>
        /// Processor Status
        /// </summary>
        public static bool IsInitialized { get { return isInitialized; } }
        public static bool IsConnected { get; set; }
        public static eCoreState CoreState { get { return coreState; } }
        public static string Platform { get { return platform; } }
        public static string DesignName { get { return designName; } }
        public static string DesignCode { get { return designCode; } }
        public static bool IsRedundant { get { return isRedundant; } }
        public static bool IsEmulator { get { return isEmulator; } }
        public static int StatusCode { get { return statusCode; } }
        public static string StatusString { get { return statusString; } }

        /// <summary>
        /// Processor disposed state.
        /// </summary>
        public static bool IsDisposed { get { return isDisposed; } }

        /// <summary>
        /// Debug Mode
        /// </summary>
        public static bool IsDebugMode { get { return debug; } }

        static internal bool RegisterControl(string control)
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
                            addControl.ControlParams.Controls.Add(control);
                            commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
                            
                            if (debug)
                                CrestronConsole.PrintLine("Adding named control: {0} to change group", control);
                        }
                        else
                        {
                            CrestronConsole.PrintLine("reg: {0}, con: {1}", isInitialized, IsConnected);
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
                            AddComponentToChangeGroup addComponent;

                            addComponent = new AddComponentToChangeGroup();
                            addComponent.method = "ChangeGroup.AddComponentControl";
                            addComponent.ComponentParams = new AddComponentToChangeGroupParams();
                            addComponent.ComponentParams.Component = component;
                            commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));
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
        public static void Initialize(string host, ushort port, string user, string pass)
        {
            if (!isInitialized)
            {
                ErrorLog.Notice("QsysProcessor is initializing.");
                commandQueue = new CrestronQueue<string>();
                responseQueue = new CrestronQueue<string>();
                commandQueueTimer = new CTimer(CommandQueueDequeue, null, 0, 50);
                responseQueueTimer = new CTimer(ResponseQueueDequeue, null, 0, 50);

                loginUser = user;
                loginPass = pass;

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
            if (debug)
                //CrestronConsole.PrintLine("RX ID:{0} - {1}", id, response);
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

                //Send login if needed
                if (loginUser.Length > 0 || loginPass.Length > 0)
                {
                    SendLogin();
                }

                CoreModuleInit();
                
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

        private static void SendLogin()
        {
            SendDebug(string.Format("Qsys - Sending login: {0}:{1}", loginUser, loginPass));
            CoreLogon logon = new CoreLogon();
            logon.Params = new CoreLogonParams();
            logon.Params.User = loginUser;
            logon.Params.Password = loginPass;
            commandQueue.Enqueue(JsonConvert.SerializeObject(logon));
        }

        private static void CoreModuleInit()
        {
            commandQueue.Enqueue(JsonConvert.SerializeObject(new GetComponents()));

            if (Controls.Count() > 0)
            {
                AddControlToChangeGroup addControls;
                addControls = new AddControlToChangeGroup();
                addControls.method = "ChangeGroup.AddControl";
                addControls.ControlParams = new AddControlToChangeGroupParams();
                addControls.ControlParams.Controls = new List<string>();
                foreach (var item in Controls)
                {
                    addControls.ControlParams.Controls.Add(item.Key);
                    if (debug)
                        SendDebug(string.Format("Adding named control: {0} to change group", item.Key));
                }
                commandQueue.Enqueue(JsonConvert.SerializeObject(addControls));
            }

            AddComponentToChangeGroup addComponents;
            foreach (var item in Components)
            {
                addComponents = new AddComponentToChangeGroup();
                addComponents.method = "ChangeGroup.AddComponentControl";
                addComponents.ComponentParams = new AddComponentToChangeGroupParams();
                addComponents.ComponentParams.Component = item.Key;
                commandQueue.Enqueue(JsonConvert.SerializeObject(addComponents));
            }

            commandQueue.Enqueue(JsonConvert.SerializeObject(new CreateChangeGroup()));
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

                SendDebug(string.Format("Processor - Sending to core from queue: {0}", data));
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

                        IList<ComponentChangeResult> changeResults = new List<ComponentChangeResult>();

                        foreach (JToken change in changes)
                        {
                            //ChangeResult changeResult = (ChangeResult)change.Cast<ChangeResult>();

                            ComponentChangeResult changeResult = JsonConvert.DeserializeObject<ComponentChangeResult>(change.ToString());

                            if (changeResult.Component != null)
                            {
                                foreach (var item in Components)
                                {
                                    if (item.Key.Name == changeResult.Component)
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.String));
                                }
                            }
                            else
                            {
                                foreach (var item in Controls)
                                {
                                    if (item.Key == changeResult.Name)
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.String));
                                }
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
                    else if (returnString.Contains("EngineStatus") || returnString.Contains("StatusGet"))
                    {
                        EngineStatusResult statusResult = JsonConvert.DeserializeObject<EngineStatusResult>(returnString);
                        coreState = (eCoreState)Enum.Parse(typeof(eCoreState), statusResult.Properties.State, true);
                        platform = statusResult.Properties.Platform;
                        designName = statusResult.Properties.DesignName;
                        designCode = statusResult.Properties.DesignCode;
                        isRedundant = Convert.ToBoolean(statusResult.Properties.IsRedundant);
                        isEmulator = Convert.ToBoolean(statusResult.Properties.IsEmulator);
                        statusCode = statusResult.Properties.Status.Code;
                        statusString = statusResult.Properties.Status.String;
                        foreach (var item in SimplClients)
                        {
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.CoreState, "", (ushort)coreState));
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.Platform, "", (ushort)coreState));     
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.DesignName, designName, 0));
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.DesignCode, designCode, 0));
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRedundant, Convert.ToString(isRedundant), (ushort)Convert.ToInt16(isRedundant)));
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsEmulator, Convert.ToString(isEmulator), (ushort)Convert.ToInt16(isEmulator)));
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.StatusCode, Convert.ToString(statusCode), (ushort)statusCode));
                            item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.StatusString, statusString, 0));
                        }
                    }
                    else if (returnString.Contains("error"))
                    {
                        CoreError err = JsonConvert.DeserializeObject<CoreError>(returnString);
                        CrestronConsole.PrintLine("QsysProcessor parse error message - {0}-{1}", err.error.code, err.error.message);
                    }
                }
                catch (Exception e)
                {
                    SendDebug(String.Format("Error is QsysProcessor: {0}:\r\n{1}", e.Message, returnString));
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
                SendDebug(string.Format("Processor - Received from core and adding to queue: {0}", data));
                responseQueue.Enqueue(data);
            }
            catch (Exception e)
            {
            }
        }

        public static void SendDebug(string msg)
        {
            if (debug)
                CrestronConsole.PrintLine("Qsys Debug: {0}", msg);
            //ErrorLog.Error("Error is QsysProcessor: {0}:\r\n{1}", e.Message, returnString);
        }


    }
    public enum eCoreState
    {
        Idle = 0,
        Active = 1,
        Standby = 2
    }
}


 
