using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using ExtensionMethods;

namespace QscQsys
{
    public class QsysPotsController : QsysComponent
    {
        public delegate void OffHookEvent(SimplSharpString cName, ushort value);
        public delegate void RingingEvent(SimplSharpString cName, ushort value);
        public delegate void DialingEvent(SimplSharpString cName, ushort value);
        public delegate void IncomingCallEvent(SimplSharpString cName, ushort value);
        public delegate void AutoAnswerEvent(SimplSharpString cName, ushort value);
        public delegate void DndEvent(SimplSharpString cName, ushort value);
        public delegate void DialStringEvent(SimplSharpString cName, SimplSharpString dialString);
        public delegate void CurrentlyCallingEvent(SimplSharpString cName, SimplSharpString currentlyCalling);
        public delegate void CurrentCallStatus(SimplSharpString cName, SimplSharpString callStatus);
        public delegate void RecentCallsEvent(SimplSharpString cName, SimplSharpString item1, SimplSharpString item2, SimplSharpString item3, SimplSharpString item4, SimplSharpString item5);
        public delegate void RecentCallListEvent(SimplSharpString cName, SimplSharpString xsig);
        public OffHookEvent onOffHookEvent { get; set; }
        public RingingEvent onRingingEvent { get; set; }
        public DialingEvent onDialingEvent { get; set; }
        public IncomingCallEvent onIncomingCallEvent { get; set; }
        public AutoAnswerEvent onAutoAnswerEvent { get; set; }
        public DndEvent onDndEvent { get; set; }
        public DialStringEvent onDialStringEvent { get; set; }
        public CurrentlyCallingEvent onCurrentlyCallingEvent { get; set; }
        public CurrentCallStatus onCurrentCallStatusChange { get; set; }
        public RecentCallsEvent onRecentCallsEvent { get; set; }
        public RecentCallListEvent onRecentCallListEvent { get; set; }

        private bool _hookState;
        private bool _ringingState;
        private bool _dialingState;
        private bool _incomingCall;
        private bool _autoAnswer;
        private bool _dnd;
        private StringBuilder _dialString = new StringBuilder();
        private string _currentlyCalling;
        private string _lastCalled;
        private string _callStatus;
        private List<ListBoxChoice> _recentCalls = new List<ListBoxChoice>();

        public bool IsOffhook { get { return _hookState; } }
        public bool IsRinging { get { return _ringingState; } }
        public bool IsDialing { get { return _dialingState; } }
        public bool IsIncomingCall { get { return _incomingCall; } }
        public bool AutoAnswer { get { return _autoAnswer; } }
        public bool DND { get { return _dnd; } }
        public string DialString { get { return _dialString.ToString(); } }
        public string CurrentlyCalling { get { return _currentlyCalling; } }
        public string LastNumberCalled { get { return _lastCalled; } }
        public string CallStatus { get { return _callStatus; } }
        public List<ListBoxChoice> RecentCalls { get { return _recentCalls; } }

