using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysRouter
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
        private int myOutput;
        public int CurrentSelectedInput { set; get; }

        //Events
        public event EventHandler<QsysEventsArgs> QsysRouterEvent;
        

        public QsysRouter(int _coreID, string _componentName, int _output)
        {
            this.componentName = _componentName;
            this.myOutput = _output;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);

            Component component = new Component();
            component.Name = this.componentName;
            List<ControlName> names = new List<ControlName>();
            names.Add(new ControlName());
            names[0].Name = string.Format("select_{0}", this.myOutput);

            component.Controls = names;

            if (this.myCore.RegisterNamedComponent(component))
            {
                this.myCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);
                this.registered = true;
                this.isComponent = true;
            }
        }

        public void InputSelect(int _input)
        {
            ComponentChange newInputSelectedChange = new ComponentChange();
            newInputSelectedChange.Params = new ComponentChangeParams();
            newInputSelectedChange.Params.Name = this.componentName;

            ComponentSetValue inputSelected = new ComponentSetValue();
            inputSelected.Name = string.Format("select_{0}", this.myOutput);
            inputSelected.Value = _input;
            newInputSelectedChange.Params.Controls = new List<ComponentSetValue>();
            newInputSelectedChange.Params.Controls.Add(inputSelected);

            this.myCore.Enqueue(JsonConvert.SerializeObject(newInputSelectedChange));
        }

        private void Component_OnNewEvent(object _sender, QsysInternalEventsArgs _e)
        {
            if (_e.changeResult.Name.Contains(string.Format("select_{0}", this.myOutput)))
            {
                this.CurrentSelectedInput = Convert.ToInt16(_e.changeResult.Value);
                QsysRouterEvent(this, new QsysEventsArgs(eQscEventIds.RouterInputSelected, this.componentName, Convert.ToBoolean(_e.changeResult.Value), Convert.ToInt16(_e.changeResult.Value), _e.changeResult.String));
            }
        }
    }
}