using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;             				// For Basic SIMPL# Classes
using ExtensionMethods;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TCP_Client;

namespace QscQsys
{
    /// <summary>
    /// Q-SYS Core class that manages connection and parses responses to be dsitributed to components and named control classes
    /// </summary>
    public class QsysCore : IDisposable
    {
        #region Delegates
        public delegate void IsLoggedIn(SimplSharpString id, ushort value);
        public delegate void IsRegistered(SimplSharpString id, ushort value);
        public delegate void IsConnectedStatus(SimplSharpString id, ushort value);
        public delegate void CoreStatus(SimplSharpString id, SimplSharpString designName, ushort isRedundant, ushort isEmulator);
        public delegate void SendingCommand(SimplSharpString id, SimplSharpString command);
        public IsLoggedIn onIsLoggedIn { get; set; }
        public IsRegistered onIsRegistered { get; set; }
        public IsConnectedStatus onIsConnected { get; set; }
        public CoreStatus onNewCoreStatus { get; set; }
        public SendingCommand onSendingCommand { get; set; }
        #endregion

        private readonly CrestronQueue<string> commandQueue = new CrestronQueue<string>(1000);
        //private readonly CrestronQueue<string> responseQueue = new CrestronQueue<string>(100);
        private CTimer commandQueueTimer;
        //private CTimer responseQueueTimer;
        private CTimer heartbeatTimer;
        private CTimer waitForConnection;
        private TCPClientDevice client;
        private StringBuilder RxData = new StringBuilder();
        private readonly object responseLock = new object();
        private readonly object parseLock = new object();
        private bool isInitialized;
        private bool isLoggedIn;
        private bool isDisposed;
        private ushort debug;
        private ushort logonAttempts;
        private ushort maxLogonAttempts = 2;
        private bool isRedundant;
        private bool isEmulator;
        private bool externalConnection;
        private bool changeGroupCreated;

        private string designName;
        private string coreId;
        private string username;
        private string password;

        internal Dictionary<Component, InternalEvents> Components = new Dictionary<Component, InternalEvents>();
        internal Dictionary<Control, InternalEvents> Controls = new Dictionary<Control, InternalEvents>();

        public QsysCore()
        {
            heartbeatTimer = new CTimer(SendHeartbeat, Timeout.Infinite);
            commandQueueTimer = new CTimer(CommandQueueDequeue, null, 0, 50);
            //responseQueueTimer = new CTimer(ResponseQueueDequeue, null, 0, 10);
            waitForConnection = new CTimer(Initialize, Timeout.Infinite);
        }


        #region Properties
        /// <summary>
        /// Get initialzation status
        /// </summary>
        public bool IsInitialized { get { return isInitialized; } }

        /// <summary>
        /// Get disposed status
        /// </summary>
        public bool IsDisposed { get { return isDisposed; } }

        /// <summary>
        /// Get connection status
        /// </summary>
        public bool IsConnected { get; private set; }

        /// <summary>
        /// Get debug mode
        /// </summary>
        public ushort IsDebugMode { get { return debug; } }

