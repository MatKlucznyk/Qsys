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
        private readonly Dictionary<int, NamedComponentControl> _muteControlsByInput;

        public QsysMatrixMixerOutputAllCrosspoints()
        {
            _muteControls = new Dictionary<NamedComponentControl, int>();
            _muteControlsByInput = new Dictionary<int, NamedComponentControl>();
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
                _muteControlsByInput.Clear();

                if (component == null)
                    return;

                for (int input = 1; input <= _inputCount; input++)
                {
                    string controlName = ControlNameUtils.GetMatrixCrosspointMuteName(input, _output);
                    var control = component.LazyLoadComponentControl(controlName);
                    _muteControls.Add(control, input);
                    _muteControlsByInput.Add(input, control);
                    SubscribeMuteControl(control);
                    UpdateState(input, control.State);
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

            UpdateState(inputNumber, args.Data);
        }

        private void UpdateState(int input, QsysStateData state)
        {
            var callback = newCrossPointMuteChange;
            if (callback != null)
                callback(state.Name, (ushort)input, state.BoolValue.BoolToSplus());
        }

        public void SetCrossPointMute(ushort input, ushort value)
        {
            NamedComponentControl control;
            lock (_muteControls)
            {
                if (!_muteControlsByInput.TryGetValue(input, out control))
                    return;
            }

            control.SendChangeBoolValue(value.BoolFromSplus());
        }
    }
}