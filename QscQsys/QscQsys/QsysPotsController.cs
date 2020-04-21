using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysPotsController
    {
        private string cName;
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

        public event EventHandler<QsysEventsArgs> QsysPotsControllerEvent;

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

        public QsysPotsController(string Name)
        {
            cName = Name;
            recentCalls = new List<ListBoxChoice>();

            Component component = new Component();
            component.Name = Name;
            List<ControlName> names = new List<ControlName>();
            for (int i = 0; i <= 5 ; i++)
            {
                names.Add(new ControlName());
            }
            names[0].Name = "call_offhook";
            names[1].Name = "call_ringing";
            names[2].Name = "call_autoanswer";
            names[3].Name = "call_dnd";
            names[4].Name = "call_status";
            names[5].Name = "recent_calls";


            component.Controls = names;

            if (QsysProcessor.RegisterComponent(component))
            {
                QsysProcessor.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(QsysPotsController_OnNewEvent);

                registered = true;
            }
        }

        void QsysPotsController_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            switch (e.Name)
            {
                case "call_offhook":
                    if (e.Value == 1)
                    {
                        hookState = true;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, cName, true, 1, "1", null));
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, cName, true, currentlyCalling.Length, currentlyCalling, null));
                    }
                    else if (e.Value == 0)
                    {
                        hookState = false;
                        dialString.Remove(0, dialString.Length);
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, cName, false, 0, "0", null));
                        lastCalled = currentlyCalling;
                        currentlyCalling = string.Empty;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, cName, false, currentlyCalling.Length, currentlyCalling, null));
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, dialString.ToString(), null));
                    }
                    break;
                case "call_ringing":
                    if (e.Value == 1)
                    {
                        ringingState = true;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, cName, true, 1, "1", null));
                    }
                    else if (e.Value == 0)
                    {
                        ringingState = false;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, cName, false, 0, "0", null));
                    }
                    break;
                case "call_autoanswer":
                    autoAnswer = Convert.ToBoolean(e.Value);
                    QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerAutoAnswerChange, cName, autoAnswer, Convert.ToInt16(e.Value), Convert.ToString(Convert.ToInt16(e.Value)), null));
                    break;
                case "call_dnd":
                    dnd = Convert.ToBoolean(e.Value);
                    QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDND_Change, cName, dnd, Convert.ToInt16(e.Value), Convert.ToString(Convert.ToInt16(e.Value)), null));
                    break;
                case "call_status":
                    callStatus = e.SValue;
                    QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCallStatusChange, cName, true, e.SValue.Length, e.SValue, null));

                    if (callStatus.Contains("Dialing") && dialingState == false)
                    {
                        dialingState = true;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialing, cName, true, 1, "true", null));
                    }
                    else if (dialingState == true)
                    {
                        dialingState = false;
                        
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialing, cName, false, 0, "false", null));
                    }

                    if (callStatus.Contains("Incoming Call"))
                    {
                        incomingCall = true;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIncomingCall, cName, true, 1, "true", null));
                    }
                    else if (incomingCall == true)
                    {
                        incomingCall = false;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIncomingCall, cName, false, 0, "false", null));
                    }
                    break;
                case "recent_calls":
                    recentCalls.Clear();
                    List<string> choices = e.Choices;
                    foreach (var choice in choices)
                    {
                        var newChoice = JsonConvert.DeserializeObject<ListBoxChoice>(choice);
                        recentCalls.Add(newChoice);
                    }
                    QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerRecentCallsChange, cName, Convert.ToBoolean(recentCalls.Count), recentCalls.Count, recentCalls.Count.ToString(), recentCalls));
                    break;
                default:
                    break;
            }
        }

        public void NumPad(string number)
        {
            dialString.Append(number);

            if (hookState)
            {
                ComponentChange pinPad = new ComponentChange();
                pinPad.Params = new ComponentChangeParams();

                pinPad.Params.Name = cName;

                ComponentSetValue pinPadSetValue = new ComponentSetValue();
                pinPadSetValue.Name = string.Format("call_pinpad_{0}", number);
                pinPadSetValue.Value = 1;

                pinPad.Params.Controls = new List<ComponentSetValue>();
                pinPad.Params.Controls.Add(pinPadSetValue);

                QsysProcessor.Enqueue(JsonConvert.SerializeObject(pinPad, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));
        }

        public void NumString(string number)
        {
            if (!hookState)
            {
                dialString.Append(number);
                QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));
            }
        }

        public void NumPadDelete()
        {
            if (dialString.Length > 0)
            {
                dialString.Remove(dialString.Length - 1, 1);

                QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));
            }
        }

        public void NumPadClear()
        {
            if (dialString.Length > 0)
            {
                dialString.Remove(0, dialString.Length);

                QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString(), null));
            }
        }

        public void Dial()
        {
            currentlyCalling = dialString.ToString();
            dialString.Remove(0, dialString.Length);

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, string.Empty, null));

            ComponentChangeString dialNumber = new ComponentChangeString();
            dialNumber.Params = new ComponentChangeParamsString();

            dialNumber.Params.Name = cName;

            ComponentSetValueString dialStringSetValue = new ComponentSetValueString();
            dialStringSetValue.Name = "call_number";
            dialStringSetValue.Value = currentlyCalling;

            dialNumber.Params.Controls = new List<ComponentSetValueString>();
            dialNumber.Params.Controls.Add(dialStringSetValue);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(dialNumber, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            ComponentChange dial = new ComponentChange();
            dial.Params = new ComponentChangeParams();

            dial.Params.Name = cName;

            ComponentSetValue dialSetValue = new ComponentSetValue();
            dialSetValue.Name = "call_connect";
            dialSetValue.Value = 1;

            dial.Params.Controls = new List<ComponentSetValue>();
            dial.Params.Controls.Add(dialSetValue);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(dial, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Dial(string number)
        {
            currentlyCalling = dialString.ToString() + number;

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, string.Empty, null));

            ComponentChangeString dialNumber = new ComponentChangeString();
            dialNumber.Params = new ComponentChangeParamsString();

            dialNumber.Params.Name = cName;

            ComponentSetValueString dialStringSetValue = new ComponentSetValueString();
            dialStringSetValue.Name = "call_number";
            dialStringSetValue.Value = currentlyCalling;

            dialNumber.Params.Controls = new List<ComponentSetValueString>();
            dialNumber.Params.Controls.Add(dialStringSetValue);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(dialNumber, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

            ComponentChange dial = new ComponentChange();
            dial.Params = new ComponentChangeParams();

            dial.Params.Name = cName;

            ComponentSetValue dialSetValue = new ComponentSetValue();
            dialSetValue.Name = "call_connect";
            dialSetValue.Value = 1;

            dial.Params.Controls = new List<ComponentSetValue>();
            dial.Params.Controls.Add(dialSetValue);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(dial, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Connect()
        {
            ComponentChange dial = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "call_connect", Value = 1 } } } };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(dial, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Disconnect()
        {
            ComponentChange disconnect = new ComponentChange();
            disconnect.Params = new ComponentChangeParams();

            disconnect.Params.Name = cName;

            ComponentSetValue disconnectValue = new ComponentSetValue();
            disconnectValue.Name = "call_disconnect";
            disconnectValue.Value = 1;

            disconnect.Params.Controls = new List<ComponentSetValue>();
            disconnect.Params.Controls.Add(disconnectValue);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(disconnect, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Redial()
        {
            dialString = new StringBuilder();
            dialString.Append(lastCalled);
            Dial();
        }

        public void AutoAnswerToggle()
        {
            ComponentChange aAnswer = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName } };

            ComponentSetValue aAsnwerValue = new ComponentSetValue() { Name = "call_autoanswer", Value = Convert.ToDouble(!autoAnswer) };

            aAnswer.Params.Controls = new List<ComponentSetValue>() { aAsnwerValue };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(aAnswer, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void DndToggle()
        {
            ComponentChange d = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName } };

            ComponentSetValue dValue = new ComponentSetValue() { Name = "call_dnd", Value = Convert.ToDouble(!dnd) };

            d.Params.Controls = new List<ComponentSetValue>() { dValue };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(d, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void SelectRecentCall(int index)
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

                QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, Convert.ToBoolean(call.Length), call.Length, call, recentCalls));
            }
        }
    }
}