using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysPotsControllerSimpl
    {
        public delegate void OffHookEvent(ushort value);
        public delegate void RingingEvent(ushort value);
        public delegate void AutoAnswerEvent(ushort value);
        public delegate void DndEvent(ushort value);
        public delegate void DialStringEvent(SimplSharpString dialString);
        public delegate void CurrentlyCallingEvent(SimplSharpString currentlyCalling);
        public OffHookEvent onOffHookEvent { get; set; }
        public RingingEvent onRingingEvent { get; set; }
        public AutoAnswerEvent onAutoAnswerEvent { get; set; }
        public DndEvent onDndEvent { get; set; }
        public DialStringEvent onDialStringEvent { get; set; }
        public CurrentlyCallingEvent onCurrentlyCallingEvent { get; set; }

        private QsysPotsController pots;

        public void Initialize(string name)
        {
            pots = new QsysPotsController(name);
            pots.QsysPotsControllerEvent += new EventHandler<QsysEventsArgs>(softphone_QsysPotsControllerEvent);
        }

        void softphone_QsysPotsControllerEvent(object sender, QsysEventsArgs e)
        {
            switch (e.EventID)
            {
                case eQscEventIds.NewCommand:
                    break;
                case eQscEventIds.GainChange:
                    break;
                case eQscEventIds.MuteChange:
                    break;
                case eQscEventIds.NewMaxGain:
                    break;
                case eQscEventIds.NewMinGain:
                    break;
                case eQscEventIds.CameraStreamChange:
                    break;
                case eQscEventIds.PotsControllerOffHook:
                    if (onOffHookEvent != null)
                        onOffHookEvent(Convert.ToUInt16(e.IntegerValue));
                    break;
                case eQscEventIds.PotsControllerIsRinging:
                    if (onRingingEvent != null)
                        onRingingEvent(Convert.ToUInt16(e.IntegerValue));
                    break;
                case eQscEventIds.PotsControllerDialString:
                    if (onDialStringEvent != null)
                        onDialStringEvent(e.StringValue);
                    break;
                case eQscEventIds.PotsControllerCurrentlyCalling:
                    if (onCurrentlyCallingEvent != null)
                        onCurrentlyCallingEvent(e.StringValue);
                    break;
                case eQscEventIds.RouterInputSelected:
                    break;
                case eQscEventIds.PotsControllerAutoAnswerChange:
                    if (onAutoAnswerEvent != null)
                        onAutoAnswerEvent(Convert.ToUInt16(e.BooleanValue));
                    break;
                case eQscEventIds.PotsControllerDND_Change:
                    if(onDndEvent != null)
                        onDndEvent(Convert.ToUInt16(e.BooleanValue));
                    break;
                default:
                    break;
            }
        }

        public void Dial(string number)
        {
            pots.Dial(number);
        }

        public void DialWithoutString()
        {
            pots.Dial();
        }

        public void NumPad(string number)
        {
            pots.NumPad(number);
        }

        public void Disconnect()
        {
            pots.Disconnect();
        }

        public void Redial()
        {
            pots.Redial();
        }

        public void AutoAnswerToggle()
        {
            pots.AutoAnswerToggle();
        }

        public void DndToggle()
        {
            pots.DndToggle();
        }

    }
}