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
        public OffHookEvent onOffHookEvent { get; set; }
        public delegate void RingingEvent(ushort value);
        public RingingEvent onRingingEvent { get; set; }
        public delegate void AutoAnswerEvent(ushort value);
        public AutoAnswerEvent onAutoAnswerEvent { get; set; }
        public delegate void DndEvent(ushort value);
        public DndEvent onDndEvent { get; set; }
        public delegate void DialStringEvent(SimplSharpString dialString);
        public DialStringEvent onDialStringEvent { get; set; }
        public delegate void CurrentlyCallingEvent(SimplSharpString currentlyCalling); 
        public CurrentlyCallingEvent onCurrentlyCallingEvent { get; set; }

        private QsysPotsController pots;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent)
        {
            this.pots = new QsysPotsController((int)_coreID,  _namedComponent.ToString());
            this.pots.QsysPotsControllerEvent += new EventHandler<QsysEventsArgs>(softphone_QsysPotsControllerEvent);
        }

        void softphone_QsysPotsControllerEvent(object _sender, QsysEventsArgs _e)
        {
            switch (_e.EventID)
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
                        onOffHookEvent(Convert.ToUInt16(_e.IntegerValue));
                    break;
                case eQscEventIds.PotsControllerIsRinging:
                    if (onRingingEvent != null)
                        onRingingEvent(Convert.ToUInt16(_e.IntegerValue));
                    break;
                case eQscEventIds.PotsControllerDialString:
                    if (onDialStringEvent != null)
                        onDialStringEvent(_e.StringValue);
                    break;
                case eQscEventIds.PotsControllerCurrentlyCalling:
                    if (onCurrentlyCallingEvent != null)
                        onCurrentlyCallingEvent(_e.StringValue);
                    break;
                case eQscEventIds.RouterInputSelected:
                    break;
                case eQscEventIds.PotsControllerAutoAnswerChange:
                    if (onAutoAnswerEvent != null)
                        onAutoAnswerEvent(Convert.ToUInt16(_e.BooleanValue));
                    break;
                case eQscEventIds.PotsControllerDND_Change:
                    if (onDndEvent != null)
                        onDndEvent(Convert.ToUInt16(_e.BooleanValue));
                    break;
                default:
                    break;
            }
        }

        public void Dial(string _number)
        {
            this.pots.Dial(_number);
        }

        public void DialWithoutString()
        {
            this.pots.Dial();
        }

        public void NumPad(string _number)
        {
            this.pots.NumPad(_number);
        }

        public void Disconnect()
        {
            this.pots.Disconnect();
        }

        public void Redial()
        {
            this.pots.Redial();
        }

        public void AutoAnswerToggle()
        {
            this.pots.AutoAnswerToggle();
        }

        public void DndToggle()
        {
            this.pots.DndToggle();
        }

    }
}