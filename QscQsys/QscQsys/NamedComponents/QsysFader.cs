using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public class QsysFader : AbstractQsysComponent
    {
        public delegate void VolumeChange(SimplSharpString cName, ushort value);
        public delegate void MuteChange(SimplSharpString cName, ushort value);
        public delegate void GainStringChange(SimplSharpString cName, SimplSharpString value);
        public VolumeChange newVolumeChange { get; set; }
        public MuteChange newMuteChange { get; set; }
        public GainStringChange newGainStringChange { get; set; }

        private bool _currentMute;
        private string _currentGainString;
        private int _currentLvl;

        private NamedComponentControl _gainControl;
        private NamedComponentControl _muteControl;

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

        public bool MuteValue { get { return _currentMute; } }
        public int VolumeValue { get { return _currentLvl; } }
        public string GainValue { get { return _currentGainString; } }

        public void Initialize(string coreId, string componentName)
        {
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
            {
                GainControl = null;
                MuteControl = null;
                return;
            }

            GainControl = component.LazyLoadComponentControl(ControlNameUtils.GetGainControlName());
            MuteControl = component.LazyLoadComponentControl(ControlNameUtils.GetMuteControlName());
        }

        #region Gain Control Callbacks

        private void SubscribeGainControl(NamedComponentControl gainControl)
        {
            if (gainControl == null)
                return;

            gainControl.OnFeedbackReceived += GainControlOnFeedbackReceived;
        }

        private void UnsubscribeGainControl(NamedComponentControl gainControl)
        {
            if (gainControl == null)
                return;

            gainControl.OnFeedbackReceived += GainControlOnFeedbackReceived;
        }

        private void GainControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            if (args.Type == "position" || args.Type == "change")
            {
                _currentLvl = SimplUtils.ScaleToUshort(args.Position);

                var callback = newVolumeChange;
                if (callback != null)
                    callback(ComponentName, (ushort)_currentLvl);
            }

            if (args.Type == "value" || args.Type == "change")
            {
                _currentGainString = args.StringValue;

                var callback = newGainStringChange;
                if (callback != null && !string.IsNullOrEmpty(_currentGainString))
                    callback(ComponentName, _currentGainString);
            }
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

            muteControl.OnFeedbackReceived += MuteControlOnFeedbackReceived;
        }

        private void MuteControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            _currentMute = args.BoolValue;

            var callback = newMuteChange;
            if (callback != null)
                callback(ComponentName, _currentMute.BoolToSplus());
        }

        #endregion

        /// <summary>
        /// Sets the current volume value.
        /// </summary>
        /// <param name="value">Volume value to set the fader to.</param>
        public void Volume(ushort value)
        {
            if (GainControl != null)
                GainControl.SendChangePosition(SimplUtils.ScaleToDouble(value));
        }

        public void Decibels(double value)
        {
            if (GainControl != null)
                GainControl.SendChangeDoubleValue(value);
        }

        public void Decibels(short value)
        {
            Decibels((double)value);
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(bool value)
        {
            if (MuteControl != null)
                MuteControl.SendChangeBoolValue(value);
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(ushort value)
        {
            Mute(value.BoolFromSplus());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                GainControl = null;
                MuteControl = null;
            }
        }
    }
}