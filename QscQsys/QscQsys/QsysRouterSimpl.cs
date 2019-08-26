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

        public void Initialize(string name, ushort size)
        {
            router = new QsysRouter(name, size);
            router.QsysRouterEvent += new EventHandler<QsysEventsArgs>(router_QsysRouterEvent);
        }

        private void router_QsysRouterEvent(object sender, QsysEventsArgs e)
        {
            if (e.EventID == eQscEventIds.RouterInputSelected)
            {
                if (newRouterInputChange != null)
                    newRouterInputChange(Convert.ToUInt16(e.IntegerValue));
            }
        }

        public void SelectInput(ushort input)
        {
            router.InputSelect(input);
        }
    }
}