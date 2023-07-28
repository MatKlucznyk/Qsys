using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using ExtensionMethods;
using Newtonsoft.Json;
using QscQsys.Intermediaries;

namespace QscQsys.NamedComponents
{
    public class QsysPotsController : AbstractQsysComponent
    {
        private const string CONTROL_CALL_OFFHOOK = "call_offhook";
        private const string CONTROL_CALL_RINGING = "call_ringing";
        private const string CONTROL_CALL_AUTOANSWER = "call_autoanswer";
        private const string CONTROL_CALL_DND = "call_dnd";
        private const string CONTROL_CALL_STATUS = "call_status";
        private const string CONTROL_RECENT_CALLS = "recent_calls";
        private const string CONTROL_CALL_NUMBER = "call_number";
        private const string CONTROL_CALL_CONNECT = "call_connect";
        private const string CONTROL_CALL_DISCONNECT = "call_disconnect";

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
        private readonly StringBuilder _dialString;
        private string _currentlyCalling;
        private string _lastCalled;
        private string _callStatus;
        private readonly List<ListBoxChoice> _recentCalls;

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

        public QsysPotsController()
        {
            _dialString = new StringBuilder();
            _recentCalls = new List<ListBoxChoice>();
        }

        public void Initialize(string coreId, string componentName)
        {
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
                return;

            component.LazyLoadComponentControl(CONTROL_CALL_OFFHOOK);
            component.LazyLoadComponentControl(CONTROL_CALL_RINGING);
            component.LazyLoadComponentControl(CONTROL_CALL_AUTOANSWER);
            component.LazyLoadComponentControl(CONTROL_CALL_DND);
            component.LazyLoadComponentControl(CONTROL_CALL_STATUS);
            component.LazyLoadComponentControl(CONTROL_RECENT_CALLS);
        }

        protected override void ComponentOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            base.ComponentOnFeedbackReceived(sender, args);

            switch (args.Name)
            {
                case CONTROL_CALL_OFFHOOK:
                    if (Math.Abs(args.Value - 1) < QsysCore.TOLERANCE)
                    {
                        _hookState = true;

                        if (onOffHookEvent != null)
                            onOffHookEvent(ComponentName, 1);

                        if (onCurrentlyCallingEvent != null)
                            onCurrentlyCallingEvent(ComponentName, _currentlyCalling);
                    }
                    else if (Math.Abs(args.Value) < QsysCore.TOLERANCE)
                    {
                        _hookState = false;
                        _dialString.Remove(0, _dialString.Length);

                        if (onOffHookEvent != null)
                            onOffHookEvent(ComponentName, 0);


                        _lastCalled = _currentlyCalling;
                        _currentlyCalling = string.Empty;

                        if (onCurrentlyCallingEvent != null)
                            onCurrentlyCallingEvent(ComponentName, _currentlyCalling);

                        if (onDialStringEvent != null)
                            onDialStringEvent(ComponentName, _dialString.ToString());
                    }
                    break;
                case CONTROL_CALL_RINGING:
                    if (Math.Abs(args.Value - 1) < QsysCore.TOLERANCE)
                    {
                        _ringingState = true;

                        if (onRingingEvent != null)
                            onRingingEvent(ComponentName, 1);
                    }
                    else if (Math.Abs(args.Value) < QsysCore.TOLERANCE)
                    {
                        _ringingState = false;

                        if (onRingingEvent != null)
                            onRingingEvent(ComponentName, 0);
                    }
                    break;
                case CONTROL_CALL_AUTOANSWER:
                    _autoAnswer = Convert.ToBoolean(args.Value);

                    if (onAutoAnswerEvent != null)
                        onAutoAnswerEvent(ComponentName, Convert.ToUInt16(args.Value));

                    break;
                case CONTROL_CALL_DND:
                    _dnd = Convert.ToBoolean(args.Value);

                    if (onDndEvent != null)
                        onDndEvent(ComponentName, Convert.ToUInt16(args.Value));

                    break;
                case CONTROL_CALL_STATUS:
                    if (args.StringValue.Length > 0)
                    {
                        _callStatus = args.StringValue;

                        if (onCurrentCallStatusChange != null)
                            onCurrentCallStatusChange(ComponentName, args.StringValue);

                        if (_callStatus.Contains("Dialing") && _dialingState == false)
                        {
                            _dialingState = true;

                            if (onDialingEvent != null)
                                onDialingEvent(ComponentName, 1);
                        }
                        else if (_dialingState)
                        {
                            _dialingState = false;

                            if (onDialingEvent != null)
                                onDialingEvent(ComponentName, 0);
                        }

                        if (_callStatus.Contains("Incoming Call"))
                        {
                            _incomingCall = true;

                            if (onIncomingCallEvent != null)
                                onIncomingCallEvent(ComponentName, 1);
                        }
                        else if (_incomingCall)
                        {
                            _incomingCall = false;

                            if (onIncomingCallEvent != null)
                                onIncomingCallEvent(ComponentName, 0);
                        }
                    }
                    break;
                case CONTROL_RECENT_CALLS:
                    _recentCalls.Clear();
                    foreach (var choice in args.Choices)
                    {
                        var newChoice = JsonConvert.DeserializeObject<ListBoxChoice>(choice);
                        _recentCalls.Add(newChoice);
                    }

                    if (onRecentCallsEvent != null)
                    {
                        List<string> calls = new List<string> { string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };

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
                        onRecentCallsEvent(ComponentName, calls[0], calls[1], calls[2], calls[3], calls[4]);
                    }
                    if (onRecentCallListEvent != null)
                    {
                        List<string> calls = new List<string>();

                        foreach (var call in calls)
                        {

                            var encodedBytes = XSig.GetBytes(calls.IndexOf(call), call);
                            onRecentCallListEvent(ComponentName, Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                        }
                    }

                    break;
            }
        }

