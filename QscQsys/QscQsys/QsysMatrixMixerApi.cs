using System;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysMatrixMixerApi
    {
        private bool _initialized;
        public string CoreId { get; private set; }
        public QsysCore Core { get; private set; }
        public string ComponentName { get; private set; }

        public void Initialize(string coreId, string name)
        {
            if (_initialized)
                return;
            _initialized = true;

            ComponentName = name;
            CoreId = coreId;

            QsysCoreManager.CoreAdded += QsysCoreManager_CoreAdded; 

            QsysCore core;
            if (QsysCoreManager.TryGetCore(CoreId, out core))
                Core = core;
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs args)
        {
            QsysCore core;
            if (QsysCoreManager.TryGetCore(CoreId, out core))
                Core = core;
            else
                Core = null;
        }

        /// <summary>
        /// Sets a crosspoint mute ex. *=everything, 1 2 3=channels 1, 2, 3,  1-6=channels 1 through 6, 1-8 !3=channels 1 through 8 except 3, * !3-5=everything but 3 through 5
        /// </summary>
        /// <param name="inputs">The input channels.</param>
        /// <param name="outputs">The output channels.</param>
        /// <param name="value">The value of the crosspoint mute.</param>
        public void SetCrossPointMute(string inputs, string outputs, bool value)
        {
            if (Core == null)
                return;

            var set = new SetCrossPointMute()
            {
                Params =
                    new SetCrossPointMuteParams
                    {
                        Name = ComponentName,
                        Inputs = inputs,
                        Outputs = outputs,
                        Value = value
                    }
            };
            Core.Enqueue(JsonConvert.SerializeObject(set));
        }

        public void SetCrossPointMute(string inputs, string outputs, ushort value)
        {
            SetCrossPointMute(inputs, outputs, Convert.ToBoolean(value));
        }
    }
}