using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCP_Client;

namespace QscQsys
{
    /// <summary>
    /// Qsys Core
    /// </summary>
    public class QsysCore
    {
        //Core specific coms
        private int coreID;
        private string loginUser;
        private string loginPass;
        private string coreIP = "";
        public string getCoreIP { get { return this.coreIP; } }
        private int corePort = 0;
        private TCPClientDevice client;
        private bool isConnected = false;
        public bool IsConnected { get { return this.isConnected; } }
        private bool loginAttempt = false;
        private bool badLogin = false;
        public bool BadLogin { get { return this.badLogin; } }

        //Module vars
        private bool debug;
        public bool IsDebugMode { get { return this.debug; } }
        private bool isInitialized;
        private bool isDisposed;
        public bool IsDisposed { get { return this.isDisposed; } }
        private bool loggedIn;
        public bool IsInitialized { get { return this.isInitialized; } }
        

        //Queues
        private CrestronQueue<string> commandQueue;
        private CrestronQueue<string> responseQueue;

        //Timers
        private CTimer commandQueueTimer;
        private CTimer responseQueueTimer;
        private CTimer heartbeatTimer;
        
        //Core info
        private eCoreState coreState;
        public eCoreState CoreState { get { return coreState; } }
        private string platform;
        public string Platform { get { return platform; } }
        private string designName;
        public string DesignName { get { return designName; } }
        private string designCode;
        public string DesignCode { get { return designCode; } }
        private bool isRedundant;
        public bool IsRedundant { get { return isRedundant; } }
        private bool isEmulator;
        public bool IsEmulator { get { return isEmulator; } }
        private int statusCode;
        public int StatusCode { get { return statusCode; } }
        private string statusString;
        public string StatusString { get { return statusString; } }

        //Components & controls & clients
        internal Dictionary<string, InternalEvents> Controls = new Dictionary<string, InternalEvents>();
        internal Dictionary<Component, InternalEvents> Components = new Dictionary<Component, InternalEvents>();
        internal Dictionary<string, SimplEvents> SimplClients = new Dictionary<string, SimplEvents>();



        /// <summary>
        /// Initialize new core
        /// </summary>
        public bool Initialize(int _coreID, string _host, ushort _port, string _user, string _pass)
        {
            bool ret = false;
            coreID = _coreID;
            coreIP = _host;
            corePort = _port;
            loginUser = _user;
            loginPass = _pass;
            if (QsysMain.AddCore(this, this.coreID))
            {
                this.SendDebug(string.Format("Add core {0} @ {1}:{2} & initialize", coreID, coreIP, corePort));
                this.commandQueue = new CrestronQueue<string>();
                this.responseQueue = new CrestronQueue<string>();
                this.commandQueueTimer = new CTimer(CommandQueueDequeue, null, 0, 50);
                this.responseQueueTimer = new CTimer(ResponseQueueDequeue, null, 0, 50);
                this.initializeConnection();
                ret = true;
            }
            else
            {
                this.SendDebug(string.Format("Error adding core {0} - already exists", this.coreID));
                ErrorLog.Error("Error adding core {0} - already exists", this.coreID);
            }
            return ret;
        }

        private void initializeConnection()
        {
            if (this.coreIP.Length > 0)
            {
                this.client = new TCPClientDevice();
                this.client.ID = this.coreID;
                this.client.ConnectionStatus += new StatusEventHandler(client_ConnectionStatus);
                this.client.ResponseString += new ResponseEventHandler(client_ResponseString);
                this.client.Connect(this.coreIP, (ushort)this.corePort);
            }
        }


