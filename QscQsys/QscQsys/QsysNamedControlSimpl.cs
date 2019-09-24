using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysNamedControlSimpl
    {
        public delegate void ValueIntChange(ushort valInt, SimplSharpString valString);
        public ValueIntChange newValueIntChange { get; set; }
        public delegate void ValueFloatChange(short valFloat, SimplSharpString valString);
        public ValueFloatChange newValueFloatChange { get; set; }
        public delegate void PositionChange(ushort valPos);
        public PositionChange newPositionChange { get; set; }
        public delegate void StateChange(ushort value);
        public StateChange newStateChange { get; set; }
        public delegate void StringChange(SimplSharpString value);
        public StringChange newStringChange { get; set; }

        private QsysNamedControl nc;

        public void Initialize(ushort _coreID, SimplSharpString _namedControl, ushort _controlType)
        {
            eControlType t = (eControlType)Enum.Parse(typeof(eControlType), Convert.ToString(_controlType), true);
            this.nc = new QsysNamedControl((int)_coreID,_namedControl.ToString(), t);
            this.nc.QsysNamedControlEvent += new EventHandler<QsysEventsArgs>(namedControl_QsysNamedControlEvent);
        }

        private void namedControl_QsysNamedControlEvent(object _sender, QsysEventsArgs _e)
        {
            switch (_e.EventID)
            {
                case eQscEventIds.NamedControl:
                    switch (this.nc.ControlType)
                    {
                        case eControlType.isIntegerValue:
                            if (this.newValueIntChange != null)
                            {
                                this.newValueIntChange((ushort)this.nc.ControlValue, this.nc.ControlString);
                            }
                            if (this.newPositionChange != null)
                            {
                                this.newPositionChange((ushort)this.nc.scale(this.nc.ControlPosition,0.0,1.0,0,65535));
                            }
                            break;
                        case eControlType.isFloatValue:
                            if (this.newValueFloatChange != null)
                            {
                                this.newValueFloatChange((short)(this.nc.ControlValue*10), this.nc.ControlString);
                            }
                            if (this.newPositionChange != null)
                            {
                                this.newPositionChange((ushort)this.nc.scale(this.nc.ControlPosition, 0.0, 1.0, 0, 65535));
                            }
                            break;
                        case eControlType.isButton:
                            if (this.newStateChange != null)
                                this.newStateChange(Convert.ToUInt16(_e.BooleanValue));
                            break;
                        case eControlType.isTrigger:
                            if (newStateChange != null)
                                this.newStateChange(Convert.ToUInt16(_e.BooleanValue));
                            break;
                        case eControlType.isString:
                            if (newStringChange != null)
                                this.newStringChange(_e.StringValue);
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetPosition(ushort _position)
        {
            this.nc.SetPosition(this.nc.scale(_position, 0, 65535, 0.0, 1.0));
        }

        public void SetValue(ushort _value)
        {
            if (this.nc.ControlType == eControlType.isFloatValue)
            {
                this.nc.SetValue((short)_value / 10);
            }
            else
            {
                this.nc.SetValue(_value);
            }
        }
        public void SetValueInt(ushort _value)
        {
            this.nc.SetValue(_value);
        }

        public void SetState(ushort _value)
        {
            this.nc.SetState(Convert.ToBoolean(_value));
        }

        public void SetStateToggle()
        {
            this.nc.SetStateToggle();
        }

        public void Trigger()
        {
            this.nc.Trigger();
        }

        public void SetString(SimplSharpString _value)
        {
            this.nc.SetString(_value.ToString());
        }

        public void RampTimeMS(ushort _time)
        {
            this.nc.RampTimeMS(_time);
        }

    }
}