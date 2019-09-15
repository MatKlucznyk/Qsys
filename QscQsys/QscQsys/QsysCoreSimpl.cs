using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysCoreSimpl
    {
        private bool isRegistered;
        public delegate void IsRegistered(ushort value);
        public delegate void IsConnected(ushort value);
        public delegate void CoreState(ushort value);
        public delegate void Platform(SimplSharpString value);
        public delegate void DesignName(SimplSharpString value);
        public delegate void DesignCode(SimplSharpString value);
        public delegate void IsRedundant(ushort value);
        public delegate void IsEmulator(ushort value);
        public delegate void StatusCode(ushort value);
        public delegate void StatusString(SimplSharpString value);
        public IsRegistered onIsRegistered { get; set; }
        public IsConnected onIsConnected { get; set; }
        public CoreState onCoreState { get; set; }
        public Platform onPlatform { get; set; }
        public DesignName onDesignName { get; set; }
        public DesignCode onDesignCode { get; set; }
        public IsRedundant onIsRedundant { get; set; }
        public IsEmulator onIsEmulator { get; set; }
        public StatusCode onStatusCode { get; set; }
        public StatusString onStatusString { get; set; }
        private bool debug;


        QsysCore core;

        public void Initialize(ushort _coreID, SimplSharpString _host, ushort _port, SimplSharpString _user, SimplSharpString _pass)
        {
            this.core  = QsysMain.AddOrGetCoreObject(_coreID);
            if (core.Initialize((int)_coreID, _host.ToString(), _port, _user.ToString(), _pass.ToString()))
            {
                core.setDebug(debug);
                core.RegisterSimplClient(Convert.ToString(_coreID));
                core.SimplClients[Convert.ToString(_coreID)].OnNewEvent += new EventHandler<SimplEventArgs>(QsysProcessor_SimplEvent);
                this.isRegistered = true;
            }
            else
            {
                //TODO: Remove all changegroups and re-add all, re-sync
            }
        }

        public void setDebug(ushort _value)
        {
            this.debug = Convert.ToBoolean(_value);
            if (isRegistered)
                this.core.setDebug(debug);
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
                case eQscSimplEventIds.CoreState:
                    if (onCoreState != null)
                        onCoreState(e.IntData);
                    break;
                case eQscSimplEventIds.Platform:
                    if(onPlatform != null)
                        onPlatform(e.StringData);
                    break;
                case eQscSimplEventIds.DesignName:
                    if (onDesignName != null)
                        onDesignName(e.StringData);
                    break;
                case eQscSimplEventIds.DesignCode:
                    if (onDesignCode != null)
                        onDesignCode(e.StringData);
                    break;
                case eQscSimplEventIds.IsRedundant:
                    if (onIsRedundant != null)
                        onIsRedundant(e.IntData);
                    break;
                case eQscSimplEventIds.IsEmulator:
                    if (onIsEmulator != null)
                        onIsEmulator(e.IntData);
                    break;
                case eQscSimplEventIds.StatusCode:
                    if (onStatusCode != null)
                        onStatusCode(e.IntData);
                    break;
                case eQscSimplEventIds.StatusString:
                    if (onStatusString != null)
                        onStatusString(e.StringData);
                    break;
                default:
                    break;
            }
        }
    }
}