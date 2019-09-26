using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysGainComponentSimpl
    {
        public delegate void VolumeChange(short valFloat, SimplSharpString valString);
        public VolumeChange newVolumeChange { get; set; }
        public delegate void VolumePositionChange(ushort valPos);
        public VolumePositionChange newVolumePositionChange { get; set; }
        public delegate void MuteChange(ushort value);
        public MuteChange newMuteChange { get; set; }
        public delegate void StringChange(SimplSharpString value);
        public StringChange newStringChange { get; set; }

        private QsysGainComponent gc;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent)
        {
            this.gc = new QsysGainComponent((int)_coreID, _namedComponent.ToString());
            this.gc.QsysFaderEvent += new EventHandler<QsysEventsArgs>(fader_QsysFaderEvent);
        }


        public void SetVolumePosition(ushort _position)
        {
            this.gc.SetPosition(this.gc.scale(_position, 0, 65535, 0.0, 1.0));
        }

        public void SetVolume(ushort _value)
        {
            this.gc.SetVolume((short)_value / 10);
        }

        public void SetVolumeString(SimplSharpString _value)
        {
            this.gc.SetVolumeString(_value.ToString());
        }

        public void SetMute(ushort _value)
        {
            bool b = Convert.ToBoolean(_value);
            this.gc.SetMute(Convert.ToBoolean(b));
        }

        public void ToggleMute()
        {
            this.gc.ToggleMute();
        }

        public void RampTimeMS(ushort _time)
        {
            this.gc.RampTimeMS(_time);
        }

        private void fader_QsysFaderEvent(object _sender, QsysEventsArgs _e)
        {
            switch (_e.EventID)
            {
                case eQscEventIds.GainChange:
                    if (newVolumeChange != null)
                        if (this.newVolumeChange != null)
                        {
                            this.newVolumeChange((short)(this.gc.VolumeLevel * 10), this.gc.VolumeString);
                        }
                        if (this.newVolumePositionChange != null)
                        {
                            this.newVolumePositionChange((ushort)this.gc.scale(this.gc.VolumePosition, 0.0, 1.0, 0, 65535));
                        }
                    break;
                case eQscEventIds.MuteChange:
                    if (newMuteChange != null)
                        newMuteChange((ushort)_e.NumberValue);
                    break;
            }
        }
    }
}