        internal bool RegisterControl(string _control)
        {
            try
            {
                lock (Controls)
                {
                    if (!Controls.ContainsKey(_control))
                    {
                        this.Controls.Add(_control, new InternalEvents());
                        this.SendDebug(string.Format("Adding named control: {0}", _control));
                        if (isInitialized && isConnected)
                        {
                            AddControlToChangeGroup addControl;

                            addControl = new AddControlToChangeGroup();
                            addControl.method = "ChangeGroup.AddControl";
                            addControl.ControlParams = new AddControlToChangeGroupParams();
                            addControl.ControlParams.Controls = new List<string>();
                            addControl.ControlParams.Controls.Add(_control);
                            this.SendDebug(string.Format("Adding named control: {0} to core change group", _control));
                            this.commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error registering Named Control to the Qsys core {0} : {1}", this.coreID, e.Message);
                return false;
            }
        }

        internal bool RegisterComponent(Component _component)
        {
            try
            {
                lock (Components)
                {
                    if (!Components.ContainsKey(_component))
                    {
                        Components.Add(_component, new InternalEvents());
                        this.SendDebug(string.Format("Adding named component {0}", _component.Name));
                        if (isInitialized && IsConnected)
                        {
                            AddComponentToChangeGroup addComponent;
                            addComponent = new AddComponentToChangeGroup();
                            addComponent.method = "ChangeGroup.AddComponentControl";
                            addComponent.ComponentParams = new AddComponentToChangeGroupParams();
                            addComponent.ComponentParams.Component = _component;
                            this.SendDebug(string.Format("Adding named component: {0} to core change group", _component.Name));
                            this.commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error registering Named Component to the Qsys Core {0} : {1}", this.coreID, e.Message);
                return false;
            }
        }

        public bool RegisterSimplClient(string _id)
        {
            try
            {
                lock (SimplClients)
                {
                    if(!SimplClients.ContainsKey(_id))
                    {
                        this.SimplClients.Add(_id, new SimplEvents());
                    }
                }
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public void setDebug(bool _value)
        {
            this.debug = _value;
        }

        void client_ResponseString(string _response, int _id)
        {
            this.ParseResponse(_response);
        }

        void client_ConnectionStatus(int _status, int _id)
        {
            try
            {
                if (_status == 2 && !isConnected)
                {
                    this.isConnected = true;
                    foreach (var item in this.SimplClients)
                    {
                        item.Value.Fire( new SimplEventArgs(eQscSimplEventIds.IsConnected,(SimplSharpString)"true", 1));
                    }
                    CrestronEnvironment.Sleep(1500);

                    //Send login if needed
                    if (this.loginUser.Length > 0 || this.loginPass.Length > 0)
                    {
                        this.SendLogin();
                    }

                    this.CoreModuleInit();

                    this.heartbeatTimer = new CTimer(SendHeartbeat, null, 0, 15000);

                    this.SendDebug("Initialized");
                    this.isInitialized = true;

                    foreach (var item in this.SimplClients)
                    {
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRegistered, (SimplSharpString)"true", 1));
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsConnected, (SimplSharpString)"true", 1));
                    }
                }
                else if (this.isConnected && _status != 2)
                {
                    ErrorLog.Error("Qsys Core {0} disconnected!", this.coreID);
                    this.isConnected = false;
                    this.isInitialized = false;
                    this.heartbeatTimer.Dispose();
                    foreach (var item in this.SimplClients)
                    {
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsRegistered, (SimplSharpString) "false", 0));
                        item.Value.Fire(new SimplEventArgs(eQscSimplEventIds.IsConnected, (SimplSharpString)"false", 0));
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Qsys Core {0} connection failure {1} - {2}", this.coreID, e.Message, e.StackTrace);
            }
        }

        void SendLogin()
        {
            this.SendDebug(string.Format("Qsys - Sending login: {0}:{1}", this.loginUser, this.loginPass));
            CoreLogon logon = new CoreLogon();
            logon.Params = new CoreLogonParams();
            logon.Params.User = loginUser;
            logon.Params.Password = loginPass;
            this.commandQueue.Enqueue(JsonConvert.SerializeObject(logon));
        }

        private void CoreModuleInit()
        {
            this.SendDebug("Requesting all named components and controls");

            this.commandQueue.Enqueue(JsonConvert.SerializeObject(new GetComponents()));

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
                    this.SendDebug(string.Format("Adding named control: {0} to change group", item.Key));
                }
                this.commandQueue.Enqueue(JsonConvert.SerializeObject(addControls));
            }

            if (Components.Count() > 0)
            {
                AddComponentToChangeGroup addComponents;
                foreach (var item in Components)
                {
                    addComponents = new AddComponentToChangeGroup();
                    addComponents.method = "ChangeGroup.AddComponentControl";
                    addComponents.ComponentParams = new AddComponentToChangeGroupParams();
                    addComponents.ComponentParams.Component = item.Key;
                    this.SendDebug(string.Format("Adding named component: {0} to change group", item.Key));
                    commandQueue.Enqueue(JsonConvert.SerializeObject(addComponents));
                }
                this.commandQueue.Enqueue(JsonConvert.SerializeObject(new CreateChangeGroup()));
            }
        }

        private void SendHeartbeat(object _o)
        {
            this.commandQueue.Enqueue(JsonConvert.SerializeObject(new Heartbeat()));
        }

        /// <summary>
        /// Cleans up all resources.
        /// </summary>
        public void Dispose()
        {
            if (this.isInitialized)
            {
                this.client.Disconnect();
                this.commandQueue.Dispose();
                this.commandQueueTimer.Stop();
                this.commandQueueTimer.Dispose();

                if (!this.heartbeatTimer.Disposed)
                {
                    this.heartbeatTimer.Stop();
                    this.heartbeatTimer.Dispose();
                }
                
                this.isDisposed = true;
            }
        }

        private void CommandQueueDequeue(object _o)
        {
            if (!this.commandQueue.IsEmpty)
            {
                var data = this.commandQueue.Dequeue();

                this.SendDebug(string.Format("Sending to core from queue: {0}", data));
                this.client.SendCommand(data + "\x00");
            }
        }

        StringBuilder RxData = new StringBuilder();
        bool busy = false;
        int Pos = -1;
        private void ResponseQueueDequeue(object _o)
        {
            if (!this.responseQueue.IsEmpty)
            {
                try
                {
                    string tmpString = this.responseQueue.Dequeue(); // removes string from queue, blocks until an item is queued

                    this.RxData.Append(tmpString); //Append received data to the COM buffer

                    if (!this.busy)
                    {
                        this.busy = true;
                        while (this.RxData.ToString().Contains("\x00"))
                        {
                            this.Pos = this.RxData.ToString().IndexOf("\x00");
                            var data = this.RxData.ToString().Substring(0, Pos);
                            var garbage = this.RxData.Remove(0, Pos + 1); // remove data from COM buffer

                            this.ParseInternalResponse(data);
                        }

                        this.busy = false;
                    }
                }
                catch (Exception e)
                {
                    this.busy = false;
                    ErrorLog.Exception(e.Message, e);
                }
            }
        }

        private void ParseInternalResponse(string _returnString)
        {
            if (_returnString.Length > 0)
            {
                try
                {
                    JObject response = JObject.Parse(_returnString);

                    if (_returnString.Contains("Changes"))
                    {
                        IList<JToken> changes = response["params"]["Changes"].Children().ToList();

                        IList<ComponentChangeResult> changeResults = new List<ComponentChangeResult>();

                        foreach (JToken change in changes)
                        {
                            //ChangeResult changeResult = (ChangeResult)change.Cast<ChangeResult>();

                            ComponentChangeResult changeResult = JsonConvert.DeserializeObject<ComponentChangeResult>(change.ToString());

                            if (changeResult.Component != null)
                            {
                                foreach (var item in this.Components)
                                {
                                    if (item.Key.Name == changeResult.Component)
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.String));
                                }
                            }
                            else
                            {
                                foreach (var item in this.Controls)
                                {
                                    if (item.Key == changeResult.Name)
                                        item.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.String));
                                }
                            }
                        }
                    }
                    else if (_returnString.Contains("Properties"))
                    {
                        IList<JToken> components = response["result"].Children().ToList();

                        IList<ComponentResults> componentResults = new List<ComponentResults>();

                        foreach (var component in components)
                        {
                            ComponentResults result = JsonConvert.DeserializeObject<ComponentResults>(component.ToString());

                            if (result.Type == "gain")
                            {
                                foreach (var item in this.Components)
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
                    else if (_returnString.Contains("EngineStatus") || _returnString.Contains("StatusGet"))
                    {
                        EngineStatusResult statusResult = JsonConvert.DeserializeObject<EngineStatusResult>(_returnString);
                        this.coreState = (eCoreState)Enum.Parse(typeof(eCoreState), statusResult.Properties.State, true);
                        this.platform = statusResult.Properties.Platform;
                        this.designName = statusResult.Properties.DesignName;
                        this.designCode = statusResult.Properties.DesignCode;
                        this.isRedundant = Convert.ToBoolean(statusResult.Properties.IsRedundant);
                        this.isEmulator = Convert.ToBoolean(statusResult.Properties.IsEmulator);
                        this.statusCode = statusResult.Properties.Status.Code;
                        this.statusString = statusResult.Properties.Status.String;
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
                    else if (_returnString.Contains("error"))
                    {
                        CrestronConsole.PrintLine("got error");
                        CoreError err = JsonConvert.DeserializeObject<CoreError>(_returnString);
                        //this.SendDebug(string.Format("core error message - {1}-{2}", err.error.code, err.error.message));
                    }
                }
                catch (Exception e)
                {
                    this.SendDebug(String.Format("Parse internal error: {1}:\r\n{2}", this.coreID, e.Message, _returnString));
                }
            }
        }

        internal void Enqueue(string _data)
        {
            this.commandQueue.Enqueue(_data);
        }

        //private static MemoryStream _memStream = new MemoryStream();
        /// <summary>
        /// Parse response from Q-Sys Core.
        /// </summary>
        /// <param name="data"></param>
        public void ParseResponse(string _data)
        {
            //gather.Gather(data);
            try
            {
                this.SendDebug(string.Format("Received from core and adding to queue: {0}", _data));
                this.responseQueue.Enqueue(_data);
            }
            catch (Exception e)
            {
            }
        }

        public void SendDebug(string _msg)
        {
            if (debug)
                CrestronConsole.PrintLine("Qsys Core {0} Debug: {1}", this.coreID, _msg);
        }


    }
    public enum eCoreState
    {
        Idle = 0,
        Active = 1,
        Standby = 2
    }
}


 
