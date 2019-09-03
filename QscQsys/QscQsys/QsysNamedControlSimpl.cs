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

        public void Initialize(string name)
        {
            cntrl = new QsysNamedControl(name);
            cntrl.QsysNamedControlEvent += new EventHandler<QsysEventsArgs>(namedControl_QsysNamedControlEvent);
        }

        public void SetControlType(uint type)
        {
            eControlType t = (eControlType)Enum.Parse(typeof(eControlType), Convert.ToString(type), true);
            cntrl.SetControlType(t);
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


        //public void Volume(ushort value)
        //{
        //    fader.Volume(value);
        //}

        //public void VolumeDb(short value)
        //{
        //    fader.VolumeDb(value);
        //}

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
            //fader.RampTimeMS(time);
        }

        public void test()
        {
            CrestronConsole.PrintLine("module name: {0} type: {1}", cntrl.ControlName, cntrl.ControlType); 
        }

        
    }
}