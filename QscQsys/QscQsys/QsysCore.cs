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

        private readonly CrestronQueue<string> _commandQueue = new CrestronQueue<string>(1000);
        //private readonly CrestronQueue<string> responseQueue = new CrestronQueue<string>(100);
        private CTimer _commandQueueTimer;
        //private CTimer responseQueueTimer;
        private CTimer _heartbeatTimer;
        private CTimer _waitForConnection;
        private TCPClientDevice _client;
        private StringBuilder _rxData = new StringBuilder();
        private readonly object _responseLock = new object();
        private readonly object _parseLock = new object();
        private bool _isInitialized;
        private bool _isConnected;
        private bool _isLoggedIn;
        private bool _disposed;
        private ushort _debug;
        private ushort _logonAttempts;
        private ushort _maxLogonAttempts = 2;
        private bool _isRedundant;
        private bool _isEmulator;
        private bool _externalConnection;
        private bool _changeGroupCreated;
        private string _designName;
        private string _coreId;
        private string _username;
        private string _password;

        internal Dictionary<Component, InternalEvents> Components = new Dictionary<Component, InternalEvents>();
        internal Dictionary<Control, InternalEvents> Controls = new Dictionary<Control, InternalEvents>();

        public QsysCore()
        {
            _heartbeatTimer = new CTimer(SendHeartbeat, Timeout.Infinite);
            _commandQueueTimer = new CTimer(CommandQueueDequeue, null, 0, 50);
            //responseQueueTimer = new CTimer(ResponseQueueDequeue, null, 0, 10);
            _waitForConnection = new CTimer(Initialize, Timeout.Infinite);
        }


        #region Properties
        /// <summary>
        /// Get initialzation status
        /// </summary>
        public bool IsInitialized { get { return _isInitialized; } }

        /// <summary>
        /// Get disposed status
        /// </summary>
        public bool IsDisposed { get { return _disposed; } }

        /// <summary>
        /// Get connection status
        /// </summary>
        public bool IsConnected { get { return _isConnected; } }

        /// <summary>
        /// Get authentication status
        /// </summary>
        public bool IsAuthenticatedIn { get { return _isLoggedIn; } }

        /// <summary>
        /// Get debug mode
        /// </summary>
        public ushort IsDebugMode { get { return _debug; } }

        /// <summary>
        /// Get or set  max logon attempts
        /// </summary>
        public ushort MaxLogonAttemps { get { return _maxLogonAttempts; } set { _maxLogonAttempts = value; } }   

        /// <summary>
        /// Get redundant status
        /// </summary>
        public bool IsRedundant { get { return _isRedundant; } }

        /// <summary>
        /// Get emulator status
        /// </summary>
        public bool IsEmulator { get { return _isEmulator; } }

        /// <summary>
        /// Get running design name
        /// </summary>
        public string DesignName { get { return _designName; } }

        /// <summary>
        /// Get core ID
        /// </summary>
        public string CoreId { get { return _coreId; } }

        /// <summary>
        /// Set debug mode.
        /// </summary>
        /// <param name="value"></param>
        public void Debug(ushort value)
        {
            _debug = value;

            if (_debug > 0)
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
                
                if (_debug == 1)
                {
                    ErrorLog.Notice("Qsys debug level: Main communications");
                }
                else if (_debug == 2)
                {
                    ErrorLog.Notice("Qsys debug level: Main communications and verbose console");
                }
                ErrorLog.Notice("Qsys TCP ID 1710");
            }
        }

        /// <summary>
        /// Get or set the network port. If currently connected, changing the port will reconnect with the new port number.
        /// </summary>
        public ushort Port
        {
            get
            {
                if (_client != null)
                {
                    return _client.Port;
                }
                else
                {
                    return ushort.MinValue;
                }
            }
            set
            {
                if (_client != null)
                {
                    _client.Port = value;
                }
            }
        }

        /// <summary>
        /// Get or set the network host address. If currently connectd, changing the host will reconnect with the new host address.
        /// </summary>
        public string Host
        {
            get
            {
                if (_client != null)
                {
                    return _client.Host;
                }
                else
                {
                    return string.Empty;
                }
            }
            set
            {
                if (_client != null)
                {
                    _client.Host = value;
                }
            }
        }

        /*public bool UseExternalConnection
        {

        }*/
        #endregion

        #region Initialization
        /// <summary>
        /// Initialzes all methods that are required to setup the class. Connection is established on port 1702.
        /// </summary>
        public void Initialize(string id, string host, ushort port, string username, string password, ushort useExternalConnection)
        {
            if (!_isInitialized)
            {
                try
                {
                    _coreId = id;

                    _externalConnection = Convert.ToBoolean(useExternalConnection);

                    if (username.Length > 0)
                        this._username = username;
                    else
                        this._username = string.Empty;

                    if (password.Length > 0)
                        this._password = password;
                    else
                        this._password = string.Empty;

                    QsysCoreManager.AddCore(this);

                    if (_debug == 1)
                        ErrorLog.Notice("QsysProcessor is initializing.");

                    if (useExternalConnection == 0)
                    {
                        _client = new TCPClientDevice();

                        _client.Debug = _debug;

                        _client.ID = id;
                        _client.ConnectionStatus += new StatusEventHandler(client_ConnectionStatus);
                        _client.ResponseString += new ResponseEventHandler(client_ResponseString);
                        _client.Connect(host, port);
                    }
                }
                catch (Exception e)
                {
                    if (_debug > 0)
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
                    if (item.Key.Subscribe)
                    {
                        var addComponent = new AddComoponentToChangeGroup() { method = "ChangeGroup.AddComponentControl", ComponentParams = new AddComponentToChangeGroupParams() { Component = item.Key } };
                        _commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));
                        StartAutoPoll();
                    }
                }
            }

            lock (Controls)
            {
                foreach (var item in Controls)
                {
                    if (item.Key.Subscribe)
                    {
                        var addControl = new AddControlToChangeGroup() { method = "ChangeGroup.AddControl", ControlParams = new AddControlToChangeGroupParams() { Controls = new List<string>() { item.Key.Name } } };
                        _commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));
                        StartAutoPoll();
                    }
                }
            }

            _heartbeatTimer.Reset(15000, 15000);

            if (_debug == 1 || _debug == 2)
                ErrorLog.Notice("QsysProcessor is initialized.");

            _isInitialized = true;

            if (onIsRegistered != null)
                onIsRegistered(_coreId, 1);
        }

        private void StartAutoPoll()
        {
            if (!_changeGroupCreated)
            {
                _commandQueue.Enqueue(JsonConvert.SerializeObject(new CreateChangeGroup()));
                _changeGroupCreated = true;
            }
        }

        internal bool RegisterComponent(Component component)
        {
            try
            {
                lock (Components)
                {
                    if (!Components.ContainsKey(component))
                    {
                        Components.Add(component, new InternalEvents());

                        if (_isInitialized && _isConnected && component.Subscribe)
                        {
                            var addComponent = new AddComoponentToChangeGroup() { method = "ChangeGroup.AddComponentControl", ComponentParams = new AddComponentToChangeGroupParams() { Component = component } };
                            _commandQueue.Enqueue(JsonConvert.SerializeObject(addComponent));

                            StartAutoPoll();
                        }
                        if (_debug == 2)
                            CrestronConsole.PrintLine("Registered {0} Component", component.Name);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if (_debug == 1 || _debug == 2)
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

                        if (_isInitialized && _isConnected && control.Subscribe)
                        {
                            var addControl = new AddControlToChangeGroup() { method = "ChangeGroup.AddControl", ControlParams = new AddControlToChangeGroupParams() { Controls = new List<string>() { control.Name } } };
                            _commandQueue.Enqueue(JsonConvert.SerializeObject(addControl));

                            StartAutoPoll();
                        }

                        if (_debug == 2)
                            CrestronConsole.PrintLine("Registered {0} Control", control.Name);
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                if (_debug == 1 || _debug == 2)
                    ErrorLog.Error("Error registering QsysClient to the QsysProcessor: {0}", e.Message);
                return false;
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
                if (status == 2 && !_isConnected)
                {
                    _isConnected = true;

                    if (_debug > 0)
                        ErrorLog.Notice("QsysProcessor is connected.");

                    if (onIsConnected != null)
                        onIsConnected(_coreId,1);
                }
                else if (_isConnected && status != 2)
                {
                    _isConnected = false;

                    if (_debug > 0)
                        ErrorLog.Error("QsysProcessor disconnected!");

                    _changeGroupCreated = false;
                    _isLoggedIn = false;
                    _isInitialized = false;

                    _commandQueue.Clear();
                    _heartbeatTimer.Stop();

                    if (onIsRegistered != null)
                        onIsRegistered(_coreId, 0);

                    if (onIsLoggedIn != null)
                        onIsLoggedIn(_coreId, 0);

                    if (onIsConnected != null)
                        onIsConnected(_coreId, 0);
                }
            }
            catch (Exception e)
            {
                if (_debug > 0)
                    ErrorLog.Error("Error in QsysProcessor client_ConnectionStatus: {0}", e.Message);
            }
        }

        private void SendHeartbeat(object o)
        {
            _commandQueue.Enqueue(JsonConvert.SerializeObject(new Heartbeat()));
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
            lock (_parseLock)
            {
                _rxData.Append(response); //Append received data to the COM buffer
            }

            if (CMonitor.TryEnter(_responseLock))
            {
                while (_rxData.ToString().Contains("\x00"))
                {
                    var responseData = string.Empty;

                    lock (_parseLock)
                    {
                        responseData = _rxData.ToString();
                        var delimeterPos = responseData.IndexOf("\x00");
                        responseData = responseData.Substring(0, delimeterPos);
                        _rxData.Remove(0, delimeterPos + 1);
                    }

                    if (_debug == 2)
                        CrestronConsole.PrintLine("Response found ** {0} **", responseData);

                    new CTimer(ParseInternalResponse, responseData, 0);
                }

                CMonitor.Exit(_responseLock);
            }
        }

        private void ParseInternalResponse(object o)
        {
            try
            {
                var returnString = o as string;

                if (returnString != null)
                {
                    if (returnString.Length > 0 && ((_isConnected && !_externalConnection) || _externalConnection))
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

                            if (_externalConnection)
                            {
                                _isLoggedIn = false;
                            }
                            if (response["params"] != null)
                            {
                                JToken engineStatus = response["params"];

                                if (engineStatus["DesignName"] != null)
                                {
                                    _designName = engineStatus["DesignName"].ToString();
                                }

                                if (engineStatus["IsRedundant"] != null)
                                {
                                    _isRedundant = Convert.ToBoolean(engineStatus["IsRedundant"].ToString());
                                }

                                if (engineStatus["IsEmulator"] != null)
                                {
                                    _isEmulator = Convert.ToBoolean(engineStatus["IsEmulator"].ToString());
                                }

                                if (onNewCoreStatus != null)
                                    onNewCoreStatus(_coreId, _designName, Convert.ToUInt16(_isRedundant), Convert.ToUInt16(_isEmulator));
                            }

                            if (!_isLoggedIn)
                            {
                                if (_debug == 1 || _debug == 2)
                                    ErrorLog.Notice("QsysProcessor server ready, starting to send intialization strings.");

                                if (_password.Length > 0 && _username.Length > 0)
                                {
                                    _logonAttempts = 1;
                                    _commandQueue.Enqueue(JsonConvert.SerializeObject(new Logon() { Params = new LogonParams() { User = _username, Password = _password } }));
                                }
                                else
                                {
                                    _isLoggedIn = true;

                                    if (onIsLoggedIn != null)
                                    {
                                        onIsLoggedIn(_coreId, 1);
                                    }

                                    _waitForConnection.Reset(5000);
                                }
                            }
                        }
                        else if (returnString.Contains("error"))
                        {
                            var response = JObject.Parse(returnString);

                            if (_logonAttempts < _maxLogonAttempts)
                            {
                                JToken error = response["error"];

                                if (error["code"] != null)
                                {
                                    if (error["code"].ToString().Replace("\'", string.Empty) == "10")
                                    {
                                        _logonAttempts++;
                                        _commandQueue.Enqueue(JsonConvert.SerializeObject(new Logon() { Params = new LogonParams() { User = _username, Password = _password } }));
                                    }
                                }
                            }
                            else
                            {
                                if (_debug > 0)
                                {
                                    ErrorLog.Error("Error in QsysProcessor max logon attempts reached");
                                }
                            }
                        }
                        else if (returnString.Contains("\"id\":") && returnString.Contains("\"result\":true"))
                        {
                            var response = JObject.Parse(returnString);

                            if (response["id"] != null)
                            {
                                var responseData = JsonConvert.DeserializeObject<CustomResponseId>(response["id"].ToObject<string>());

                                if (responseData.Method == "Logon")
                                {
                                    _isLoggedIn = true;

                                    if (onIsLoggedIn != null)
                                    {
                                        onIsLoggedIn(_coreId, 1);
                                    }

                                    _waitForConnection.Reset(5000);
                                }

                                if (responseData.Caller != string.Empty)
                                {
                                    var components = Components.Where(x => x.Key.Name == responseData.Caller);

                                    foreach (var component in components)
                                    {
                                        if (component.Key != null)
                                            component.Value.Fire(new QsysInternalEventsArgs(responseData.Method, responseData.Value, responseData.Position, responseData.StringValue, null));
                                    }
                                }

                                responseData = null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_debug > 0)
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
                _commandQueue.Enqueue(data);
        }

        private void CommandQueueDequeue(object o)
        {
            try
            {
                if (!_commandQueue.IsEmpty)
                {
                    var data = _commandQueue.TryToDequeue();

                    if (data != null)
                    {
                        if (_debug == 2)
                        {
                            CrestronConsole.PrintLine("Command sent -->{0}<--", data);
                        }

                        if (!_externalConnection)
                            _client.SendCommand(data + "\x00");
                        //else if (SendingCommandEvent != null)
                        //    SendingCommandEvent(this, new SendingCommandEventArgs(data + "\x00"));
                        else if (onSendingCommand != null)
                        {
                            data = data + "\x00";
                            var xs = data.Chunk(200);

                            foreach (var x in xs)
                            {
                                if (_debug == 2)
                                    CrestronConsole.PrintLine("Command chuck sent externally length={0} -->{1}<--", x.Length, x);
                                onSendingCommand(_coreId, x);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (_debug > 0)
                    ErrorLog.Error("Error in QsysProcessor CommandQueueDequeue: {0}", e.Message);

                _commandQueue.Clear();
            }
        }
        #endregion

        /// <summary>
        /// Clean up of unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            _disposed = true;
            if (disposing)
            {
                _changeGroupCreated = false;
                _isLoggedIn = false;
                _isInitialized = false;

                _waitForConnection.Dispose();

                _client.Dispose();

                _heartbeatTimer.Dispose();

                _commandQueueTimer.Dispose();
                _commandQueue.Dispose();

                _rxData = null;
            }
        }
    }
}