using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysNvmDecoder : AbstractQsysComponent
    {
        public delegate void NvmDecoderInputChange(SimplSharpString cName, ushort input);
        public delegate void VideoFreezeChange(SimplSharpString cName, ushort value);
        public delegate void VideoMuteChange(SimplSharpString cName, ushort value);
        public delegate void HdmiEnabledChange(SimplSharpString cName, ushort value);
        public NvmDecoderInputChange newNvmDecoderInputChange { get; set; }
        public VideoFreezeChange newVideoFreezeChange { get; set; }
        public VideoMuteChange newVideoMuteChange { get; set; }
        public HdmiEnabledChange newHdmiEnabledChange { get; set; }

        private NamedComponentControl _inputControl;
        private NamedComponentControl _videoFreezeControl;
        private NamedComponentControl _videoMuteControl;
        private NamedComponentControl _hdmiEnabledControl;

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

        public NamedComponentControl VideoFreezeControl
        {
            get { return _videoFreezeControl; }
            private set
            {
                if (_videoFreezeControl == value)
                    return;

                UnsubscribeVideoFreezeControl(_videoFreezeControl);
                _videoFreezeControl = value;
                SubscribeVideoFreezeControl(_videoFreezeControl);
            }
        }

        public NamedComponentControl VideoMuteControl
        {
            get { return _videoMuteControl; }
            private set
            {
                if (_videoMuteControl == value)
                    return;

                UnsubscribeVideoMuteControl(_videoMuteControl);
                _videoMuteControl = value;
                SubscribeVideoMuteControl(_videoMuteControl);
            }
        }

        public NamedComponentControl HdmiEnabledControl
        {
            get { return _hdmiEnabledControl; }
            private set
            {
                if (_hdmiEnabledControl == value)
                    return;

                UnsubscribeHdmiEnabledControl(_hdmiEnabledControl);
                _hdmiEnabledControl = value;
                SubscribeHdmiEnabledControl(_hdmiEnabledControl);
            }
        }

        private int _currentSource;
        private bool _currentVideoFreeze;
        private bool _currentVideoMute;
        private bool _currentHdmiEnabled;

        public int CurrentSource { get { return _currentSource; } }
        public bool VideoFreezeValue { get { return _currentVideoFreeze; } }
        public bool VideoMuteValue { get { return _currentVideoMute; } }
        public bool HdmiEnabledValue { get { return _currentHdmiEnabled; } }

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
                VideoFreezeControl = null;
                VideoMuteControl = null;
                HdmiEnabledControl = null;
                return;
            }

            InputControl = component.LazyLoadComponentControl(ControlNameUtils.GetNvmHdmiOutputSelectName());
            VideoFreezeControl = component.LazyLoadComponentControl(ControlNameUtils.GetNvmHdmiVideoFreezeName());
            VideoMuteControl = component.LazyLoadComponentControl(ControlNameUtils.GetNvmHdmiVideoMuteName());
            HdmiEnabledControl = component.LazyLoadComponentControl(ControlNameUtils.GetNvmHdmiEnabledName());
        }

        /// <summary>
        /// Selects the input source. The select control is a string control, so the index is sent as a string.
        /// </summary>
        /// <param name="source">Input source index to select.</param>
        public void ChangeInput(int source)
        {
            if (InputControl != null)
                InputControl.SendChangeStringValue(source.ToString());
        }

        /// <summary>
        /// Sets the current video freeze state.
        /// </summary>
        /// <param name="value">The state to set the video freeze.</param>
        public void VideoFreeze(bool value)
        {
            if (VideoFreezeControl != null)
                VideoFreezeControl.SendChangeBoolValue(value);
        }

        /// <summary>
        /// Sets the current video freeze state.
        /// </summary>
        /// <param name="value">The state to set the video freeze.</param>
        public void VideoFreeze(ushort value)
        {
            VideoFreeze(value.BoolFromSplus());
        }

        /// <summary>
        /// Sets the current video mute state.
        /// </summary>
        /// <param name="value">The state to set the video mute.</param>
        public void VideoMute(bool value)
        {
            if (VideoMuteControl != null)
                VideoMuteControl.SendChangeBoolValue(value);
        }

        /// <summary>
        /// Sets the current video mute state.
        /// </summary>
        /// <param name="value">The state to set the video mute.</param>
        public void VideoMute(ushort value)
        {
            VideoMute(value.BoolFromSplus());
        }

        /// <summary>
        /// Sets the current HDMI enabled state.
        /// </summary>
        /// <param name="value">The state to set the HDMI enabled.</param>
        public void HdmiEnabled(bool value)
        {
            if (HdmiEnabledControl != null)
                HdmiEnabledControl.SendChangeBoolValue(value);
        }

        /// <summary>
        /// Sets the current HDMI enabled state.
        /// </summary>
        /// <param name="value">The state to set the HDMI enabled.</param>
        public void HdmiEnabled(ushort value)
        {
            HdmiEnabled(value.BoolFromSplus());
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
            _currentSource = ParseSource(args);

            var callback = newNvmDecoderInputChange;
            if (callback != null)
                callback(ComponentName, Convert.ToUInt16(_currentSource));
        }

        /// <summary>
        /// The select control is a string control, so read the index from its string, falling back to its value.
        /// </summary>
        private static int ParseSource(QsysInternalEventsArgs args)
        {
            if (!string.IsNullOrEmpty(args.StringValue))
            {
                try
                {
                    return int.Parse(args.StringValue);
                }
                catch (FormatException)
                {
                }
                catch (OverflowException)
                {
                }
            }

            return Convert.ToInt16(args.Value);
        }

        #endregion

        #region Video Freeze Control Callbacks

        private void SubscribeVideoFreezeControl(NamedComponentControl videoFreezeControl)
        {
            if (videoFreezeControl == null)
                return;

            videoFreezeControl.OnStateChanged += VideoFreezeControlOnStateChanged;
        }

        private void UnsubscribeVideoFreezeControl(NamedComponentControl videoFreezeControl)
        {
            if (videoFreezeControl == null)
                return;

            videoFreezeControl.OnStateChanged -= VideoFreezeControlOnStateChanged;
        }

        private void VideoFreezeControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            _currentVideoFreeze = args.BoolValue;

            var callback = newVideoFreezeChange;
            if (callback != null)
                callback(ComponentName, _currentVideoFreeze.BoolToSplus());
        }

        #endregion

        #region Video Mute Control Callbacks

        private void SubscribeVideoMuteControl(NamedComponentControl videoMuteControl)
        {
            if (videoMuteControl == null)
                return;

            videoMuteControl.OnStateChanged += VideoMuteControlOnStateChanged;
        }

        private void UnsubscribeVideoMuteControl(NamedComponentControl videoMuteControl)
        {
            if (videoMuteControl == null)
                return;

            videoMuteControl.OnStateChanged -= VideoMuteControlOnStateChanged;
        }

        private void VideoMuteControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            _currentVideoMute = args.BoolValue;

            var callback = newVideoMuteChange;
            if (callback != null)
                callback(ComponentName, _currentVideoMute.BoolToSplus());
        }

        #endregion

        #region HDMI Enabled Control Callbacks

        private void SubscribeHdmiEnabledControl(NamedComponentControl hdmiEnabledControl)
        {
            if (hdmiEnabledControl == null)
                return;

            hdmiEnabledControl.OnStateChanged += HdmiEnabledControlOnStateChanged;
        }

        private void UnsubscribeHdmiEnabledControl(NamedComponentControl hdmiEnabledControl)
        {
            if (hdmiEnabledControl == null)
                return;

            hdmiEnabledControl.OnStateChanged -= HdmiEnabledControlOnStateChanged;
        }

        private void HdmiEnabledControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            _currentHdmiEnabled = args.BoolValue;

            var callback = newHdmiEnabledChange;
            if (callback != null)
                callback(ComponentName, _currentHdmiEnabled.BoolToSplus());
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                InputControl = null;
                VideoFreezeControl = null;
                VideoMuteControl = null;
                HdmiEnabledControl = null;
            }
        }
    }
}
