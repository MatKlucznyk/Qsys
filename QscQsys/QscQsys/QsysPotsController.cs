using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        private string progressState;
        private bool incomingCall;
        public bool IncomingCall { get { return incomingCall; } }
        private bool autoAnswer;
        public bool AutoAnswer { get { return autoAnswer; } }
        private bool dnd;
        public bool DND { get { return dnd; } }
        private string dialString;
        public string DialString { get { return dialString.ToString(); } }
        private string cidName = "";
        public string CidName { get { return cidName; } }
        private string cidNumber = "";
        public string CidNumber { get { return cidNumber; } }


        //Events
        public event EventHandler<QsysEventsArgs> QsysPotsControllerEvent;


        public QsysPotsController(int _coreID, string _componentName)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);
            
            Component component = new Component();
            component.Name = this.componentName;
            List<ControlName> names = new List<ControlName>();
            names.Add(new ControlName { Name = "call_offhook" });
            names.Add(new ControlName { Name = "call_ringing" });
            names.Add(new ControlName { Name = "call_autoanswer" });
            names.Add(new ControlName { Name = "call_dnd" });
            names.Add(new ControlName { Name = "call_number" });
            names.Add(new ControlName { Name = "call_cid_name" });
            names.Add(new ControlName { Name = "call_cid_number" });
            names.Add(new ControlName { Name = "call_status" });
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
                    }
                    else if (_e.changeResult.Value == 0)
                    {
                        this.hookState = false;
                        this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerOffHook, this.componentName, false, 0, "0"));
                    }
                    break;
                case "call_ringing":
                    if (_e.changeResult.Value == 1)
                        this.ringingState = true;
                    else
                        this.ringingState = false;
                    CallStatusLogic();
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
                    this.cidName = _e.changeResult.String;
                    this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCID, this.componentName, false, 0, this.cidName));
                    break;
                case "call_cid_number":
                    this.cidNumber = _e.changeResult.String;
                    this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerCID, this.componentName, false, 1, this.cidNumber));
                    break;
                case "call_number":
                    this.dialString = _e.changeResult.String;
                    this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerDialString, this.componentName, false, 0, this.dialString));
                    break;
                case "call_status":
                    this.progressState = _e.changeResult.String;
                    CallStatusLogic();
                    break;


                default:
                    break;
            }
        }

        private void CallStatusLogic()
        {
            if (this.ringingState && progressState.ToLower().Contains("incoming"))
            {
                incomingCall = true;
                this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIncomingCall, this.componentName, incomingCall, 1, "1"));
            }
            else
            {
                incomingCall = false;
                this.QsysPotsControllerEvent(this, new QsysEventsArgs(eQscEventIds.PotsControllerIncomingCall, this.componentName, incomingCall, 0, "0"));
            }
        }

        public void Connect()
        {
            ComponentChange cc = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue ccv = new ComponentSetValue() { Name = "call_connect", Value = 1 };
            cc.Params.Controls = new List<ComponentSetValue>() { ccv };
            this.myCore.SendDebug(string.Format("Component {0} :: Connect", this.componentName));
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }

        public void Disconnect()
        {
            ComponentChange cc = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue ccv = new ComponentSetValue() { Name = "call_disconnect", Value = 1 };
            cc.Params.Controls = new List<ComponentSetValue>() { ccv };
            this.myCore.SendDebug(string.Format("Component {0} :: Disconnect", this.componentName));
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }

        public void SetDialString(string _dialString)
        {
            ComponentChangeString cc = new ComponentChangeString() { Params = new ComponentChangeParamsString() { Name = this.componentName } };
            ComponentSetValueString ccv = new ComponentSetValueString { Name = "call_number", Value = _dialString };
            cc.Params.Controls = new List<ComponentSetValueString>() { ccv };
            this.myCore.SendDebug(string.Format("Component {0} :: SetDialString - {1}", this.componentName));
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }

        public void NumPadKey(string _key)
        {
            ComponentChange cc = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue ccv = new ComponentSetValue() { Name = string.Format("call_pinpad_{0}", _key), Value = 1 };
            cc.Params.Controls = new List<ComponentSetValue>() { ccv };
            this.myCore.SendDebug(string.Format("Component {0} :: NumPadKey {0}", this.componentName, _key));
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }

        public void NumPadBackspace()
        {
            ComponentChange cc = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue ccv = new ComponentSetValue() { Name = "call_backspace", Value = 1 };
            cc.Params.Controls = new List<ComponentSetValue>() { ccv };
            this.myCore.SendDebug(string.Format("Component {0} :: NumPadBackspace", this.componentName));
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }

        public void NumPadClear()
        {
            ComponentChange cc = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue ccv = new ComponentSetValue() { Name = "call_clear", Value = 1 };
            cc.Params.Controls = new List<ComponentSetValue>() { ccv };
            this.myCore.SendDebug(string.Format("Component {0} :: NumPadClear", this.componentName));
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }

        public void AutoAnswerToggle()
        {
            ComponentChange cc = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue ccv = new ComponentSetValue() { Name = "call_autoanswer", Value = Convert.ToDouble(!autoAnswer) };
            cc.Params.Controls = new List<ComponentSetValue>() { ccv };
            this.myCore.SendDebug(string.Format("Component {0} :: Setting AutoAnswer '{1}'", this.componentName, this.autoAnswer));
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }

        public void DndToggle()
        {
            ComponentChange cc = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName } };
            ComponentSetValue ccv = new ComponentSetValue() { Name = "call_dnd", Value = Convert.ToDouble(!this.dnd) };
            cc.Params.Controls = new List<ComponentSetValue>() { ccv };
            this.myCore.Enqueue(JsonConvert.SerializeObject(cc));
        }
    }
}