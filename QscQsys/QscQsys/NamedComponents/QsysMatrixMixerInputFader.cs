using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysMatrixMixerInputFader : AbstractQsysComponent
    {
        public delegate void MuteChange(SimplSharpString cName, ushort value);
        public delegate void VolumeChange(SimplSharpString cName, ushort value);
        public MuteChange newCrossPointMuteChange { get; set; }
        public VolumeChange newCrossPointGainChange { get; set; }

        private ushort _input;
        private bool _initialized;

        private NamedComponentControl _muteControl;
        private NamedComponentControl _gainControl;

        private string MuteControlName
        {
            get { return ControlNameUtils.GetMatrixInputMuteName(_input); }
        }

        private string GainControlName
        {
            get { return ControlNameUtils.GetMatrixInputGainName(_input); }
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

        public NamedComponentControl GainControl
        {
            get { return _gainControl; }
            private set
            {
                if (_gainControl == value)
                    return;

                UnsubscribeGainControl(_gainControl);
                _gainControl = value;
                SubscribeGainControl(_gainControl);
            }
        }

        public void Initialize(string coreId, string componentName, ushort input)
        {
            if (_initialized)
                return;

            _initialized = true;

            _input = input;
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
            {
                MuteControl = null;
                GainControl = null;
                return;
            }

            MuteControl = component.LazyLoadComponentControl(MuteControlName);
            GainControl = component.LazyLoadComponentControl(GainControlName);
        }

        public void SetCrossPointMute(ushort value)
        {
            if (MuteControl != null)
                MuteControl.SendChangeBoolValue(value.BoolFromSplus());
        }

        public void SetCrossPointGain(ushort value)
        {
            if (GainControl != null)
                GainControl.SendChangePosition(SimplUtils.ScaleToDouble(value));
        }

        #region Mute Control Callbacks

        private void SubscribeMuteControl(NamedComponentControl muteControl)
        {
            if (muteControl == null)
                return;

            muteControl.OnStateChanged += MuteControlOnStateChanged;
        }

        private void UnsubscribeMuteControl(NamedComponentControl muteControl)
        {
            if (muteControl == null)
                return;

            muteControl.OnStateChanged -= MuteControlOnStateChanged;
        }

        private void MuteControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            var callback = newCrossPointMuteChange;
            if (callback != null)
                callback(MuteControlName, args.BoolValue.BoolToSplus());
        }

        #endregion

        #region Gain Control Callbacks

        private void SubscribeGainControl(NamedComponentControl gainControl)
        {
            if (gainControl == null)
                return;

            gainControl.OnStateChanged += GainControlOnStateChanged;
        }

        private void UnsubscribeGainControl(NamedComponentControl gainControl)
        {
            if (gainControl == null)
                return;

            gainControl.OnStateChanged -= GainControlOnStateChanged;
        }

        private void GainControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            var callback = newCrossPointGainChange;
            if (callback != null)
                callback(GainControlName, SimplUtils.ScaleToUshort(args.Position));
        }

        #endregion
    }
}