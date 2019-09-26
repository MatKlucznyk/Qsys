using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysFaderSimpl
    {
        public delegate void VolumeChange(short valFloat, SimplSharpString valString);
        public VolumeChange newVolumeChange { get; set; }
        public delegate void VolumePositionChange(ushort valPos);
        public VolumePositionChange newVolumePositionChange { get; set; }
        public delegate void MuteChange(ushort value);
        public MuteChange newMuteChange { get; set; }
        public delegate void StringChange(SimplSharpString value);
        public StringChange newStringChange { get; set; }

        private QsysFader fader;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent)
        {
            this.fader = new QsysFader((int)_coreID, _namedComponent.ToString());
            this.fader.QsysFaderEvent += new EventHandler<QsysEventsArgs>(fader_QsysFaderEvent);
        }


        public void SetVolumePosition(ushort _position)
        {
            this.fader.SetPosition(this.fader.scale(_position, 0, 65535, 0.0, 1.0));
        }

        public void SetVolume(ushort _value)
        {
            this.fader.SetVolume((short)_value / 10);
        }

        public void SetMute(ushort _value)
        {
            this.fader.SetMute(Convert.ToBoolean(_value));
        }

        public void ToggleMute()
        {
            this.fader.ToggleMute();
        }

        public void RampTimeMS(ushort _time)
        {
            this.fader.RampTimeMS(_time);
        }

        private void fader_QsysFaderEvent(object _sender, QsysEventsArgs _e)
        {
            switch (_e.EventID)
            {
                case eQscEventIds.GainChange:
                    if (newVolumeChange != null)
                        if (this.newVolumeChange != null)
                        {
                            this.newVolumeChange((short)(this.fader.VolumeLevel * 10), this.fader.VolumeString);
                        }
                        if (this.newVolumePositionChange != null)
                        {
                            this.newVolumePositionChange((ushort)this.fader.scale(this.fader.VolumePosition, 0.0, 1.0, 0, 65535));
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