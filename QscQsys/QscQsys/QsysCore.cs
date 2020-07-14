using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;             				// For Basic SIMPL# Classes
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCP_Client;

namespace QscQsys
{
    /// <summary>
    /// processor for Q-Sys Cores.
    /// </summary>
    public class QsysCore
    {
        public delegate void IsRegistered(ushort value);
        public delegate void IsConnectedStatus(ushort value);
        public delegate void CoreStatus(SimplSharpString designName, ushort isRedundant, ushort isEmulator);
        public IsRegistered onIsRegistered { get; set; }
        public IsConnectedStatus onIsConnected { get; set; }
        public CoreStatus onNewCoreStatus { get; set; }

        private CrestronQueue<string> commandQueue;
        private CrestronQueue<string> responseQueue;
        private CTimer commandQueueTimer;
        private CTimer responseQueueTimer;
        private CTimer heartbeatTimer;
        private TCPClientDevice client;

        private bool isInitialized;
        private bool isDisposed;
        private ushort debug;
        private bool isRedundant;
        private bool isEmulator;

        private string designName;
        private string coreId;

        internal Dictionary<Component, InternalEvents> Components = new Dictionary<Component, InternalEvents>();
        internal Dictionary<Control, InternalEvents> Controls = new Dictionary<Control, InternalEvents>();
        //internal Dictionary<string, SimplEvents> SimplClients = new Dictionary<string, SimplEvents>();

        /// <summary>
        /// Processor initialzation state.
        /// </summary>
        public bool IsInitialized { get { return isInitialized; } }

        /// <summary>
        /// Processor disposed state.
        /// </summary>
        public bool IsDisposed { get { return isDisposed; } }

        public bool IsConnected { get; set; }

        public ushort IsDebugMode { get { return debug; } }

        internal bool RegisterComponent(Component component)
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

