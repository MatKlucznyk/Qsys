using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysFaderSimpl
    {
        public delegate void VolumeChange(ushort value, short valueDb);
        public VolumeChange newVolumeChange { get; set; }
        public delegate void MuteChange(ushort value);
        public MuteChange newMuteChange { get; set; }

        private QsysFader fader;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent)
        {
            this.fader = new QsysFader((int)_coreID, _namedComponent.ToString());
            this.fader.QsysFaderEvent += new EventHandler<QsysEventsArgs>(fader_QsysFaderEvent);
        }

        public void Volume(ushort _value)
        {
            this.fader.Volume(_value);
        }

        public void VolumeDb(short _value)
        {
            this.fader.VolumeDb(_value);
        }

        public void Mute(ushort _value)
        {
            switch (_value)
            {
                case (0):
                    this.fader.Mute(false);
                    break;
                case (1):
                    this.fader.Mute(true);
                    break;
                default:
                    break;
            }
        }

        public void RampTimeMS(ushort _time)
        {
            this.fader.RampTimeMS(_time);
        }

        private void fader_QsysFaderEvent(object _sender, QsysEventsArgs _e)
        {
            switch (_e.EventID)
            {
                case eQscEventIds.NewCommand:
                    break;
                case eQscEventIds.GainChange:
                    if (newVolumeChange != null)
                        newVolumeChange((ushort)_e.NumberValue, (short)this.fader.CurrentVolumeDb);
                    break;
                case eQscEventIds.MuteChange:
                    if (newMuteChange != null)
                        newMuteChange((ushort)_e.NumberValue);
                    break;
                case eQscEventIds.NewMaxGain:
                    break;
                case eQscEventIds.NewMinGain:
                    break;
            }
        }
    }
}