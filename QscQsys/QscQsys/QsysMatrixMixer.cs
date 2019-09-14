using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysMatrixMixer
    {
        private string cName;
        private bool registered;

        public QsysMatrixMixer(string name)
        {
            this.cName = name;
        }

        /// <summary>
        /// Sets a crosspoint mute ex. *=everything, 1 2 3=channels 1, 2, 3,  1-6=channels 1 through 6, 1-8 !3=channels 1 through 8 except 3, * !3-5=everything but 3 through 5
        /// </summary>
        /// <param name="inputs">The input channels.</param>
        /// <param name="outputs">The output channels.</param>
        /// <param name="value">The value of the crosspoint mute.</param>
        public void SetCrossPointMute(string inputs, string outputs, bool value)
        {
            SetCrossPointMute set = new SetCrossPointMute();
            set.Params = new SetCrossPointMuteParams();
            set.Params.Name = cName;
            set.Params.Inputs = inputs;
            set.Params.Outputs = outputs;
            set.Params.Value = value;

            QsysCore.Enqueue(JsonConvert.SerializeObject(set));
        }
    }
}