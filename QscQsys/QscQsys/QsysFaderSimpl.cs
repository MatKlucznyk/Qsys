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
        public delegate void MuteChange(ushort value);
        public VolumeChange newVolumeChange { get; set; }
        public MuteChange newMuteChange { get; set; }

        private QsysFader fader;

        public void Initialize(string name)
        {
            fader = new QsysFader(name);
            fader.QsysFaderEvent += new EventHandler<QsysEventsArgs>(fader_QsysFaderEvent);
        }

        public void Volume(ushort value)
        {
            fader.Volume(value);
        }

        public void VolumeDb(short value)
        {
            fader.VolumeDb(value);
        }

        public void Mute(ushort value)
        {
            switch (value)
            {
                case(0):
                    fader.Mute(false);
                    break;
                case(1):
                    fader.Mute(true);
                    break;
                default:
                    break;
            }
        }

        public void RampTimeMS(ushort time)
        {
            fader.RampTimeMS(time);
        }

        private void fader_QsysFaderEvent(object sender, QsysEventsArgs e)
        {
            switch (e.EventID)
            {
                case eQscEventIds.NewCommand:
                    break;
                case eQscEventIds.GainChange:
                    if (newVolumeChange != null)
                        newVolumeChange((ushort)e.IntegerValue, (short)fader.CurrentVolumeDb);
                    break;
                case eQscEventIds.MuteChange:
                    if (newMuteChange != null)
                        newMuteChange((ushort)e.IntegerValue);
                    break;
                case eQscEventIds.NewMaxGain:
                    break;
                case eQscEventIds.NewMinGain:
                    break;
                case eQscEventIds.CameraStreamChange:
                    break;
                case eQscEventIds.PotsControllerOffHook:
                    break;
                case eQscEventIds.PotsControllerIsRinging:
                    break;
                case eQscEventIds.PotsControllerDialString:
                    break;
                case eQscEventIds.PotsControllerCurrentlyCalling:
                    break;
                default:
                    break;
            }
        }
    }
}