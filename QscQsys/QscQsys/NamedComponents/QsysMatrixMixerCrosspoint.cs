using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysMatrixMixerCrosspoint : QsysComponent
    {
        public delegate void CrossPointMuteChange(SimplSharpString cName, ushort value);
        public delegate void CrossPointGainChange(SimplSharpString cName, ushort value);
        public CrossPointMuteChange newCrossPointMuteChange { get; set; }
        public CrossPointGainChange newCrossPointGainChange { get; set; }



        private ushort _input;
        private ushort _output;
        private bool _initialized;

        private NamedComponentControl _muteControl;
        private NamedComponentControl _gainControl;

        private string MuteControlName
        {
            get { return ControlNameUtils.GetMatrixCrosspointMuteName(_input, _output); }
        }

        private string GainControlName
        {
            get { return ControlNameUtils.GetMatrixCrosspointGainName(_input, _output); }
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

        public void Initialize(string coreId, string componentName, ushort input, ushort output)
        {
            if (_initialized)
                return;

            _initialized = true;

            _input = input;
            _output = output;
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

            SendComponentChangeDoubleValue(MuteControlName,
                                           Convert.ToDouble(value));
        }

        public void SetCrossPointGain(ushort value)
        {
            SendComponentChangePosition(GainControlName, QsysCoreManager.ScaleDown(value));
        }

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
            var callback = newCrossPointMuteChange;
            if (callback != null)
                callback(MuteControlName, Convert.ToUInt16(args.Value));
        }

        #endregion

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

            gainControl.OnFeedbackReceived -= GainControlOnFeedbackReceived;
        }

        private void GainControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            var callback = newCrossPointGainChange;
            if (callback != null)
                callback(GainControlName, Convert.ToUInt16(QsysCoreManager.ScaleUp(args.Position)));
        }

        #endregion
    }
}