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

        public void Initialize(string name, ushort index)
        {
            meter = new QsysMeter(name, index);
            meter.QsysMeterEvent += new EventHandler<QsysEventsArgs>(meter_QsysMeterEvent);
        }

        void meter_QsysMeterEvent(object sender, QsysEventsArgs e)
        {
            if (onMeterChange != null)
                onMeterChange(Convert.ToUInt16(e.IntegerValue));
        }


    }
}