        public void NumPad(string number)
        {
            if (Component == null)
                return;

            _dialString.Append(number);

            if (_hookState)
                SendComponentChangeDoubleValue(string.Format("call_pinpad_{0}", number), 1);

            if (onDialingEvent != null)
                onDialStringEvent(ComponentName, _dialString.ToString());
        }

        public void NumString(string number)
        {
            if (Component == null)
                return;

            if (_hookState)
                return;

            _dialString.Append(number);

            if (onDialingEvent != null)
                onDialStringEvent(ComponentName, _dialString.ToString());
        }

        public void NumPadDelete()
        {
            if (Component == null)
                return;

            if (_dialString.Length == 0)
                return;

            _dialString.Remove(_dialString.Length - 1, 1);

            if (onDialingEvent != null)
                onDialStringEvent(ComponentName, _dialString.ToString());
        }

        public void NumPadClear()
        {
            if (Component == null)
                return;

            if (_dialString.Length == 0)
                return;

            _dialString.Remove(0, _dialString.Length);

            if (onDialingEvent != null)
                onDialStringEvent(ComponentName, _dialString.ToString());
        }

        public void Dial()
        {
            if (Component == null)
                return;

            _currentlyCalling = _dialString.ToString();
            _dialString.Remove(0, _dialString.Length);

            DialNow();
        }

        public void Dial(string number)
        {
            if (Component == null)
                return;

            _currentlyCalling = _dialString.ToString() + number;
            _dialString.Remove(0, _dialString.Length);

            DialNow();
        }

        private void DialNow()
        {
            if (Component == null)
                return;

            if (onDialingEvent != null)
                onDialStringEvent(ComponentName, string.Empty);

            SendComponentChangeStringValue(CONTROL_CALL_NUMBER, _currentlyCalling);

            Connect();
        }

        public void Connect()
        {
            if (Component == null)
                return;
            
            SendComponentChangeDoubleValue(CONTROL_CALL_CONNECT, 1);
        }

        public void Disconnect()
        {
            if (Component == null)
                return;

            SendComponentChangeDoubleValue(CONTROL_CALL_DISCONNECT, 1);
        }

        public void Redial()
        {
            if (Component == null)
                return;

            _dialString.Remove(0, _dialString.Length);
            _dialString.Append(_lastCalled);
            Dial();
        }

        public void AutoAnswerToggle()
        {
            if (Component == null)
                return;
            
            SendComponentChangeDoubleValue(CONTROL_CALL_AUTOANSWER, Convert.ToDouble(!_autoAnswer));
        }

        public void DndToggle()
        {
            if (Component == null)
                return;
            
            SendComponentChangeDoubleValue(CONTROL_CALL_DND, Convert.ToDouble(!_dnd));
        }

        public void SelectRecentCall(int index)
        {
            if (Component == null)
                return;

            if (_recentCalls.Count < index)
                return;

            _dialString.Remove(0, _dialString.Length);

            var call = _recentCalls[index - 1].Text;

            if (call.Contains(' '))
                call = call.Remove(call.IndexOf(' '), call.Length - call.IndexOf(' '));

            _dialString.Append(call);

            if (onDialStringEvent != null)
                onDialStringEvent(ComponentName, call);
        }
    }
}