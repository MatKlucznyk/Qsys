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
        //Core
        private QsysCore myCore;

        //Named Component
        private string componentName;
        public string ComponentName { get { return componentName; } }
        private bool registered;
        public bool IsRegistered { get { return registered; } }
        private bool isComponent;

        //Internal Vars
        private bool hookState;
        public bool IsOffhook { get { return hookState; } }
        private bool ringingState;
        public bool IsRinging { get { return ringingState; } }
        private bool autoAnswer;
        public bool AutoAnswer { get { return autoAnswer; } }
        private bool dnd;
        public bool DND { get { return dnd; } }
        private StringBuilder dialString = new StringBuilder();
        public string DialString { get { return dialString.ToString(); } }
        private string currentlyCalling;
        public string CurrentlyCalling { get { return currentlyCalling; } }
        private string lastCalled;
        public string LastNumberCalled { get { return lastCalled; } }

        //Events
        public event EventHandler<QsysEventsArgs> QsysPotsControllerEvent;


        public QsysPotsController(int _coreID, string _componentName)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);
            
            Component component = new Component();
            component.Name = this.componentName;
            List<ControlName> names = new List<ControlName>();
            for (int i = 0; i < 4; i++)
            {
                names.Add(new ControlName());
            }
            names[0].Name = "call_offhook";
            names[1].Name = "call_ringing";
            names[2].Name = "call_autoanswer";
            names[3].Name = "call_dnd";
            names[4].Name = "call_cid_name";
            names[5].Name = "call_cid_number";
            component.Controls = names;

            if (this.myCore.RegisterNamedComponent(component))
            {
                this.myCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);
                this.registered = true;
                this.isComponent = true;
            }
        }

        void Component_OnNewEvent(object _sender, QsysInternalEventsArgs _e)
        {
            switch (_e.changeResult.Name)
            {
                case "call_offhook":
                    if (_e.changeResult.Value == 1)
                    {
                        this.hookState = true;
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, this.componentName, true, 1, "1"));
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, this.componentName, true, this.currentlyCalling.Length, this.currentlyCalling));
                    }
                    else if (_e.changeResult.Value == 0)
                    {
                        this.hookState = false;
                        this.dialString.Remove(0, this.dialString.Length);
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, this.componentName, false, 0, "0"));
                        this.lastCalled = this.currentlyCalling;
                        this.currentlyCalling = string.Empty;
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCurrentlyCalling, this.componentName, false, this.currentlyCalling.Length, this.currentlyCalling));
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, this.componentName, false, 0, this.dialString.ToString()));
                    }
                    break;
                case "call_ringing":
                    if (_e.changeResult.Value == 1)
                    {
                        this.ringingState = true;
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, this.componentName, true, 1, "1"));
                    }
                    else if (_e.changeResult.Value == 0)
                    {
                        this.ringingState = false;
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIsRinging, this.componentName, false, 0, "0"));
                    }
                    break;
                case "call_autoanswer":
                    this.autoAnswer = Convert.ToBoolean(_e.changeResult.Value);
                    this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerAutoAnswerChange, this.componentName, this.autoAnswer, Convert.ToInt16(_e.changeResult.Value), Convert.ToString(Convert.ToInt16(_e.changeResult.Value))));
                    break;
                case "call_dnd":
                    this.dnd = Convert.ToBoolean(_e.changeResult.Value);
                    this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDND_Change, this.componentName, this.dnd, Convert.ToInt16(_e.changeResult.Value), Convert.ToString(Convert.ToInt16(_e.changeResult.Value))));
                    break;
                case "call_cid_name":

                    break;
                case "call_cid_number":

                    break;

                default:
                    break;
            }
        }

        public void NumPad(string _number)
        {
            this.dialString.Append(_number);

            if (this.hookState)
            {
                ComponentChange pinPad = new ComponentChange();
                pinPad.Params = new ComponentChangeParams();
                pinPad.Params.Name = this.componentName;

                ComponentSetValue pinPadSetValue = new ComponentSetValue();
                pinPadSetValue.Name = string.Format("call_pinpad_{0}", _number);
                pinPadSetValue.Value = 1;

                pinPad.Params.Controls = new List<ComponentSetValue>();
                pinPad.Params.Controls.Add(pinPadSetValue);

                this.myCore.Enqueue(JsonConvert.SerializeObject(pinPad));
            }

            this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, this.componentName, true, this.dialString.Length, this.dialString.ToString()));
        }

        public void NumPadDelete()
        {
            this.dialString.Remove(this.dialString.Length - 1, 1);

            this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, this.componentName, true, this.dialString.Length, this.dialString.ToString()));
        }

        public void NumPadClear()
        {
            this.dialString.Remove(0, this.dialString.Length);

            this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, this.componentName, true, this.dialString.Length, this.dialString.ToString()));
        }

        public void Dial()
        {
            this.currentlyCalling = this.dialString.ToString();
            this.dialString.Remove(0, this.dialString.Length);

            this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, this.componentName, false, 0, string.Empty));

            ComponentChangeString dialNumber = new ComponentChangeString();
            dialNumber.Params = new ComponentChangeParamsString();

            dialNumber.Params.Name = this.componentName;

            ComponentSetValueString dialStringSetValue = new ComponentSetValueString();
            dialStringSetValue.Name = "call_number";
            dialStringSetValue.Value = this.currentlyCalling;

            dialNumber.Params.Controls = new List<ComponentSetValueString>();
            dialNumber.Params.Controls.Add(dialStringSetValue);

            this.myCore.Enqueue(JsonConvert.SerializeObject(dialNumber));

            ComponentChange dial = new ComponentChange();
            dial.Params = new ComponentChangeParams();

            dial.Params.Name = this.componentName;

            ComponentSetValue dialSetValue = new ComponentSetValue();
            dialSetValue.Name = "call_connect";
            dialSetValue.Value = 1;

            dial.Params.Controls = new List<ComponentSetValue>();
            dial.Params.Controls.Add(dialSetValue);

            this.myCore.Enqueue(JsonConvert.SerializeObject(dial));
        }

        public void Dial(string number)
        {
            this.currentlyCalling = number;

            this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, this.componentName, false, 0, string.Empty));

            ComponentChangeString dialNumber = new ComponentChangeString();
            dialNumber.Params = new ComponentChangeParamsString();

            dialNumber.Params.Name = this.componentName;

            ComponentSetValueString dialStringSetValue = new ComponentSetValueString();
            dialStringSetValue.Name = "call_number";
            dialStringSetValue.Value = currentlyCalling;

            dialNumber.Params.Controls = new List<ComponentSetValueString>();
            dialNumber.Params.Controls.Add(dialStringSetValue);

            this.myCore.Enqueue(JsonConvert.SerializeObject(dialNumber));

            ComponentChange dial = new ComponentChange();
            dial.Params = new ComponentChangeParams();

            dial.Params.Name = this.componentName;

            ComponentSetValue dialSetValue = new ComponentSetValue();
            dialSetValue.Name = "call_connect";
            dialSetValue.Value = 1;

            dial.Params.Controls = new List<ComponentSetValue>();
            dial.Params.Controls.Add(dialSetValue);

            this.myCore.Enqueue(JsonConvert.SerializeObject(dial));
        }

        public void Disconnect()
        {
            ComponentChange disconnect = new ComponentChange();
            disconnect.Params = new ComponentChangeParams();

            disconnect.Params.Name = this.componentName;

            ComponentSetValue disconnectValue = new ComponentSetValue();
            disconnectValue.Name = "call_disconnect";
            disconnectValue.Value = 1;

            disconnect.Params.Controls = new List<ComponentSetValue>();
            disconnect.Params.Controls.Add(disconnectValue);

            this.myCore.Enqueue(JsonConvert.SerializeObject(disconnect));
        }

        public void Redial()
        {
            this.dialString = new StringBuilder();
            this.dialString.Append(this.lastCalled);
            this.Dial();
        }

        public void AutoAnswerToggle()
        {
            ComponentChange aAnswer = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue aAsnwerValue = new ComponentSetValue() { Name = "call_autoanswer", Value = Convert.ToDouble(!autoAnswer) };
            aAnswer.Params.Controls = new List<ComponentSetValue>() { aAsnwerValue };
            this.myCore.SendDebug(string.Format("Component {0} :: Setting AutoAnswer '{1}'", this.componentName, this.autoAnswer));
            this.myCore.Enqueue(JsonConvert.SerializeObject(aAnswer));
        }

        public void DndToggle()
        {
            ComponentChange d = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue dValue = new ComponentSetValue() { Name = "call_dnd", Value = Convert.ToDouble(!this.dnd) };
            d.Params.Controls = new List<ComponentSetValue>() { dValue };
            this.myCore.Enqueue(JsonConvert.SerializeObject(d));
        }
    }
}