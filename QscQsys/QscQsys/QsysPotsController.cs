using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Crestron.SimplSharp.SimplSharpExtensions;

namespace QscQsys
{
    public class QsysPotsController
    {
        public delegate void OffHookEvent(ushort value);
        public delegate void RingingEvent(ushort value);
        public delegate void DialingEvent(ushort value);
        public delegate void IncomingCallEvent(ushort value);
        public delegate void AutoAnswerEvent(ushort value);
        public delegate void DndEvent(ushort value);
        public delegate void DialStringEvent(SimplSharpString dialString);
        public delegate void CurrentlyCallingEvent(SimplSharpString currentlyCalling);
        public delegate void CurrentCallStatus(SimplSharpString callStatus);
        public delegate void RecentCallsEvent(SimplSharpString item1, SimplSharpString item2, SimplSharpString item3, SimplSharpString item4, SimplSharpString item5);
        public delegate void RecentCallListEvent(SimplSharpString xsig);
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

        private string cName;
        private string coreId;
        private bool registered;
        private bool hookState;
        private bool ringingState;
        private bool dialingState;
        private bool incomingCall;
        private bool autoAnswer;
        private bool dnd;
        private StringBuilder dialString = new StringBuilder();
        private string currentlyCalling;
        private string lastCalled;
        private string callStatus;
        private List<ListBoxChoice> recentCalls;

        //public event EventHandler<QsysEventsArgs> QsysPotsControllerEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public bool IsOffhook { get { return hookState; } }
        public bool IsRinging { get { return ringingState; } }
        public bool IsDialing { get { return dialingState; } }
        public bool IsIncomingCall { get { return incomingCall; } }
        public bool AutoAnswer { get { return autoAnswer; } }
        public bool DND { get { return dnd; } }
        public string DialString { get { return dialString.ToString(); } }
        public string CurrentlyCalling { get { return currentlyCalling; } }
        public string LastNumberCalled { get { return lastCalled; } }
        public string CallStatus { get { return callStatus; } }
        public List<ListBoxChoice> RecentCalls { get { return recentCalls; } }

        public void Initialize(string coreId, string Name)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

            cName = Name;
            this.coreId = coreId;
            recentCalls = new List<ListBoxChoice>();

