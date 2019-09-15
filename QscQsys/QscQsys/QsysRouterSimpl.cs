using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysRouterSimpl
    {
        private QsysRouter router;

        public delegate void RouterInputChange(ushort input);
        public RouterInputChange newRouterInputChange { get; set; }

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent, ushort _output)
        {
            this.router = new QsysRouter((int)_coreID, _namedComponent.ToString(), (int)_output);
            this.router.QsysRouterEvent += new EventHandler<QsysEventsArgs>(router_QsysRouterEvent);
        }

        private void router_QsysRouterEvent(object _sender, QsysEventsArgs _e)
        {
            if (_e.EventID == eQscEventIds.RouterInputSelected)
            {
                if (newRouterInputChange != null)
                    this.newRouterInputChange(Convert.ToUInt16(_e.IntegerValue));
            }
        }

        public void SelectInput(ushort _input)
        {
            this.router.InputSelect(_input);
        }
    }
}