using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMeter
    {
        public delegate void MeterChange(ushort meterValue);
        public MeterChange onMeterChange { get; set; }

        private string cName;
        private string coreId;
        private bool registered;
        private int meterIndex;

        public event EventHandler<QsysEventsArgs> QsysMeterEvent;

        public void Initialize(string coreId, string name, int index)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

            cName = name;
            this.coreId = coreId;
            meterIndex = index;

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
                Component component = new Component() { Name = cName, Controls = new List<ControlName>() { new ControlName() { Name = string.Format("meter_{0}", meterIndex) } } };

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        private void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            QsysMeterEvent(this, new QsysEventsArgs(eQscEventIds.MeterUpdate, cName, Convert.ToBoolean(e.Value), Convert.ToInt16(e.Value), e.SValue, null));

            if (onMeterChange != null)
                onMeterChange(Convert.ToUInt16(e.Value));
        }
    }
}