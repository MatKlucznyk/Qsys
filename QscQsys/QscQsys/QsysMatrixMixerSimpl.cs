using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMatrixMixerSimpl
    {
        public delegate void CrossPointValueChange(ushort value);
        public CrossPointValueChange newCrossPointValueChange { get; set; }
        public delegate void CrossPointMuteChange(ushort value);
        public CrossPointMuteChange newCrosspointMuteChange { get; set; }

        private QsysMatrixMixer mixer;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent, ushort _input, ushort _output)
        {
            this.mixer = new QsysMatrixMixer((int)_coreID, _namedComponent.ToString(), _input, _output);
            this.mixer.QsysMatrixMixerEvent += new EventHandler<QsysEventsArgs>(matrix_QsysMatrixMixerEvent);
        }

        private void matrix_QsysMatrixMixerEvent(object _sender, QsysEventsArgs _e)
        {
            switch (_e.EventID)
            {
                case eQscEventIds.MuteChange:
                    if (newCrosspointMuteChange != null)
                        newCrosspointMuteChange((ushort)_e.NumberValue);
                    break;
            }
        }

       
        public void SetCrossPoint(ushort value)
        {
            switch (value)
            {
                case (1):
                    mixer.SetCrossPointMute(this.mixer.Input.ToString(), this.mixer.Output.ToString(), true);
                    break;
                case (0):
                    mixer.SetCrossPointMute(this.mixer.Input.ToString(), this.mixer.Output.ToString(), false);
                    break;
                default:
                    break;
            }
        }
    }
}