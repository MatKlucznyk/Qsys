using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMeterSimpl
    {
        private QsysMeter meter;

        public delegate void MeterChange(ushort meterValue);
        public MeterChange onMeterChange { get; set; }

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent, ushort _index)
        {
            this.meter = new QsysMeter((int)_coreID, _namedComponent.ToString(), (int)_index);
            this.meter.QsysMeterEvent += new EventHandler<QsysEventsArgs>(meter_QsysMeterEvent);
        }

        void meter_QsysMeterEvent(object _sender, QsysEventsArgs _e)
        {
            if (onMeterChange != null)
                onMeterChange(Convert.ToUInt16(_e.IntegerValue));
        }
    }
}