        /// <summary>
        /// Get or set  max logon attempts
        /// </summary>
        public ushort MaxLogonAttemps { get { return maxLogonAttempts; } set { maxLogonAttempts = value; } }

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
                            var addComponent = new AddComoponentToChangeGroup() { method = "ChangeGroup.AddComponentControl", ComponentParams = new AddComponentToChangeGroupParams() { Component = component } };
                            commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));

                            StartAutoPoll();

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
                            var addControl = new AddControlToChangeGroup() { method = "ChangeGroup.AddControl", ControlParams = new AddControlToChangeGroupParams() { Controls = new List<string>() { control.Name } } };
                            commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));

                            StartAutoPoll();

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

        /// <summary>
        /// Get redundant status
        /// </summary>
        public bool IsRedundant { get { return isRedundant; } }

        /// <summary>
        /// Get emulator status
        /// </summary>
        public bool IsEmulator { get { return isEmulator; } }

        /// <summary>
        /// Get running design name
        /// </summary>
        public string DesignName { get { return designName; } }

        /// <summary>
        /// Get core ID
        /// </summary>
        public string CoreId { get { return coreId; } }
        #endregion

        /// <summary>
        /// Set dbug mode.
        /// </summary>
        /// <param name="value"></param>
        public void Debug(ushort value)
        {
            debug = value;

            if (debug > 0)
            {
                CrestronConsole.PrintLine("********Qsys Debug Mode Active********");
                CrestronConsole.PrintLine("See log for details");
                ErrorLog.Notice("********Qsys Debug Mode Active********");
                if (QsysCoreManager.Is3Series)
                {
                    CrestronConsole.PrintLine("********Qsys Running On 3-Series********");
                    ErrorLog.Notice("********Qsys Running On 3-Series********");
                }
                else
                {
                    CrestronConsole.PrintLine("********Qsys Running On 4-Series Or Greater********");
                    ErrorLog.Notice("********Qsys Running On 4-Series Or Greater********");
                }
                
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

        #region Initialization
        /// <summary>
        /// Initialzes all methods that are required to setup the class. Connection is established on port 1702.
        /// </summary>
        public void Initialize(string id, string host, ushort port, string username, string password, ushort useExternalConnection)
        {
            if (!isInitialized)
            {
                try
                {
                    coreId = id;

                    externalConnection = Convert.ToBoolean(useExternalConnection);

                    if (username.Length > 0)
                        this.username = username;
                    else
                        this.username = string.Empty;

                    if (password.Length > 0)
                        this.password = password;
                    else
                        this.password = string.Empty;

                    QsysCoreManager.AddCore(this);

                    if (debug == 1)
                        ErrorLog.Notice("QsysProcessor is initializing.");

                    if (useExternalConnection == 0)
                    {
                        client = new TCPClientDevice();

                        client.Debug = debug;

                        client.ID = id;
                        client.ConnectionStatus += new StatusEventHandler(client_ConnectionStatus);
                        client.ResponseString += new ResponseEventHandler(client_ResponseString);
                        client.Connect(host, port);
                    }
                }
                catch (Exception e)
                {
                    if (debug > 0)
                        ErrorLog.Error("Error in QsysProcessor Iniitialize: {0}", e.Message);
                }
            }
        }

        private void Initialize(object o)
        {
            lock (Components)
            {
                foreach (var item in Components)
                {
                    var addComponent = new AddComoponentToChangeGroup() { method = "ChangeGroup.AddComponentControl", ComponentParams = new AddComponentToChangeGroupParams() { Component = item.Key } };
                    commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));
                    StartAutoPoll();
                }
            }

            lock (Controls)
            {
                foreach (var item in Controls)
                {
                    var addControl = new AddControlToChangeGroup() { method = "ChangeGroup.AddControl", ControlParams = new AddControlToChangeGroupParams() { Controls = new List<string>() { item.Key.Name } } };
                    commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
                    StartAutoPoll();
                }
            }

            heartbeatTimer.Reset(15000, 15000);

            if (debug == 1 || debug == 2)
                ErrorLog.Notice("QsysProcessor is initialized.");

            isInitialized = true;

            if (onIsRegistered != null)
                onIsRegistered(coreId, 1);
        }

        private void StartAutoPoll()
        {
            if (!changeGroupCreated)
            {
                commandQueue.Enqueue(JsonConvert.SerializeObject(new CreateChangeGroup()));
                changeGroupCreated = true;
            }
        }

        #endregion

        #region TCP Client Events
        private void client_ResponseString(string response, SimplSharpString id)
        {
            //responseQueue.Enqueue(response);
            ProcessResponse(response);
        }

        private void client_ConnectionStatus(int status, SimplSharpString id)
        {
            try
            {
                if (status == 2 && !IsConnected)
                {
                    IsConnected = true;

                    if (debug > 0)
                        ErrorLog.Notice("QsysProcessor is connected.");

                    if (onIsConnected != null)
                        onIsConnected(coreId,1);
                }
                else if (IsConnected && status != 2)
                {
                    IsConnected = false;

                    if (debug > 0)
                        ErrorLog.Error("QsysProcessor disconnected!");

                    changeGroupCreated = false;
                    isLoggedIn = false;
                    isInitialized = false;

                    heartbeatTimer.Stop();

                    if (onIsRegistered != null)
                        onIsRegistered(coreId, 0);

                    if (onIsLoggedIn != null)
                        onIsLoggedIn(coreId, 0);

                    if (onIsConnected != null)
                        onIsConnected(coreId, 0);
                }
            }
            catch (Exception e)
            {
                if (debug > 0)
                    ErrorLog.Error("Error in QsysProcessor client_ConnectionStatus: {0}", e.Message);
            }
        }

        private void SendHeartbeat(object o)
        {
            commandQueue.Enqueue(JsonConvert.SerializeObject(new Heartbeat()));
        }
        #endregion

        #region Parsing
        /*private void ResponseQueueDequeue(object o)
        {
            try
            {
                if (!responseQueue.IsEmpty)
                {
                    // removes string from queue, blocks until an item is queued
                    string tmpString = responseQueue.Dequeue();

                    lock (parseLock)
                    {
                        RxData.Append(tmpString); //Append received data to the COM buffer
                    }

                    if (CMonitor.TryEnter(responseLock))
                    {
                        while (RxData.ToString().Contains("\x00"))
                        {
                            var responseData = string.Empty;

                            lock (parseLock)
                            {
                                responseData = RxData.ToString();
                                var delimeterPos = responseData.IndexOf("\x00");
                                responseData = responseData.Substring(0, delimeterPos);
                                RxData.Remove(0, delimeterPos + 1);
                            }

                            if (debug == 2)
                                CrestronConsole.PrintLine("Response found ** {0} **", responseData);
                            
                            new CTimer(ParseInternalResponse, responseData, 0);
                        }
                        CMonitor.Exit(responseLock);
                    }
                }
            }
            catch (Exception e)
            {
                CMonitor.Exit(responseLock);

                if (debug == 1 || debug == 2)
                    ErrorLog.Error("Error in QsysProcessor ResponseQueueDequeue: {0}", e.Message);
            }
        }*/

        private void ProcessResponse(string response)
        {
            lock (parseLock)
            {
                RxData.Append(response); //Append received data to the COM buffer
            }

            if (CMonitor.TryEnter(responseLock))
            {
                while (RxData.ToString().Contains("\x00"))
                {
                    var responseData = string.Empty;

                    lock (parseLock)
                    {
                        responseData = RxData.ToString();
                        var delimeterPos = responseData.IndexOf("\x00");
                        responseData = responseData.Substring(0, delimeterPos);
                        RxData.Remove(0, delimeterPos + 1);
                    }

                    if (debug == 2)
                        CrestronConsole.PrintLine("Response found ** {0} **", responseData);

                    new CTimer(ParseInternalResponse, responseData, 0);
                }

                CMonitor.Exit(responseLock);
            }
        }

        private void ParseInternalResponse(object o)
        {
            try
            {
                var returnString = o as string;

                if (returnString != null)
                {
                    if (returnString.Length > 0 && ((IsConnected && !externalConnection) || externalConnection))
                    {
                        if (returnString == "{\"jsonrpc\":\"2.0\",\"result\":true,\"id\":\"crestron\"}")
                        {
                            if (!isLoggedIn)
                            {
                                isLoggedIn = true;

                                if (onIsLoggedIn != null)
                                {
                                    onIsLoggedIn(coreId, 1);
                                }


                                waitForConnection.Reset(5000);
                            }
                        }
                        else
                        {
                            if (returnString.Contains("Changes") && !returnString.Contains("\"Changes\":[]"))
                            {
                                var response = JObject.Parse(returnString);
                                var changes = response["params"]["Changes"].Children().ToList();
                                response = null;

                                //var changeResults = new List<ChangeResult>();

                                foreach (JToken change in changes)
                                {
                                    var changeResult = JsonConvert.DeserializeObject<ChangeResult>(change.ToString(), new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Ignore });

                                    if (changeResult.Component != null)
                                    {
                                        List<string> choices;

                                        if (changeResult.Choices != null)
                                            choices = changeResult.Choices.ToList();
                                        else
                                            choices = new List<string>();

                                        //var component = Components.First(x => x.Key.Name == changeResult.Component);
                                        var components = Components.Where(x => x.Key.Name == changeResult.Component);

                                        foreach (var component in components)
                                        {
                                            if (component.Key != null)
                                                component.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.Position, changeResult.String, choices));
                                        }
                                        
                                    }
                                    else if (changeResult.Name != null)
                                    {
                                        List<string> choices;

                                        if (changeResult.Choices != null)
                                            choices = changeResult.Choices.ToList();
                                        else
                                            choices = new List<string>();

                                        var control = Controls.First(x => x.Key.Name == changeResult.Name);
                                        if (control.Key != null)
                                            control.Value.Fire(new QsysInternalEventsArgs(changeResult.Name, changeResult.Value, changeResult.Position, changeResult.String, choices));

                                    }

                                    changeResult = null;
                                }
                            }
                            else if (returnString.Contains("EngineStatus"))
                            {
                                var response = JObject.Parse(returnString);

                                if (externalConnection)
                                {
                                    isLoggedIn = false;
                                }
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
                                        onNewCoreStatus(coreId, designName, Convert.ToUInt16(isRedundant), Convert.ToUInt16(isEmulator));
                                }

                                if (!isLoggedIn)
                                {
                                    if (debug == 1 || debug == 2)
                                        ErrorLog.Notice("QsysProcessor server ready, starting to send intialization strings.");

                                    if (password.Length > 0 && username.Length > 0)
                                    {
                                        logonAttempts = 1;
                                        commandQueue.Enqueue(JsonConvert.SerializeObject(new Logon() { Params = new LogonParams() { User = username, Password = password } }));
                                    }
                                    else
                                    {
                                        isLoggedIn = true;

                                        if (onIsLoggedIn != null)
                                        {
                                            onIsLoggedIn(coreId, 1);
                                        }

                                        waitForConnection.Reset(5000);
                                    }
                                }
                            }
                            else if (returnString.Contains("error"))
                            {
                                var response = JObject.Parse(returnString);

                                if (logonAttempts < maxLogonAttempts)
                                {
                                    JToken error = response["error"];

                                    if (error["code"] != null)
                                    {
                                        if (error["code"].ToString().Replace("\'", string.Empty) == "10")
                                        {
                                            logonAttempts++;
                                            commandQueue.Enqueue(JsonConvert.SerializeObject(new Logon() { Params = new LogonParams() { User = username, Password = password } }));
                                        }
                                    }
                                }
                                else
                                {
                                    if (debug > 0)
                                    {
                                        ErrorLog.Error("Error in QsysProcessor max logon attempts reached");
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (debug > 0)
                    ErrorLog.Error("Error in QsysProcessor ParseInternalResponse: {0}", e.Message);
            }
        }

        /// <summary>
        /// Enqueue response from SIMPL to be parsed
        /// </summary>
        /// <param name="response">Response from SIMPL to be parsed</param>
        public void NewExternalResponse(string response)
        {
            //responseQueue.Enqueue(response);
            ProcessResponse(response);
        }
        #endregion

        #region Command Queue
        internal void Enqueue(string data)
        {
            if (data.Length > 0)
                commandQueue.Enqueue(data);
        }

        private void CommandQueueDequeue(object o)
        {
            try
            {
                if (!commandQueue.IsEmpty)
                {
                    var data = commandQueue.TryToDequeue();

                    if (data != null)
                    {
                        if (debug == 2)
                        {
                            CrestronConsole.PrintLine("Command sent -->{0}<--", data);
                        }

                        if (!externalConnection)
                            client.SendCommand(data + "\x00");
                        //else if (SendingCommandEvent != null)
                        //    SendingCommandEvent(this, new SendingCommandEventArgs(data + "\x00"));
                        else if (onSendingCommand != null)
                        {
                            data = data + "\x00";
                            var xs = data.Chunk(200);

                            foreach (var x in xs)
                            {
                                if (debug == 2)
                                    CrestronConsole.PrintLine("Command chuck sent externally length={0} -->{1}<--", x.Length, x);
                                onSendingCommand(coreId, x);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (debug > 0)
                    ErrorLog.Error("Error in QsysProcessor CommandQueueDequeue: {0}", e.Message);

                commandQueue.Clear();
            }
        }
        #endregion

        /// <summary>
        /// Clean up of unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            isDisposed = true;
        }

        private void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    changeGroupCreated = false;
                    isLoggedIn = false;
                    isInitialized = false;

                    waitForConnection.Stop();
                    waitForConnection.Dispose();

                    heartbeatTimer.Stop();
                    heartbeatTimer.Dispose();

                    commandQueueTimer.Stop();
                    commandQueueTimer.Dispose();
                    commandQueue.Dispose();

                    RxData.Remove(0, RxData.Length);

                    maxLogonAttempts = 2;
                    debug = 0;
                }

                client.ConnectionStatus -= client_ConnectionStatus;
                client.ResponseString -= client_ResponseString;
                client.Disconnect();
            }
        }
    }
}