using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public class QsysMatrixMixerOutputAllCrosspoints : AbstractQsysComponent
    {
        public delegate void CrossPointMuteChange(SimplSharpString cName, ushort input, ushort value);

        public CrossPointMuteChange newCrossPointMuteChange { get; set; }

        private ushort _inputCount;
        private ushort _output;
        private bool _initialized;
        private readonly Dictionary<NamedComponentControl, int> _muteControls;

        public QsysMatrixMixerOutputAllCrosspoints()
        {
            _muteControls = new Dictionary<NamedComponentControl, int>();
        }

        public void Initialize(string coreId, string componentName, ushort inputCount, ushort output)
        {
            if (_initialized)
                return;

            _initialized = true;

            _inputCount = inputCount;
            _output = output;

            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            lock (_muteControls)
            {
                foreach (var control in _muteControls.Keys)
                    UnsubscribeMuteControl(control);
                _muteControls.Clear();

                if (component == null)
                    return;

                for (int input = 1; input <= _inputCount; input++)
                {
                    string controlName = ControlNameUtils.GetMatrixCrosspointMuteName(input, _output);
                    var control = component.LazyLoadComponentControl(controlName);
                    _muteControls.Add(control, input);
                    SubscribeMuteControl(control);
                }
            }
        }

        private void SubscribeMuteControl(NamedComponentControl control)
        {
            if (control == null)
                return;

            control.OnFeedbackReceived += ControlOnFeedbackReceived;
        }

        private void UnsubscribeMuteControl(NamedComponentControl control)
        {
            if (control == null)
                return;

            control.OnFeedbackReceived -= ControlOnFeedbackReceived;
        }

        private void ControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            NamedComponentControl control = sender as NamedComponentControl;
            if (control == null)
                return;

            int inputNumber;
            lock (_muteControls)
            {
                if (!_muteControls.TryGetValue(control, out inputNumber))
                    return;
            }

            bool muted = Math.Abs(args.Value) > QsysCore.TOLERANCE;

            var callback = newCrossPointMuteChange;
            if (callback != null)
                callback(args.Name, (ushort)inputNumber, Convert.ToUInt16(muted));

        }

        public void SetCrossPointMute(ushort input, ushort value)
        {
            string muteControlName = ControlNameUtils.GetMatrixCrosspointMuteName(input, _output);
            SendComponentChangeDoubleValue(muteControlName, Convert.ToDouble(value));
        }
    }
}