using System;
using Crestron.SimplSharp;
using JetBrains.Annotations;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    [PublicAPI("S+")]
    public sealed class QsysCamera : AbstractQsysComponent
    {
        public delegate void PrivacyChange(SimplSharpString componentName, ushort privacyValue);

        public delegate void BrightnessChange(SimplSharpString componentName, ushort brightnessValue);

        public delegate void SaturationChange(SimplSharpString componentName, ushort saturationValue);

        public delegate void SharpnessChange(SimplSharpString componentName, ushort sharpnessValue);

        public delegate void ContrastChange(SimplSharpString componentName, ushort contrastValue);

        public delegate void ExposureModeChange(SimplSharpString componentName, SimplSharpString mode);

        public delegate void IrisChange(SimplSharpString componentName, SimplSharpString irisValue);

        public delegate void ShutterChange(SimplSharpString componentName, SimplSharpString apertureValue);

        public delegate void GainChange(SimplSharpString componentName, ushort gainValue);

        public delegate void AutoWhiteBalanceSensitivityChange(SimplSharpString componentName, SimplSharpString awbSens);

        public delegate void AutoWhiteBalanceModeChange(SimplSharpString componentName, SimplSharpString awbMode);

        public delegate void WhiteBalanceHueChange(SimplSharpString componentName, ushort hueValue);

        public delegate void WhiteBalanceRedGainChange(SimplSharpString componentName, ushort redGainValue);

        public delegate void WhiteBalanceBlueGainChange(SimplSharpString componentName, ushort blueGainValue);

        public delegate void AutoFocusChange(SimplSharpString componentName, ushort value);

        [PublicAPI("S+")]
        public PrivacyChange onPrivacyChange { get; set; }
        [PublicAPI("S+")]
        public BrightnessChange onBrightnessChange { get; set; }
        [PublicAPI("S+")]
        public SaturationChange onSaturationChange { get; set; }
        [PublicAPI("S+")]
        public SharpnessChange onSharpnessChange { get; set; }
        [PublicAPI("S+")]
        public ContrastChange onContrastChange { get; set; }
        [PublicAPI("S+")]
        public ExposureModeChange onExposureModeChange { get; set; }
        [PublicAPI("S+")]
        public IrisChange onIrisChange { get; set; }
        [PublicAPI("S+")]
        public ShutterChange onShutterChange { get; set; }
        [PublicAPI("S+")]
        public GainChange onGainChange { get; set; }
        [PublicAPI("S+")]
        public AutoWhiteBalanceSensitivityChange onAutoWhiteBalanceSensitivityChange { get; set; }
        [PublicAPI("S+")]
        public AutoWhiteBalanceModeChange onAutoWhiteBalanceModeChange { get; set; }
        [PublicAPI("S+")]
        public WhiteBalanceHueChange onWhiteBalanceHueChange { get; set; }
        [PublicAPI("S+")]
        public WhiteBalanceRedGainChange onWhiteBalanceRedGainChange { get; set; }
        [PublicAPI("S+")]
        public WhiteBalanceBlueGainChange onWhiteBalanceBlueGainChange { get; set; }
        [PublicAPI("S+")]
        public AutoFocusChange onAutoFocusChange { get; set; }


        private const string CONTROL_TOGGLE_PRIVACY = "toggle_privacy";
        private const string CONTROL_IMAGE_BRIGHTNESS = "img_brightness";
        private const string CONTROL_IMAGE_SATURATION = "img_saturation";
        private const string CONTROL_IMAGE_SHARPNESS = "img_sharpness";
        private const string CONTROL_IMAGE_CONTRAST = "img_contrast";
        private const string CONTROL_EXPOSURE_MODE = "exp_mode";
        private const string CONTROL_EXPOSURE_IRIS = "exp_iris";
        private const string CONTROL_EXPOSURE_SHUTTER = "exp_shutter";
        private const string CONTROL_EXPOSURE_GAIN = "exp_gain";
        private const string CONTROL_WHITEBALANCE_AUTO_SENSITIVITY = "wb_awb_sensitivity";
        private const string CONTROL_WHITEBALANCE_AUTO_MODE = "wb_awb_mode";
        private const string CONTROL_WHITEBALANCE_HUE = "wb_hue";
        private const string CONTROL_WHITEBALANCE_GAIN_RED = "wb_red_gain";
        private const string CONTROL_WHITEBALANCE_GAIN_BLUE = "wb_blue_gain";
        private const string CONTROL_FOCUS_AUTO = "focus_auto";

        private bool _currentPrivacy;
        private ushort _currentBri;
        private ushort _currentSat;
        private ushort _currentSharp;
        private ushort _currentCont;
        private string _currentExpMode;
        private string _currentIris;
        private string _currentShutter;
        private ushort _currentGain;
        private string _currentAwbSens;
        private string _currentAwbMode;
        private ushort _currentHue;
        private ushort _currentRed;
        private ushort _currentBlue;
        private ushort _currentAutoFocus;

        public bool PrivacyValue { get { return _currentPrivacy; } }
        public ushort BrightnessValue { get { return _currentBri; } }
        public ushort SaturationValue { get { return _currentSat; } }
        public ushort SharpnessValue { get { return _currentSharp; } }
        public ushort ContrastValue { get { return _currentCont; } }
        public string ExposureModeValue { get { return _currentExpMode; } }
        public string IrisValue { get { return _currentIris; } }
        public string ShutterValue { get { return _currentShutter; } }
        public ushort GainValue { get { return _currentGain; } }
        public string AutoWhiteBalanceSensitivityValue { get { return _currentAwbSens; } }
        public string AutoWhiteBalanceModeValue { get { return _currentAwbMode; } }
        public ushort HueValue { get { return _currentHue; } }
        public ushort RedGainValue { get { return _currentRed; } }
        public ushort BlueGainValue { get { return _currentBlue; } }
        public ushort AutoFocusValue { get { return _currentAutoFocus; } }

        public void Initialize(string coreId, string componentName)
        {
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
                return;

            component.LazyLoadComponentControl(CONTROL_TOGGLE_PRIVACY);
            component.LazyLoadComponentControl(CONTROL_IMAGE_BRIGHTNESS);
            component.LazyLoadComponentControl(CONTROL_IMAGE_SATURATION);
            component.LazyLoadComponentControl(CONTROL_IMAGE_SHARPNESS);
            component.LazyLoadComponentControl(CONTROL_IMAGE_CONTRAST);
            component.LazyLoadComponentControl(CONTROL_EXPOSURE_MODE);
            component.LazyLoadComponentControl(CONTROL_EXPOSURE_IRIS);
            component.LazyLoadComponentControl(CONTROL_EXPOSURE_SHUTTER);
            component.LazyLoadComponentControl(CONTROL_EXPOSURE_GAIN);
            component.LazyLoadComponentControl(CONTROL_WHITEBALANCE_AUTO_SENSITIVITY);
            component.LazyLoadComponentControl(CONTROL_WHITEBALANCE_AUTO_MODE);
            component.LazyLoadComponentControl(CONTROL_WHITEBALANCE_HUE);
            component.LazyLoadComponentControl(CONTROL_WHITEBALANCE_GAIN_RED);
            component.LazyLoadComponentControl(CONTROL_WHITEBALANCE_GAIN_BLUE);
            component.LazyLoadComponentControl(CONTROL_FOCUS_AUTO);
        }

        protected override void ComponentOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            base.ComponentOnFeedbackReceived(sender, args);

            switch (args.Name)
            {
                case CONTROL_TOGGLE_PRIVACY:
                    _currentPrivacy = Convert.ToBoolean(args.Value);

                    if (onPrivacyChange != null)
                        onPrivacyChange(ComponentName, Convert.ToUInt16(args.Value));
                    break;
                case CONTROL_IMAGE_BRIGHTNESS:
                    if (onBrightnessChange != null)
                    {
                        onBrightnessChange(ComponentName, SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_IMAGE_SATURATION:
                    if (onSaturationChange != null)
                    {
                        onSaturationChange(ComponentName, SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_IMAGE_SHARPNESS:
                    if (onSharpnessChange != null)
                    {
                        onSharpnessChange(ComponentName, SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_IMAGE_CONTRAST:
                    if (onContrastChange != null)
                    {
                        onContrastChange(ComponentName, SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_EXPOSURE_MODE:
                    if (onExposureModeChange != null)
                    {
                        onExposureModeChange(ComponentName, args.StringValue);
                    }
                    break;
                case CONTROL_EXPOSURE_IRIS:
                    if (onIrisChange != null)
                    {
                        onIrisChange(ComponentName, args.StringValue);
                    }
                    break;
                case CONTROL_EXPOSURE_SHUTTER:
                    if (onShutterChange != null)
                    {
                        onShutterChange(ComponentName, args.StringValue);
                    }
                    break;
                case CONTROL_EXPOSURE_GAIN:
                    if (onGainChange != null)
                    {
                        onGainChange(ComponentName, SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_WHITEBALANCE_AUTO_SENSITIVITY:
                    if (onAutoWhiteBalanceSensitivityChange != null)
                    {
                        onAutoWhiteBalanceSensitivityChange(ComponentName, args.StringValue);
                    }
                    break;
                case CONTROL_WHITEBALANCE_AUTO_MODE:
                    if (onAutoWhiteBalanceModeChange != null)
                    {
                        onAutoWhiteBalanceModeChange(ComponentName, args.StringValue);
                    }
                    break;
                case CONTROL_WHITEBALANCE_HUE:
                    if (onWhiteBalanceHueChange != null)
                    {
                        onWhiteBalanceHueChange(ComponentName,SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_WHITEBALANCE_GAIN_RED:
                    if (onWhiteBalanceRedGainChange != null)
                    {
                        onWhiteBalanceRedGainChange(ComponentName,SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_WHITEBALANCE_GAIN_BLUE:
                    if (onWhiteBalanceBlueGainChange != null)
                    {
                        onWhiteBalanceBlueGainChange(ComponentName,SimplUtils.ScaleToUshort(args.Position));
                    }
                    break;
                case CONTROL_FOCUS_AUTO:
                    if (onAutoFocusChange != null)
                    {
                        onAutoFocusChange(ComponentName, (ushort)args.Value);
                    }
                    break;
            }
        }

        public void StartPTZ(PtzTypes type)
        {
            if (Component == null)
                return;

            string controlName = ControlNameUtils.GetControlNameForPtzType(type);
            Component.SendChangeDoubleValue(controlName, 1);
        }

        public void StopPTZ(PtzTypes type)
        {
            if (Component == null)
                return;

            string controlName = ControlNameUtils.GetControlNameForPtzType(type);
            Component.SendChangeDoubleValue(controlName, 0);
        }

        public void AutoFocus()
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("focus_auto", 1);
        }

        public void FocusNear()
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("focus_near", 1);
        }

        public void FocusNearStop()
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("focus_near", 0);
        }

        public void FocusFar()
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("focus_far", 1);
        }

        public void FocusFarStop()
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("focus_far", 0);
        }

        public void RecallHome()
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("preset_home_load", 1);
        }

        public void SaveHome()
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("preset_home_save_trigger", 1);
        }

        public void PrivacyToggle(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangeDoubleValue("toggle_privacy", value);
        }

        public void PrivacyToggle(bool value)
        {
            PrivacyToggle(Convert.ToUInt16(value));
        }

        public void Brightness(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("img_brightness", SimplUtils.ScaleToDouble(value));
        }

        public void Saturation(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("img_saturation", SimplUtils.ScaleToDouble(value));
        }

        public void Sharpness(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("img_sharpness", SimplUtils.ScaleToDouble(value));
        }

        public void Contrast(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("img_contrast", SimplUtils.ScaleToDouble(value));
        }

        public void ExposureMode(string value)
        {
            if (Component == null)
                return;

            Component.SendChangeStringValue("exp_mode", value);
        }

        public void Iris(string value)
        {
            if (Component == null)
                return;

            Component.SendChangeStringValue("exp_iris", value);
        }

        public void Shutter(string value)
        {
            if (Component == null)
                return;

            Component.SendChangeStringValue("exp_shutter", value);
        }

        public void Gain(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("exp_gain", SimplUtils.ScaleToDouble(value));
        }

        public void AutoWhiteBalanceMode(string value)
        {
            if (Component == null)
                return;

            Component.SendChangeStringValue("wb_awb_mode", value);
        }

        public void AutoWhiteBalanceSensitivity(string value)
        {
            if (Component == null)
                return;

            Component.SendChangeStringValue("wb_awb_sensitivity", value);
        }

        public void Hue(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("wb_hue", SimplUtils.ScaleToDouble(value));
        }

        public void RedGain(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("wb_red_gain", SimplUtils.ScaleToDouble(value));
        }

        public void BlueGain(ushort value)
        {
            if (Component == null)
                return;

            Component.SendChangePosition("wb_blue_gain", SimplUtils.ScaleToDouble(value));
        }

        public void TiltUp()
        {
            StartPTZ(PtzTypes.Up);
        }

        public void StopTiltUp()
        {
            StopPTZ(PtzTypes.Up);
        }

        public void TiltDown()
        {
            StartPTZ(PtzTypes.Down);
        }

        public void StopTiltDown()
        {
            StopPTZ(PtzTypes.Down);
        }

        public void PanLeft()
        {
            StartPTZ(PtzTypes.Left);
        }

        public void StopPanLeft()
        {
            StopPTZ(PtzTypes.Left);
        }

        public void PanRight()
        {
            StartPTZ(PtzTypes.Right);
        }

        public void StopPanRight()
        {
            StopPTZ(PtzTypes.Right);
        }

        public void ZoomIn()
        {
            StartPTZ(PtzTypes.ZoomIn);
        }

        public void StopZoomIn()
        {
            StopPTZ(PtzTypes.ZoomIn);
        }

        public void ZoomOut()
        {
            StartPTZ(PtzTypes.ZoomOut);
        }

        public void StopZoomOut()
        {
            StopPTZ(PtzTypes.ZoomOut);
        }

        public enum PtzTypes
        {
            Up = 1,
            Down = 2,
            Left = 3,
            Right = 4,
            ZoomIn = 5,
            ZoomOut = 6
        }
    }
}