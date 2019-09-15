using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysNv32hDecoderSimpl
    {
        public delegate void Nv32hDecoderInputChange(ushort input);
        public Nv32hDecoderInputChange newNv32hDecoderInputChange { get; set; }

        private QsysNv32hDecoder decoder;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent)
        {
            this.decoder = new QsysNv32hDecoder((int)_coreID, _namedComponent.ToString());
            this.decoder.QsysNv32hDecoderEvent += new EventHandler<QsysEventsArgs>(router_QsysDecoderEvent);
        }

        private void router_QsysDecoderEvent(object sender, QsysEventsArgs e)
        {
            if (e.EventID == eQscEventIds.Nv32hDecoderInputChange)
            {
                if (this.newNv32hDecoderInputChange != null)
                    this.newNv32hDecoderInputChange(Convert.ToUInt16(e.IntegerValue));
            }
        }

        public void ChangeSource(ushort input)
        {
            this.decoder.ChangeInput(input);
        }
    }
}