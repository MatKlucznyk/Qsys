using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysNv32hDecoderSimpl
    {
        private QsysNv32hDecoder decoder;

        public delegate void Nv32hDecoderInputChange(ushort input);
        public Nv32hDecoderInputChange newNv32hDecoderInputChange { get; set; }

        public void Initialize(string name)
        {
            decoder = new QsysNv32hDecoder(name);
            decoder.QsysNv32hDecoderEvent += new EventHandler<QsysEventsArgs>(router_QsysDecoderEvent);
        }

        private void router_QsysDecoderEvent(object sender, QsysEventsArgs e)
        {
            if (e.EventID == eQscEventIds.Nv32hDecoderInputChange)
            {
                if (newNv32hDecoderInputChange != null)
                    newNv32hDecoderInputChange(Convert.ToUInt16(e.IntegerValue));
            }
        }

        public void ChangeSource(ushort input)
        {
            decoder.ChangeInput(input);
        }
    }
}