using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysAes67Rx1 : AbstractQsysComponent
    {
        private const int NUM_CHANNELS = 4;

        private const string LEVEL_FORMAT = "channel_{0}_digital_input_level";
        private const string GAIN_FORMAT = "channel_{0}_input_gain";
        private const string INVERT_FORMAT = "channel_{0}_input_invert";
        private const string MUTE_FORMAT = "channel_{0}_input_mute";

        public delegate void LevelChange(SimplSharpString cName, ushort channel, ushort value);
        public delegate void GainChange(SimplSharpString cName, ushort channel, ushort value);
        public delegate void GainStringChange(SimplSharpString cName, ushort channel, SimplSharpString value);
        public delegate void InvertChange(SimplSharpString cName, ushort channel, ushort value);
        public delegate void MuteChange(SimplSharpString cName, ushort channel, ushort value);

        public LevelChange newLevelChange { get; set; }
        public GainChange newGainChange { get; set; }
        public GainStringChange newGainStringChange { get; set; }
        public InvertChange newInvertChange { get; set; }
        public MuteChange newMuteChange { get; set; }

        private readonly NamedComponentControl[] _levelControls = new NamedComponentControl[NUM_CHANNELS];
        private readonly NamedComponentControl[] _gainControls = new NamedComponentControl[NUM_CHANNELS];
        private readonly NamedComponentControl[] _invertControls = new NamedComponentControl[NUM_CHANNELS];
        private readonly NamedComponentControl[] _muteControls = new NamedComponentControl[NUM_CHANNELS];

        public void Initialize(string coreId, string componentName)
        {
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            for (int i = 0; i < NUM_CHANNELS; i++)
            {
                UnsubscribeControl(_levelControls[i]);
                UnsubscribeControl(_gainControls[i]);
                UnsubscribeControl(_invertControls[i]);
                UnsubscribeControl(_muteControls[i]);

                if (component == null)
                {
                    _levelControls[i] = null;
                    _gainControls[i] = null;
                    _invertControls[i] = null;
                    _muteControls[i] = null;
                }
                else
                {
                    int ch = i + 1;
                    var levelName = string.Format(LEVEL_FORMAT, ch);
                    var gainName = string.Format(GAIN_FORMAT, ch);
                    var invertName = string.Format(INVERT_FORMAT, ch);
                    var muteName = string.Format(MUTE_FORMAT, ch);
                    _levelControls[i] = component.LazyLoadComponentControl(levelName);
                    _gainControls[i] = component.LazyLoadComponentControl(gainName);
                    _invertControls[i] = component.LazyLoadComponentControl(invertName);
                    _muteControls[i] = component.LazyLoadComponentControl(muteName);

                    SubscribeLevelControl(_levelControls[i], i);
                    SubscribeGainControl(_gainControls[i], i);
                    SubscribeInvertControl(_invertControls[i], i);
                    SubscribeMuteControl(_muteControls[i], i);
                }
            }
        }

        public void SetGain(ushort channel, ushort value)
        {
            int idx = channel - 1;
            if (idx >= 0 && idx < NUM_CHANNELS && _gainControls[idx] != null)
                _gainControls[idx].SendChangePosition(SimplUtils.ScaleToDouble(value));
        }

        public void SetGainDb(ushort channel, short value)
        {
            int idx = channel - 1;
            if (idx >= 0 && idx < NUM_CHANNELS && _gainControls[idx] != null)
                _gainControls[idx].SendChangeDoubleValue((double)value);
        }

        public void SetMute(ushort channel, ushort value)
        {
            int idx = channel - 1;
            if (idx >= 0 && idx < NUM_CHANNELS && _muteControls[idx] != null)
                _muteControls[idx].SendChangeBoolValue(value.BoolFromSplus());
        }

        public void SetInvert(ushort channel, ushort value)
        {
            int idx = channel - 1;
            if (idx >= 0 && idx < NUM_CHANNELS && _invertControls[idx] != null)
                _invertControls[idx].SendChangeBoolValue(value.BoolFromSplus());
        }

        #region Subscriptions

        private void UnsubscribeControl(NamedComponentControl control)
        {
            if (control == null)
                return;

            control.OnStateChanged -= LevelControlChanged;
            control.OnStateChanged -= GainControlChanged;
            control.OnStateChanged -= InvertControlChanged;
            control.OnStateChanged -= MuteControlChanged;
        }

        private void SubscribeLevelControl(NamedComponentControl control, int index)
        {
            if (control == null)
                return;

            control.OnStateChanged += LevelControlChanged;
        }

        private void SubscribeGainControl(NamedComponentControl control, int index)
        {
            if (control == null)
                return;

            control.OnStateChanged += GainControlChanged;
        }

        private void SubscribeInvertControl(NamedComponentControl control, int index)
        {
            if (control == null)
                return;

            control.OnStateChanged += InvertControlChanged;
        }

        private void SubscribeMuteControl(NamedComponentControl control, int index)
        {
            if (control == null)
                return;

            control.OnStateChanged += MuteControlChanged;
        }

        private void LevelControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int ch = GetChannelFromControl(sender, _levelControls);
            if (ch < 0) return;

            var callback = newLevelChange;
            if (callback != null)
                callback(ComponentName, (ushort)(ch + 1), SimplUtils.ScaleToUshort(args.Position));
        }

        private void GainControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int ch = GetChannelFromControl(sender, _gainControls);
            if (ch < 0) return;

            if (args.Type == "position" || args.Type == "change")
            {
                var callback = newGainChange;
                if (callback != null)
                    callback(ComponentName, (ushort)(ch + 1), SimplUtils.ScaleToUshort(args.Position));
            }

            if (args.Type == "value" || args.Type == "change")
            {
                var callback = newGainStringChange;
                if (callback != null && !string.IsNullOrEmpty(args.StringValue))
                    callback(ComponentName, (ushort)(ch + 1), args.StringValue);
            }
        }

        private void InvertControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int ch = GetChannelFromControl(sender, _invertControls);
            if (ch < 0) return;

            var callback = newInvertChange;
            if (callback != null)
                callback(ComponentName, (ushort)(ch + 1), args.BoolValue.BoolToSplus());
        }

        private void MuteControlChanged(object sender, QsysInternalEventsArgs args)
        {
            int ch = GetChannelFromControl(sender, _muteControls);
            if (ch < 0) return;

            var callback = newMuteChange;
            if (callback != null)
                callback(ComponentName, (ushort)(ch + 1), args.BoolValue.BoolToSplus());
        }

        private int GetChannelFromControl(object sender, NamedComponentControl[] controls)
        {
            for (int i = 0; i < controls.Length; i++)
            {
                if (ReferenceEquals(sender, controls[i]))
                    return i;
            }
            return -1;
        }

        #endregion
    }
}
