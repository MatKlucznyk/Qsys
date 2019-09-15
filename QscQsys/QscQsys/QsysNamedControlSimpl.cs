//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Crestron.SimplSharp;

//namespace QscQsys
//{
//    public class QsysNamedControlSimpl
//    {
//        public delegate void ValueChange(ushort valScaled, short valRaw, SimplSharpString valString);
//        public ValueChange newValueChange { get; set; }
//        public delegate void StateChange(ushort value);
//        public StateChange newStateChange { get; set; }
//        public delegate void StringChange(SimplSharpString value);
//        public StringChange newStringChange { get; set; }

//        private QsysNamedControl cntrl;

//        public void Initialize(string name, uint type)
//        {
//            eControlType t = (eControlType)Enum.Parse(typeof(eControlType), Convert.ToString(type), true);
//            cntrl = new QsysNamedControl(name, t);
//            cntrl.QsysNamedControlEvent += new EventHandler<QsysEventsArgs>(namedControl_QsysNamedControlEvent);
//        }

//        private void namedControl_QsysNamedControlEvent(object sender, QsysEventsArgs e)
//        {
//            switch (e.EventID)
//            {
//                case eQscEventIds.NamedControl:
//                    switch (cntrl.ControlType)
//                    {
//                        case eControlType.isValue:
//                            if (newValueChange != null && newStringChange != null)
//                            {
//                                if (e.StringValue == "[[VAL]]")
//                                {
//                                    newValueChange((ushort)cntrl.ValScaled, (short)cntrl.Val, cntrl.S_Val);
//                                }
//                            }
//                            break;
//                        case eControlType.isButton:
//                            if (newStateChange != null && newStringChange != null && newValueChange != null)
//                            {
//                                newStateChange(Convert.ToUInt16(e.BooleanValue));
//                            }
//                            break;
//                        case eControlType.isTrigger:
//                            if (newStateChange != null)
//                                newStateChange(Convert.ToUInt16(e.BooleanValue));
//                            break;
//                        case eControlType.isString:
//                            if (newStringChange != null)
//                                newStringChange(e.StringValue);
//                            break;
//                        default:
//                            break;
//                    }
//                    break;
//                default:
//                    break;
//            }
//        }

//        public void SetValueScaled(ushort value)
//        {
//            cntrl.SetValueScaled(value);
//        }

//        public void SetValueRaw(short value)
//        {
//            cntrl.SetValueRaw(value);
//        }

//        public void SetState(ushort value)
//        {
//            cntrl.SetState(Convert.ToBoolean(value));
//        }
        
//        public void SetStateToggle()
//        {
//            cntrl.SetStateToggle();
//        }
        
//        public void Trigger()
//        {
//            cntrl.Trigger();
//        }

//        public void SetString(SimplSharpString value)
//        {
//            cntrl.SetString(value.ToString());
//        }

//        public void RampTimeMS(ushort time)
//        {
//            cntrl.RampTimeMS(time);
//        }

//        public void SetMinMax(SimplSharpString newMin, SimplSharpString newMax)
//        {
//            cntrl.SetMinMaxViaString(newMin.ToString(), newMax.ToString());
//        }
        
//    }
//}