            if (!registered)
                RegisterWithCore();
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                RegisterWithCore();
            }
        }

        private void RegisterWithCore()
        {
            if (QsysCoreManager.Cores.ContainsKey(coreId))
            {
                Component component = new Component()
                {
                    Name = cName,
                    Controls = new List<ControlName>(){new ControlName(){Name = "call_offhook"},
                new ControlName(){Name = "call_ringing"}, new ControlName(){Name = "call_autoanswer"}, new ControlName(){Name = "call_dnd"}, new ControlName(){Name = "call_Status"},
                new ControlName(){Name = "recent_calls"}}
                };

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            switch (e.Name)
            {
                case "call_offhook":
                    if (e.Value == 1)
                    {
                        hookState = true;
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, cName, true, 1, "1", null));
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, cName, true, currentlyCalling.Length, currentlyCalling, null));

                        if (onOffHookEvent != null)
                            onOffHookEvent(1);

                        if (onCurrentlyCallingEvent != null)
                            onCurrentlyCallingEvent(currentlyCalling);
                    }
                    else if (e.Value == 0)
                    {
                        hookState = false;
                        dialString.Remove(0, dialString.Length);
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, cName, false, 0, "0", null));

                        if (onOffHookEvent != null)
                            onOffHookEvent(0);


                        lastCalled = currentlyCalling;
                        currentlyCalling = string.Empty;
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, cName, false, currentlyCalling.Length, currentlyCalling, null));
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, dialString.ToString(), null));

                        if (onCurrentlyCallingEvent != null)
                            onCurrentlyCallingEvent(currentlyCalling);

                        if (onDialStringEvent != null)
                            onDialStringEvent(dialString.ToString());
                    }
                    break;
                case "call_ringing":
                    if (e.Value == 1)
                    {
                        ringingState = true;
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, cName, true, 1, "1", null));

                        if (onRingingEvent != null)
                            onRingingEvent(1);
                    }
                    else if (e.Value == 0)
                    {
                        ringingState = false;
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, cName, false, 0, "0", null));

                        if (onRingingEvent != null)
                            onRingingEvent(0);
                    }
                    break;
                case "call_autoanswer":
                    autoAnswer = Convert.ToBoolean(e.Value);
                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerAutoAnswerChange, cName, autoAnswer, Convert.ToInt16(e.Value), Convert.ToString(Convert.ToInt16(e.Value)), null));

                    if (onAutoAnswerEvent != null)
                        onAutoAnswerEvent(Convert.ToUInt16(e.Value));
                    
                    break;
                case "call_dnd":
                    dnd = Convert.ToBoolean(e.Value);
                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDND_Change, cName, dnd, Convert.ToInt16(e.Value), Convert.ToString(Convert.ToInt16(e.Value)), null));
                    
                    if(onDndEvent != null)
                        onDndEvent(Convert.ToUInt16(e.Value));

                    break;
                case "call_status":
                    callStatus = e.SValue;
                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCallStatusChange, cName, true, e.SValue.Length, e.SValue, null));

                    if (onCurrentCallStatusChange != null)
                        onCurrentCallStatusChange(e.SValue);

                    if (callStatus.Contains("Dialing") && dialingState == false)
                    {
                        dialingState = true;
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialing, cName, true, 1, "true", null));

                        if (onDialingEvent != null)
                            onDialingEvent(1);
                    }
                    else if (dialingState == true)
                    {
                        dialingState = false;
                        
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialing, cName, false, 0, "false", null));

                        if (onDialingEvent != null)
                            onDialingEvent(0);
                    }

                    if (callStatus.Contains("Incoming Call"))
                    {
                        incomingCall = true;
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIncomingCall, cName, true, 1, "true", null));

                        if (onIncomingCallEvent != null)
                            onIncomingCallEvent(1);
                    }
                    else if (incomingCall == true)
                    {
                        incomingCall = false;
                        //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIncomingCall, cName, false, 0, "false", null));

                        if (onIncomingCallEvent != null)
                            onIncomingCallEvent(0);
                    }
                    break;
                case "recent_calls":
                    recentCalls.Clear();
                    foreach (var choice in e.Choices)
                    {
                        var newChoice = JsonConvert.DeserializeObject<ListBoxChoice>(choice);
                        recentCalls.Add(newChoice);
                    }
                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerRecentCallsChange, cName, Convert.ToBoolean(recentCalls.Count), recentCalls.Count, recentCalls.Count.ToString(), recentCalls));

                    if (onRecentCallsEvent != null)
                    {
                        List<string> calls = new List<string>(){string.Empty, string.Empty, string.Empty, string.Empty, string.Empty};

                        for (int i = 0; i <= 4; i++)
                        {
                            if (recentCalls.Count > i)
                            {
                                calls[i] = recentCalls[i].Text;
                            }
                            else
                            {
                                break;
                            }
                        }
                        onRecentCallsEvent(calls[0], calls[1], calls[2], calls[3], calls[4]);
                    }
                    if (onRecentCallListEvent != null)
                    {
                        List<string> calls = new List<string>();

                        foreach (var call in calls)
                        {
                            var encodedBytes = XSig.GetBytes(calls.IndexOf(call), call);
                            onRecentCallListEvent(Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        public void NumPad(string number)
        {
            if (registered)
            {
                dialString.Append(number);

                if (hookState)
                {
                    ComponentChange pinPad = new ComponentChange()
                    {
                        Params = new ComponentChangeParams()
                        {
                            Name = cName,
                            Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("call_pinpad_{0}", number), Value = 1 } }
                        }
                    };

                    QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(pinPad, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }

                //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));

                if (onDialingEvent != null)
                    onDialStringEvent(dialString.ToString());
            }
        }

        public void NumString(string number)
        {
            if (registered)
            {
                if (!hookState)
                {
                    dialString.Append(number);
                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));

                    if (onDialingEvent != null)
                        onDialStringEvent(dialString.ToString());
                }
            }
        }

        public void NumPadDelete()
        {
            if (registered)
            {
                if (dialString.Length > 0)
                {
                    dialString.Remove(dialString.Length - 1, 1);

                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));

                    if (onDialingEvent != null)
                        onDialStringEvent(dialString.ToString());
                }
            }
        }

        public void NumPadClear()
        {
            if (registered)
            {
                if (dialString.Length > 0)
                {
                    dialString.Remove(0, dialString.Length);

                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));

                    if (onDialingEvent != null)
                        onDialStringEvent(dialString.ToString());
                }
            }
        }

        public void Dial()
        {
            if (registered)
            {
                currentlyCalling = dialString.ToString();
                dialString.Remove(0, dialString.Length);;

                DialNow();
            }
        }

        public void Dial(string number)
        {
            if (registered)
            {
                currentlyCalling = dialString.ToString() + number;
                dialString.Remove(0, dialString.Length);

                DialNow();
            }
        }


        private void DialNow()
        {
            //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, string.Empty, null));

            if (onDialingEvent != null)
                onDialStringEvent(string.Empty);

            ComponentChangeString dialNumber = new ComponentChangeString()
            {
                Params = new ComponentChangeParamsString()
                {
                    Name = cName,
                    Controls = new List<ComponentSetValueString>() { new ComponentSetValueString() { Name = "call_number", Value = currentlyCalling } }
                }
            };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(dialNumber, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            ComponentChange dial = new ComponentChange()
            {
                Params = new ComponentChangeParams()
                {
                    Name = cName,
                    Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_connect", Value = 1 } }
                }
            };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(dial, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Connect()
        {
            if (registered)
            {
                ComponentChange dial = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_connect", Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(dial, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Disconnect()
        {
            if (registered)
            {
                ComponentChange disconnect = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_disconnect", Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(disconnect, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Redial()
        {
            if (registered)
            {
                dialString = new StringBuilder();
                dialString.Append(lastCalled);
                Dial();
            }
        }

        public void AutoAnswerToggle()
        {
            if (registered)
            {
                ComponentChange aAnswer = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue> (){new ComponentSetValue(){Name = "call_autoanswer", Value = Convert.ToDouble(!autoAnswer)}}
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(aAnswer, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void DndToggle()
        {
            if (registered)
            {
                ComponentChange d = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_dnd", Value = Convert.ToDouble(!dnd) } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(d, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SelectRecentCall(int index)
        {
            if (registered)
            {
                if (recentCalls.Count >= index)
                {
                    dialString.Remove(0, dialString.Length);

                    var call = recentCalls[index - 1].Text;

                    if (call.Contains(' '))
                    {
                        call = call.Remove(call.IndexOf(' '), call.Length - call.IndexOf(' '));
                    }
                    dialString.Append(call);

                    //QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, Convert.ToBoolean(call.Length), call.Length, call, recentCalls));

                    if (onDialStringEvent != null)
                        onDialStringEvent(call);
                }
            }
        }
    }
}