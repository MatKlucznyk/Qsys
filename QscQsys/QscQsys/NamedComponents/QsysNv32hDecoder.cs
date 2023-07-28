using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;

namespace QscQsys.NamedComponents
{
    public sealed class QsysNv32hDecoder : AbstractQsysComponent
    {
        private const string CONTROL_NAME = "hdmi_out_0_select_index";

        public delegate void Nv32hDecoderInputChange(SimplSharpString cName, ushort input);
        public Nv32hDecoderInputChange newNv32hDecoderInputChange { get; set; }

        private NamedComponentControl _inputControl;

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

        private int _currentSource;

        public int CurrentSource { get { return _currentSource; } }

        public void Initialize(string coreId, string componentName)
        {
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
            {
                InputControl = null;
                return;
            }

            InputControl = component.LazyLoadComponentControl(CONTROL_NAME);
        }

        public void ChangeInput(int source)
        {
            SendComponentChangeDoubleValue(CONTROL_NAME, source);
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
            _currentSource = Convert.ToInt16(args.Value);

            var callback = newNv32hDecoderInputChange;
            if (callback != null)
                callback(ComponentName, Convert.ToUInt16(_currentSource));
        }

        #endregion
    }
}