using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMeter
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
        private int meterIndex;

        public event EventHandler<QsysEventsArgs> QsysMeterEvent;

        public QsysMeter(int _coreID, string _componentName, int _index)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);
            this.meterIndex = _index;

            Component component = new Component() { Name = this.componentName, Controls = new List<ControlName>() { new ControlName() { Name = string.Format("meter_{0}", this.meterIndex) } } };

            if (this.myCore.RegisterNamedComponent(component))
            {
                this.myCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);
                this.registered = true;
                this.isComponent = true;
            }
        }

        private void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            QsysMeterEvent(this, new QsysEventsArgs(eQscEventIds.MeterUpdate, this.componentName, Convert.ToBoolean(e.Data), Convert.ToInt16(e.Data), e.SData));
        }
    }
}