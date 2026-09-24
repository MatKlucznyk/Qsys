using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysNv21hDecoder : AbstractQsysComponent
    {
        private const int HDMI_OUTPUT = 1;

        public delegate void Nv21hDecoderInputChange(SimplSharpString cName, ushort input);
        public Nv21hDecoderInputChange newNv21hDecoderInputChange { get; set; }

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

            InputControl = component.LazyLoadComponentControl(ControlNameUtils.GetHdmiOutputSelectName(HDMI_OUTPUT));
        }

        public void ChangeInput(int source)
        {
            if (InputControl != null)
                InputControl.SendChangeDoubleValue(source);
        }

        #region Input Control Callbacks

        private void SubscribeInputControl(NamedComponentControl inputControl)
        {
            if (inputControl == null)
                return;

            inputControl.OnStateChanged += InputControlOnStateChanged;
        }

        private void UnsubscribeInputControl(NamedComponentControl inputControl)
        {
            if (inputControl == null)
                return;

            inputControl.OnStateChanged -= InputControlOnStateChanged;
        }

        private void InputControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            _currentSource = Convert.ToInt16(args.Value);

            var callback = newNv21hDecoderInputChange;
            if (callback != null)
                callback(ComponentName, Convert.ToUInt16(_currentSource));
        }

        #endregion
    }
}