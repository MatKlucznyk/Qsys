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
            if (!_registered)
            {
                _meterIndex = meterIndex;
                var component = new Component(true) { Name = _cName, Controls = new List<ControlName>() { new ControlName() { Name = string.Format("meter_{0}", _meterIndex) } } };
                base.Initialize(coreId, component);
            }
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            //QsysMeterEvent(this, new QsysEventsArgs(eQscEventIds.MeterUpdate, cName, Convert.ToBoolean(e.Value), Convert.ToInt16(e.Value), e.SValue, null));

            if (onMeterChange != null)
                onMeterChange(_cName, Convert.ToUInt16(e.Value));
        }
    }
}