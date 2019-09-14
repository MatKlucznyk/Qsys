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
        private bool autoAnswer;
        private bool dnd;
        private StringBuilder dialString = new StringBuilder();
        private string currentlyCalling;
        private string lastCalled;

        public event EventHandler<QsysEventsArgs> QsysPotsControllerEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public bool IsOffhook { get { return hookState; } }
        public bool IsRinging { get { return ringingState; } }
        public bool AutoAnswer { get { return autoAnswer; } }
        public bool DND { get { return dnd; } }
        public string DialString { get { return dialString.ToString(); } }
        public string CurrentlyCalling { get { return currentlyCalling; } }
        public string LastNumberCalled { get { return lastCalled; } }

        public QsysPotsController(string Name)
        {
            cName = Name;

            Component component = new Component();
            component.Name = Name;
            List<ControlName> names = new List<ControlName>();
            for (int i = 0; i < 4; i++)
            {
                names.Add(new ControlName());
            }
            names[0].Name = "call_offhook";
            names[1].Name = "call_ringing";
            names[2].Name = "call_autoanswer";
            names[3].Name = "call_dnd";


            component.Controls = names;

            if (QsysCore.RegisterComponent(component))
            {
                QsysCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(QsysPotsController_OnNewEvent);

                registered = true;
            }
        }

        void QsysPotsController_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            switch (e.Name)
            {
                case "call_offhook":
                    if (e.Data == 1)
                    {
                        hookState = true;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, cName, true, 1, "1"));
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, cName, true, currentlyCalling.Length, currentlyCalling));
                    }
                    else if (e.Data == 0)
                    {
                        hookState = false;
                        dialString.Remove(0, dialString.Length);
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, cName, false, 0, "0"));
                        lastCalled = currentlyCalling;
                        currentlyCalling = string.Empty;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, cName, false, currentlyCalling.Length, currentlyCalling));
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, dialString.ToString()));
                    }
                    break;
                case "call_ringing":
                    if (e.Data == 1)
                    {
                        ringingState = true;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, cName, true, 1, "1"));
                    }
                    else if (e.Data == 0)
                    {
                        ringingState = false;
                        QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, cName, false, 0, "0"));
                    }
                    break;
                case "call_autoanswer":
                    autoAnswer = Convert.ToBoolean(e.Data);
                    QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerAutoAnswerChange, cName, autoAnswer, Convert.ToInt16(e.Data), Convert.ToString(Convert.ToInt16(e.Data))));
                    break;
                case "call_dnd":
                    dnd = Convert.ToBoolean(e.Data);
                    QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDND_Change, cName, dnd, Convert.ToInt16(e.Data), Convert.ToString(Convert.ToInt16(e.Data))));
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

                QsysCore.Enqueue(JsonConvert.SerializeObject(pinPad));
            }

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString()));
        }

        public void NumPadDelete()
        {
            dialString.Remove(dialString.Length - 1, 1);

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString()));
        }

        public void NumPadClear()
        {
            dialString.Remove(0, dialString.Length);

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, true, dialString.Length, dialString.ToString()));
        }

        public void Dial()
        {
            currentlyCalling = dialString.ToString();
            dialString.Remove(0, dialString.Length);

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, string.Empty));

            ComponentChangeString dialNumber = new ComponentChangeString();
            dialNumber.Params = new ComponentChangeParamsString();

            dialNumber.Params.Name = cName;

            ComponentSetValueString dialStringSetValue = new ComponentSetValueString();
            dialStringSetValue.Name = "call_number";
            dialStringSetValue.Value = currentlyCalling;

            dialNumber.Params.Controls = new List<ComponentSetValueString>();
            dialNumber.Params.Controls.Add(dialStringSetValue);

            QsysCore.Enqueue(JsonConvert.SerializeObject(dialNumber));

            ComponentChange dial = new ComponentChange();
            dial.Params = new ComponentChangeParams();

            dial.Params.Name = cName;

            ComponentSetValue dialSetValue = new ComponentSetValue();
            dialSetValue.Name = "call_connect";
            dialSetValue.Value = 1;

            dial.Params.Controls = new List<ComponentSetValue>();
            dial.Params.Controls.Add(dialSetValue);

            QsysCore.Enqueue(JsonConvert.SerializeObject(dial));
        }

        public void Dial(string number)
        {
            currentlyCalling = number;

            QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, cName, false, 0, string.Empty));

            ComponentChangeString dialNumber = new ComponentChangeString();
            dialNumber.Params = new ComponentChangeParamsString();

            dialNumber.Params.Name = cName;

            ComponentSetValueString dialStringSetValue = new ComponentSetValueString();
            dialStringSetValue.Name = "call_number";
            dialStringSetValue.Value = currentlyCalling;

            dialNumber.Params.Controls = new List<ComponentSetValueString>();
            dialNumber.Params.Controls.Add(dialStringSetValue);

            QsysCore.Enqueue(JsonConvert.SerializeObject(dialNumber));

            ComponentChange dial = new ComponentChange();
            dial.Params = new ComponentChangeParams();

            dial.Params.Name = cName;

            ComponentSetValue dialSetValue = new ComponentSetValue();
            dialSetValue.Name = "call_connect";
            dialSetValue.Value = 1;

            dial.Params.Controls = new List<ComponentSetValue>();
            dial.Params.Controls.Add(dialSetValue);

            QsysCore.Enqueue(JsonConvert.SerializeObject(dial));
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

            QsysCore.Enqueue(JsonConvert.SerializeObject(disconnect));
        }

        public void Redial()
        {
            dialString = new StringBuilder();
            dialString.Append(lastCalled);
            Dial();
        }

        public void AutoAnswerToggle()
        {
            if (QsysCore.IsDebugMode)
            {
                CrestronConsole.PrintLine("Current AutoAnswer '{0}', new AutoAnswer '{1}'", autoAnswer, !autoAnswer);
            }

            ComponentChange aAnswer = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName } };

            ComponentSetValue aAsnwerValue = new ComponentSetValue() { Name = "call_autoanswer", Value = Convert.ToDouble(!autoAnswer) };

            if (QsysCore.IsDebugMode)
            {
                CrestronConsole.PrintLine("Sending AutoAnswer '{0}'", aAsnwerValue.Value);
            }

            aAnswer.Params.Controls = new List<ComponentSetValue>() { aAsnwerValue };

            QsysCore.Enqueue(JsonConvert.SerializeObject(aAnswer));
        }

        public void DndToggle()
        {
            ComponentChange d = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName } };

            ComponentSetValue dValue = new ComponentSetValue() { Name = "call_dnd", Value = Convert.ToDouble(!dnd) };

            d.Params.Controls = new List<ComponentSetValue>() { dValue };

            QsysCore.Enqueue(JsonConvert.SerializeObject(d));
        }
    }
}