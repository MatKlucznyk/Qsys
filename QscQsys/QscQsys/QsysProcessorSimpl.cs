using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysProcessorSimplInterface
    {
        private bool isRegistered;

        public delegate void IsRegistered(ushort value);
        public delegate void IsConnected(ushort value);
        public IsRegistered onIsRegistered { get; set; }
        public IsConnected onIsConnected { get; set; }

        public void Register(string id)
        {
            if (QsysProcessor.RegisterSimplClient(id))
            {
                QsysProcessor.SimplClients[id].OnNewEvent += new EventHandler<SimplEventArgs>(QsysProcessor_SimplEvent);

                isRegistered = true;
            }
        }

        public void Debug(ushort value)
        {
            QsysProcessor.Debug(value);
        }

        void QsysProcessor_SimplEvent(object sender, SimplEventArgs e)
        {
            switch (e.ID)
            {
                case eQscSimplEventIds.IsRegistered:
                    if (onIsRegistered != null)
                        onIsRegistered(e.IntData);
                    break;
                case eQscSimplEventIds.IsConnected:
                    if (onIsConnected != null)
                        onIsConnected(e.IntData);
                    break;
                default:
                    break;
            }
        }
    }
}