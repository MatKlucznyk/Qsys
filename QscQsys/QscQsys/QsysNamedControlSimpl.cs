using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysNamedControlSimpl
    {
        public delegate void NamedControlChange(ushort intData, SimplSharpString stringData);
        public NamedControlChange newNamedControlChange { get; set; }

        private QsysNamedControl namedControl;

        public void Initialize(string name, ushort isInteger)
        {
            namedControl = new QsysNamedControl(name);
            namedControl.IsInteger = isInteger;
            namedControl.QsysNamedControlEvent += new EventHandler<QsysEventsArgs>(namedControl_QsysNamedControlEvent);
        }

        void namedControl_QsysNamedControlEvent(object sender, QsysEventsArgs e)
        {
            if (newNamedControlChange != null)
                newNamedControlChange(Convert.ToUInt16(e.IntegerValue), e.StringValue);
        }

        public void SetInteger(ushort value)
        {

            namedControl.SetInteger(value);
        }

        public void SetString(string value)
        {
            namedControl.SetString(value);
        }

        public void SetBoolean(ushort value)
        {
            namedControl.SetBoolean(value);
        }
    }
}