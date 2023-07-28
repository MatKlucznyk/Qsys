using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysRouter : QsysComponent
    {
        public delegate void RouterInputChange(SimplSharpString cName, ushort input);
        public delegate void MuteChange(SimplSharpString cName, ushort value);
        public RouterInputChange newRouterInputChange { get; set; }
        public MuteChange newOutputMuteChange { get; set; }

        private int _output;
        private int _currentSelectedInput;
        private bool _currentMute;

        private NamedComponentControl _inputControl;
        private NamedComponentControl _muteControl;

        public int CurrentSelectedInput { get { return _currentSelectedInput; } }
        public int Output { get { return _output; } }
        public bool CurrentMute {get { return _currentMute; } }

        public NamedComponentControl InputControl
        {
            get { return _inputControl; }
            private set
            {
                if (_inputControl == value)
                    return;

                UnsubscribeInputControl(_inputControl);
                _inputControl = value;
                SubscribeInputControl(_inputControl);
            }
        }

        public NamedComponentControl MuteControl
        {
            get { return _muteControl; }
            private set
            {
                if (_muteControl == value)
                    return;

                UnsubscribeMuteControl(_muteControl);
                _muteControl = value;
                SubscribeMuteControl(_muteControl);
            }
        }

        public void Initialize(string coreId, string componentName, int output)
        {
            _output = output;

            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
            {
                InputControl = null;
                MuteControl = null;
                return;
            }

            InputControl = component.LazyLoadComponentControl(ControlNameUtils.GetRouterSelectName(_output));
            MuteControl = component.LazyLoadComponentControl(ControlNameUtils.GetRouterMuteName(_output));
        }

        public void InputSelect(int input)
        {
            SendComponentChangeDoubleValue(ControlNameUtils.GetRouterSelectName(_output), input);
        }

        public void OutputMute(bool value)
        {
            SendComponentChangeDoubleValue(ControlNameUtils.GetRouterMuteName(_output), Convert.ToDouble(value));
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void OutputMute(ushort value)
        {
            OutputMute(Convert.ToBoolean(value));
        }

        #region Input Control Callbacks

        private void SubscribeInputControl(NamedComponentControl inputControl)
        {
            if (inputControl == null)
                return;

            inputControl.OnFeedbackReceived += InputControlOnFeedbackReceived;
        }

        private void UnsubscribeInputControl(NamedComponentControl inputControl)
        {
            if (inputControl == null)
                return;

            inputControl.OnFeedbackReceived -= InputControlOnFeedbackReceived;
        }

        private void InputControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            _currentSelectedInput = Convert.ToInt16(args.Value);

            var callback = newRouterInputChange;
            if (callback != null)
                callback(ComponentName, Convert.ToUInt16(_currentSelectedInput));
        }

        #endregion

        #region Mute Control Callbacks

        private void SubscribeMuteControl(NamedComponentControl muteControl)
        {
            if (muteControl == null)
                return;

            muteControl.OnFeedbackReceived += MuteControlOnFeedbackReceived;
        }

        private void UnsubscribeMuteControl(NamedComponentControl muteControl)
        {
            if (muteControl == null)
                return;

            muteControl.OnFeedbackReceived -= MuteControlOnFeedbackReceived;
        }

        private void MuteControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            _currentMute = Math.Abs(args.Value) > QsysCore.TOLERANCE;

            var callback = newOutputMuteChange;
            if (callback != null)
                callback(ComponentName, Convert.ToUInt16(_currentMute));
        }

        #endregion
    }
}