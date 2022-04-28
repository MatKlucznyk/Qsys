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
        private string coreId;
        private bool registered;

        public void Initialize(string coreId, string Name)
        {
            this.cName = Name;
            this.coreId = coreId;

            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                registered = true;
            }
        }

        /// <summary>
        /// Sets a crosspoint mute ex. *=everything, 1 2 3=channels 1, 2, 3,  1-6=channels 1 through 6, 1-8 !3=channels 1 through 8 except 3, * !3-5=everything but 3 through 5
        /// </summary>
        /// <param name="inputs">The input channels.</param>
        /// <param name="outputs">The output channels.</param>
        /// <param name="value">The value of the crosspoint mute.</param>
        public void SetCrossPointMute(string inputs, string outputs, bool value)
        {
            if (registered)
            {
                SetCrossPointMute set = new SetCrossPointMute() { Params = new SetCrossPointMuteParams() { Name = cName, Inputs = inputs, Outputs = outputs, Value = value } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(set));
            }
        }

        public void SetCrossPointMute(string inputs, string outputs, ushort value)
        {
            if (registered)
            {
                SetCrossPointMute set = new SetCrossPointMute() { Params = new SetCrossPointMuteParams() { Name = cName, Inputs = inputs, Outputs = outputs, Value = Convert.ToBoolean(value) } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(set));
            }
        }
    }
}