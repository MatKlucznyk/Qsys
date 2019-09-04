using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysNamedControlSimpl
    {
        public delegate void ValueChange(ushort value, short valueDb);
        public ValueChange newValueChange { get; set; }
        public delegate void StateChange(ushort value);
        public StateChange newStateChange { get; set; }
        public delegate void StringChange(SimplSharpString value);
        public StringChange newStringChange { get; set; }

        private QsysNamedControl cntrl;

        public void Initialize(string name, uint type)
        {
            eControlType t = (eControlType)Enum.Parse(typeof(eControlType), Convert.ToString(type), true);
            cntrl = new QsysNamedControl(name, t);
            cntrl.QsysNamedControlEvent += new EventHandler<QsysEventsArgs>(namedControl_QsysNamedControlEvent);
        }

        private void namedControl_QsysNamedControlEvent(object sender, QsysEventsArgs e)
        {
            switch (e.EventID)
            {
                case eQscEventIds.NamedControl:
                    switch (cntrl.ControlType)
                    {
                        case eControlType.isFloat:
                        case eControlType.isInteger:
                            if (newValueChange != null && newStringChange != null)
                            {
                                newValueChange((ushort)e.IntegerValue, (short)e.IntegerValue);
                                newStringChange(e.StringValue);
                            }
                            break;
                        case eControlType.isMomentary:
                        case eControlType.isToggle:
                            if (newStateChange != null && newStringChange != null && newValueChange != null)
                            {
                                newValueChange((ushort)e.IntegerValue, (short)e.IntegerValue);
                                newStateChange(Convert.ToUInt16(e.BooleanValue));
                                newStringChange(e.StringValue);
                            }
                            break;
                        case eControlType.isTrigger:
                            if (newStateChange != null)
                                newStateChange(Convert.ToUInt16(e.BooleanValue));
                            break;
                        case eControlType.isString:
                            if (newStringChange != null)
                                newStringChange(e.StringValue);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetValue(ushort value)
        {
            //fader.volume(value);
        }

        public void SetValueDB(short value)
        {
            //fader.VolumeDb(value);
        }

        public void SetState(ushort state)
        {
            
        }



        //public void Mute(ushort value)
        //{
        //    switch (value)
        //    {
        //        case(0):
        //            fader.Mute(false);
        //            break;
        //        case(1):
        //            fader.Mute(true);
        //            break;
        //        default:
        //            break;
        //    }
        //}

        public void RampTimeMS(ushort time)
        {
            cntrl.RampTimeMS(time);
        }


        
    }
}