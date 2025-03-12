using System;
using Crestron.SimplSharp;
using JetBrains.Annotations;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    /// <summary>
    /// Router for Q-Sys cameras
    /// </summary>
    [PublicAPI("S+")]
    public sealed class QsysMediacastRouter : AbstractQsysComponent
    {
        public delegate void RouterInputChangeDelegate(SimplSharpString cName, ushort input);

        [PublicAPI("S+")]
        public RouterInputChangeDelegate RouterInputChange { get; set; }

        private NamedComponentControl _inputControl;
        private int _currentSelectedInput;

        [PublicAPI]
        public int CurrentSelectedInput
        {
            get { return _currentSelectedInput; }
            private set
            {
                if (_currentSelectedInput == value)
                    return;

                _currentSelectedInput = value;

                var callback = RouterInputChange;
                if (callback != null)
                    callback(ComponentName, (ushort)value);
            }
        }

        [PublicAPI]
        public int Output { get; private set; }

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

        [PublicAPI("S+")]
        public void Initialize(string coreId, string componentName, int output)
        {
            Output = output;

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

            InputControl = component.LazyLoadComponentControl(ControlNameUtils.GetRouterSelectName(Output));
        }

        [PublicAPI("S+")]
        public void InputSelect(int input)
        {
            if (InputControl != null)
                InputControl.SendChangeDoubleValue(input);
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
            CurrentSelectedInput = Convert.ToInt16(args.Value);
        }

        #endregion
    }
}