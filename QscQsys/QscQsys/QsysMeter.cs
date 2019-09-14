using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMeter
    {
        private string cName;
        private bool isComponent;
        private bool isRegistered;
        private int meterIndex;

        public event EventHandler<QsysEventsArgs> QsysMeterEvent;

        public QsysMeter(string name, int index)
        {
            cName = name;
            meterIndex = index;

            Component component = new Component() { Name = cName, Controls = new List<ControlName>() { new ControlName() { Name = string.Format("meter_{0}", meterIndex) } } };

            if (QsysCore.RegisterComponent(component))
            {
                QsysCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                isRegistered = true;
                isComponent = true;
            }
        }

        private void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            QsysMeterEvent(this, new QsysEventsArgs(eQscEventIds.MeterUpdate, cName, Convert.ToBoolean(e.Data), Convert.ToInt16(e.Data), e.SData));
        }
    }
}