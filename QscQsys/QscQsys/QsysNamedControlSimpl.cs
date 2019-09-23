using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysNamedControlSimpl
    {
        public delegate void ValueChange(ushort valScaled, short valRaw, SimplSharpString valString);
        public ValueChange newValueChange { get; set; }
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
                            if (this.newValueChange != null && this.newStringChange != null)
                            {
                                this.newValueChange((ushort)this.nc.ValScaled, (short)this.nc.Val, this.nc.S_Val);
                            }
                            break;
                        case eControlType.isFloatValue:
                            if (this.newValueChange != null && this.newStringChange != null)
                            {
                                this.newValueChange((ushort)this.nc.ValScaled, (short)(this.nc.Val*10), this.nc.S_Val);
                            }
                            break;
                        case eControlType.isButton:
                            if (this.newStateChange != null && this.newStringChange != null && this.newValueChange != null)
                            {
                                this.newStateChange(Convert.ToUInt16(_e.BooleanValue));
                            }
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

        public void SetValueScaled(ushort _value)
        {
            this.nc.SetValueScaled(_value);
        }

        public void SetValueRaw(short _value)
        {
            if (this.nc.ControlType == eControlType.isIntegerValue)
            {
                this.nc.SetValueRaw(_value);
            }
            else if (this.nc.ControlType == eControlType.isFloatValue)
            {
                this.nc.SetValueRaw(_value/10);
            }
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

        public void SetMinMax(SimplSharpString _newMin, SimplSharpString _newMax)
        {
            this.nc.SetMinMaxViaString(_newMin.ToString(), _newMax.ToString());
        }
    }
}