        public void Initialize(string coreId, string componentName)
        {
            var component = new Component()
            {
                Name = componentName,
                Controls = new List<ControlName>(){new ControlName(){Name = "call_offhook"},
                new ControlName(){Name = "call_ringing"}, 
                new ControlName(){Name = "call_autoanswer"}, 
                new ControlName(){Name = "call_dnd"}, 
                new ControlName(){Name = "call_status"},
                new ControlName(){Name = "recent_calls"}}
            };
            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            switch (e.Name)
            {
                case "call_offhook":
                    if (e.Value == 1)
                    {
                        _hookState = true;

                        if (onOffHookEvent != null)
                            onOffHookEvent(_cName, 1);

                        if (onCurrentlyCallingEvent != null)
                            onCurrentlyCallingEvent(_cName, _currentlyCalling);
                    }
                    else if (e.Value == 0)
                    {
                        _hookState = false;
                        _dialString.Remove(0, _dialString.Length);

                        if (onOffHookEvent != null)
                            onOffHookEvent(_cName, 0);


                        _lastCalled = _currentlyCalling;
                        _currentlyCalling = string.Empty;
 
                        if (onCurrentlyCallingEvent != null)
                            onCurrentlyCallingEvent(_cName, _currentlyCalling);

                        if (onDialStringEvent != null)
                            onDialStringEvent(_cName, _dialString.ToString());
                    }
                    break;
                case "call_ringing":
                    if (e.Value == 1)
                    {
                        _ringingState = true;

                        if (onRingingEvent != null)
                            onRingingEvent(_cName, 1);
                    }
                    else if (e.Value == 0)
                    {
                        _ringingState = false;

                        if (onRingingEvent != null)
                            onRingingEvent(_cName, 0);
                    }
                    break;
                case "call_autoanswer":
                    _autoAnswer = Convert.ToBoolean(e.Value);

                    if (onAutoAnswerEvent != null)
                        onAutoAnswerEvent(_cName, Convert.ToUInt16(e.Value));
                    
                    break;
                case "call_dnd":
                    _dnd = Convert.ToBoolean(e.Value);

                    if(onDndEvent != null)
                        onDndEvent(_cName, Convert.ToUInt16(e.Value));

                    break;
                case "call_status":
                    _callStatus = e.SValue;

                    if (onCurrentCallStatusChange != null)
                        onCurrentCallStatusChange(_cName, e.SValue);

                    if (_callStatus.Contains("Dialing") && _dialingState == false)
                    {
                        _dialingState = true;

                        if (onDialingEvent != null)
                            onDialingEvent(_cName, 1);
                    }
                    else if (_dialingState == true)
                    {
                        _dialingState = false;
                        
                        if (onDialingEvent != null)
                            onDialingEvent(_cName, 0);
                    }

                    if (_callStatus.Contains("Incoming Call"))
                    {
                        _incomingCall = true;

                        if (onIncomingCallEvent != null)
                            onIncomingCallEvent(_cName, 1);
                    }
                    else if (_incomingCall == true)
                    {
                        _incomingCall = false;

                        if (onIncomingCallEvent != null)
                            onIncomingCallEvent(_cName, 0);
                    }
                    break;
                case "recent_calls":
                    _recentCalls.Clear();
                    foreach (var choice in e.Choices)
                    {
                        var newChoice = JsonConvert.DeserializeObject<ListBoxChoice>(choice);
                        _recentCalls.Add(newChoice);
                    }

                    if (onRecentCallsEvent != null)
                    {
                        List<string> calls = new List<string>(){string.Empty, string.Empty, string.Empty, string.Empty, string.Empty};

                        for (int i = 0; i <= 4; i++)
                        {
                            if (_recentCalls.Count > i)
                            {
                                calls[i] = _recentCalls[i].Text;
                            }
                            else
                            {
                                break;
                            }
                        }
                        onRecentCallsEvent(_cName, calls[0], calls[1], calls[2], calls[3], calls[4]);
                    }
                    if (onRecentCallListEvent != null)
                    {
                        List<string> calls = new List<string>();

                        foreach (var call in calls)
                        {
                            
                            var encodedBytes = XSig.GetBytes(calls.IndexOf(call), call);
                            onRecentCallListEvent(_cName, Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        public void NumPad(string number)
        {
            if (_registered)
            {
                _dialString.Append(number);

                if (_hookState)
                {
                    ComponentChange pinPad = new ComponentChange()
                    {
                        Params = new ComponentChangeParams()
                        {
                            Name = _cName,
                            Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("call_pinpad_{0}", number), Value = 1 } }
                        }
                    };

                    QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(pinPad, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }

                if (onDialingEvent != null)
                    onDialStringEvent(_cName, _dialString.ToString());
            }
        }

        public void NumString(string number)
        {
            if (_registered)
            {
                if (!_hookState)
                {
                    _dialString.Append(number);

                    if (onDialingEvent != null)
                        onDialStringEvent(_cName, _dialString.ToString());
                }
            }
        }

        public void NumPadDelete()
        {
            if (_registered)
            {
                if (_dialString.Length > 0)
                {
                    _dialString.Remove(_dialString.Length - 1, 1);

                    if (onDialingEvent != null)
                        onDialStringEvent(_cName, _dialString.ToString());
                }
            }
        }

        public void NumPadClear()
        {
            if (_registered)
            {
                if (_dialString.Length > 0)
                {
                    _dialString.Remove(0, _dialString.Length);

                    if (onDialingEvent != null)
                        onDialStringEvent(_cName, _dialString.ToString());
                }
            }
        }

        public void Dial()
        {
            if (_registered)
            {
                _currentlyCalling = _dialString.ToString();
                _dialString.Remove(0, _dialString.Length);;

                DialNow();
            }
        }

        public void Dial(string number)
        {
            if (_registered)
            {
                _currentlyCalling = _dialString.ToString() + number;
                _dialString.Remove(0, _dialString.Length);

                DialNow();
            }
        }

        private void DialNow()
        {
            if (_registered)
            {
                if (onDialingEvent != null)
                    onDialStringEvent(_cName, string.Empty);

                ComponentChangeString dialNumber = new ComponentChangeString()
                {
                    Params = new ComponentChangeParamsString()
                    {
                        Name = _cName,
                        Controls = new List<ComponentSetValueString>() { new ComponentSetValueString() { Name = "call_number", Value = _currentlyCalling } }
                    }
                };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(dialNumber, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                ComponentChange dial = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = _cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_connect", Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(dial, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Connect()
        {
            if (_registered)
            {
                ComponentChange dial = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = _cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_connect", Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(dial, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Disconnect()
        {
            if (_registered)
            {
                ComponentChange disconnect = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = _cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_disconnect", Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(disconnect, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Redial()
        {
            if (_registered)
            {
                _dialString = new StringBuilder();
                _dialString.Append(_lastCalled);
                Dial();
            }
        }

        public void AutoAnswerToggle()
        {
            if (_registered)
            {
                ComponentChange aAnswer = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = _cName,
                        Controls = new List<ComponentSetValue> (){new ComponentSetValue(){Name = "call_autoanswer", Value = Convert.ToDouble(!_autoAnswer)}}
                    }
                };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(aAnswer, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void DndToggle()
        {
            if (_registered)
            {
                ComponentChange d = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = _cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_dnd", Value = Convert.ToDouble(!_dnd) } }
                    }
                };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(d, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SelectRecentCall(int index)
        {
            if (_registered)
            {
                if (_recentCalls.Count >= index)
                {
                    _dialString.Remove(0, _dialString.Length);

                    var call = _recentCalls[index - 1].Text;

                    if (call.Contains(' '))
                    {
                        call = call.Remove(call.IndexOf(' '), call.Length - call.IndexOf(' '));
                    }
                    _dialString.Append(call);

                    if (onDialStringEvent != null)
                        onDialStringEvent(_cName, call);
                }
            }
        }
    }
}