                            if (debug == 2)
                                CrestronConsole.PrintLine("Registered {0} Component", component.Name);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if (debug == 1 || debug == 2)
                    ErrorLog.Error("Error registering QsysClient to the QsysProcessor: {0}", e.Message);
                return false;
            }
        }

        internal bool RegisterControl(Control control)
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

                            if (debug == 2)
                                CrestronConsole.PrintLine("Registered {0} Control", control.Name);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if (debug == 1 || debug == 2)
                    ErrorLog.Error("Error registering QsysClient to the QsysProcessor: {0}", e.Message);
                return false;
            }
        }

        /*public bool RegisterSimplClient(string id)
        {
            try
            {
                lock (SimplClients)
                {
                    if (!SimplClients.ContainsKey(id))
                    {
                        SimplClients.Add(id, new SimplEvents());

                        if (debug == 2)
                            CrestronConsole.PrintLine("Registered {0} SimplClient", id);
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                if (debug == 1 || debug == 2)
                    ErrorLog.Error("Error registering SimplClient to the QsysProcessor: {0}", e.Message);
                return false;
            }
        }*/

        public bool IsRedundant { get { return isRedundant; } }

        public bool IsEmulator { get { return isEmulator; } }

        public string DesignName { get { return designName; } }

        public string CoreId { get { return coreId; } }

        /// <summary>
        /// Initialzes all methods that are required to setup the class. Connection is established on port 1702.
        /// </summary>
        public void Initialize(string id, string host, ushort port)
        {
            if (!isInitialized)
            {
                try
                {
                    coreId = id;

                    QsysCoreManager.AddCore(this);

                    if (debug == 1)
                        ErrorLog.Notice("QsysProcessor is initializing.");

                    commandQueue = new CrestronQueue<string>();
                    responseQueue = new CrestronQueue<string>();
                    commandQueueTimer = new CTimer(CommandQueueDequeue, null, 0, 50);
                    responseQueueTimer = new CTimer(ResponseQueueDequeue, null, 0, 50);

                    client = new TCPClientDevice();
                    client.ID = 1710;
                    client.ConnectionStatus += new StatusEventHandler(client_ConnectionStatus);
                    client.ResponseString += new ResponseEventHandler(client_ResponseString);
                    client.Connect(host, port);
                }
                catch (Exception e)
                {
                }
            }
        }

        public void Debug(ushort value)
        {
            debug = value;

            if (debug > 0)
            {
                CrestronConsole.PrintLine("********Qsys Debug Mode Active********");
                CrestronConsole.PrintLine("See log for details");
                ErrorLog.Notice("********Qsys Debug Mode Active********");
                if (debug == 1)
                {
                    ErrorLog.Notice("Qsys debug level: Main communications");
                }
                else if (debug == 2)
                {
                    ErrorLog.Notice("Qsys debug level: Main communications and verbose console");
                }
                ErrorLog.Notice("Qsys TCP ID 1710");
            }
        }

        void client_ResponseString(string response, int id)
        {
            ParseResponse(response);
        }

        void client_ConnectionStatus(int status, int id)
        {
            try
            {
                if (status == 2 && !IsConnected)
                {
                    if (debug == 1 || debug == 2)
                        ErrorLog.Notice("QsysProcessor is connected.");
                    IsConnected = true;

                    if (onIsConnected != null)
                        onIsConnected(1);

                    /*foreach (var item in SimplClients)
                    {
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsConnected, "true", 1));
                    }*/

                    //CrestronEnvironment.Sleep(1500);

                    CTimer waitForServerToStartTimer = new CTimer(WaitForServerToStart, 1500);

                    //commandQueue.Enqueue(JsonConvert.SerializeObject(new GetComponents()));

                    

                    /*foreach (var item in SimplClients)
                    {
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRegistered, "true", 1));
                    }*/
                }
                else if (IsConnected && status != 2)
                {
                    if (debug == 1 || debug == 2)
                        ErrorLog.Error("QsysProcessor disconnected!");
                    IsConnected = false;
                    isInitialized = false;
                    heartbeatTimer.Dispose();

                    if (onIsRegistered != null)
                        onIsRegistered(0);

                    if (onIsConnected != null)
                        onIsConnected(0);

                    /*foreach (var item in SimplClients)
                    {
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRegistered, "false", 0));
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsConnected, "false", 0));
                    }*/
                }
            }
            catch (Exception e)
            {
            }
        }

        private void WaitForServerToStart(object o)
        {
            AddComoponentToChangeGroup addComponent;


            foreach (var item in Components)
            {
                addComponent = new AddComoponentToChangeGroup() { method = "ChangeGroup.AddComponentControl", ComponentParams = new AddComponentToChangeGroupParams() { Component = item.Key } };
                commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));
            }

            AddControlToChangeGroup addControl;

            foreach (var item in Controls)
            {
                addControl = new AddControlToChangeGroup() { method = "ChangeGroup.AddControl", ControlParams = new AddControlToChangeGroupParams() { Controls = new List<string>() { item.Key.Name } } };
                commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
            }

            commandQueue.Enqueue(JsonConvert.SerializeObject(new CreateChangeGroup()));

            if (heartbeatTimer != null)
            {
                heartbeatTimer.Stop();
                heartbeatTimer.Dispose();
            }

            heartbeatTimer = new CTimer(SendHeartbeat, null, 0, 15000);

            if (debug == 1 || debug == 2)
                ErrorLog.Notice("QsysProcessor is initialized.");
            isInitialized = true;

            if (onIsRegistered != null)
                onIsRegistered(1);
        }

        private void SendHeartbeat(object o)
        {
            commandQueue.Enqueue(JsonConvert.SerializeObject(new Heartbeat()));
        }

        /// <summary>
        /// Cleans up all resources.
        /// </summary>
        public void Dispose()
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

        private void CommandQueueDequeue(object o)
        {
            try
            {
                if (!commandQueue.IsEmpty)
                {
                    var data = commandQueue.TryToDequeue();

                    if (debug == 2)
                    {
                        CrestronConsole.PrintLine("Command sent ** {0} **", data);
                    }

                    client.SendCommand(data + "\x00");
                }
            }
            catch (Exception e)
            {
            }
        }

        StringBuilder RxData = new StringBuilder();
        bool busy = false;
        int Pos = -1;
        private void ResponseQueueDequeue(object o)
        {
            try
            {
                if (!responseQueue.IsEmpty)
                {
                    // removes string from queue, blocks until an item is queued
                    string tmpString = responseQueue.TryToDequeue();

                    RxData.Append(tmpString); //Append received data to the COM buffer

                    if (!busy && RxData.Length > 0 && RxData.ToString().Contains("\x00"))
                    {
                        busy = true;
                        while (RxData.ToString().Contains("\x00"))
                        {
                            Pos = RxData.ToString().IndexOf("\x00");
                            var data = RxData.ToString().Substring(0, Pos);
                            var garbage = RxData.Remove(0, Pos + 1); // remove data from COM buffer

                            if (debug == 2)
                                CrestronConsole.PrintLine("Response found ** {0} **", data);

                            ParseInternalResponse(data);
                        }

                        busy = false;
                    }
                }
            }
            catch (Exception e)
            {
                busy = false;
                if (debug == 1 || debug == 2)
                    ErrorLog.Error("Error in QsysProcessor ResponseQueueDequeue: {0}", e.Message);
            }
        }

        private void ParseInternalResponse(string returnString)
        {
            try
            {
                if (returnString.Length > 0)
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
                            else if (changeResult.Name != null)
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

                            if (onNewCoreStatus != null)
                                onNewCoreStatus(designName, Convert.ToUInt16(isRedundant), Convert.ToUInt16(isEmulator));

                            /*foreach (var item in SimplClients)
                            {
                                item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.NewCoreStatus, string.Empty, 1));
                            }*/
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
            }
            catch (Exception e)
            {
                if (debug == 1 || debug == 2)
                    ErrorLog.Error("Error in QsysProcessor ParseInternalResponse: {0}:\r\n{1}", e.Message, returnString);
            }
        }

        internal void Enqueue(string data)
        {
            if (data.Length > 0)
                commandQueue.Enqueue(data);
        }

        /// <summary>
        /// Parse response from Q-Sys Core.
        /// </summary>
        /// <param name="data"></param>
        public void ParseResponse(string data)
        {
            try
            {
                responseQueue.Enqueue(data);
            }
            catch (Exception e)
            {
            }
        }
    }
}


 
