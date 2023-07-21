using System;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysMatrixMixerApi
    {
        private string _cName;
        private string _coreId;
        private bool _registered;

        public void Initialize(string coreId, string Name)
        {
            if (!_registered)
            {
                this._cName = Name;
                this._coreId = coreId;

                QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
            }
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!_registered && e.CoreId == _coreId)
            {
                _registered = true;
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
            if (_registered)
            {
                var set = new SetCrossPointMute() { Params = new SetCrossPointMuteParams() { Name = _cName, Inputs = inputs, Outputs = outputs, Value = value } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(set));
            }
        }

        public void SetCrossPointMute(string inputs, string outputs, ushort value)
        {
            if (_registered)
            {
                var set = new SetCrossPointMute() { Params = new SetCrossPointMuteParams() { Name = _cName, Inputs = inputs, Outputs = outputs, Value = Convert.ToBoolean(value) } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(set));
            }
        }
    }
}