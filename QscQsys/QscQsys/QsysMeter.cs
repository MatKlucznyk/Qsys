using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMeter : QsysComponent
    {
        public delegate void MeterChange(SimplSharpString cName, ushort meterValue);
        public MeterChange onMeterChange { get; set; }

        private int _meterIndex;

        public void Initialize(string coreId, string componentName, int meterIndex)
        {
            _meterIndex = meterIndex;
            var component = new Component(true) { Name = componentName, Controls = new List<ControlName>() { new ControlName() { Name = string.Format("meter_{0}", _meterIndex) } } };
            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (onMeterChange != null)
            {
                var value = QsysCoreManager.ScaleUp(e.Position);
                onMeterChange(_cName, Convert.ToUInt16(value));
            }
        }
    }
}