using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysCamera : QsysComponent
    {
        public delegate void PrivacyChange(SimplSharpString _cName, ushort privacyValue);
        public delegate void BrightnessChange(SimplSharpString _cName, ushort brightnessValue);
        public delegate void SaturationChange(SimplSharpString _cName, ushort saturationValue);
        public delegate void SharpnessChange(SimplSharpString _cName, ushort sharpnessValue);
        public delegate void ContrastChange(SimplSharpString _cName, ushort contrastValue);
        public delegate void ExposureModeChange(SimplSharpString _cName, SimplSharpString mode);
        public delegate void IrisChange(SimplSharpString _cName, SimplSharpString irisValue);
        public delegate void ShutterChange(SimplSharpString _cName, SimplSharpString apertureValue);
        public delegate void GainChange(SimplSharpString _cName, ushort gainValue);
        public delegate void AutoWhiteBalanceSensitivityChange(SimplSharpString _cName, SimplSharpString awbSens);
        public delegate void AutoWhiteBalanceModeChange(SimplSharpString _cName, SimplSharpString awbMode);
        public delegate void WhiteBalanceHueChange(SimplSharpString _cName, ushort hueValue);
        public delegate void WhiteBalanceRedGainChange(SimplSharpString _cName, ushort redGainValue);
        public delegate void WhiteBalanceBlueGainChange(SimplSharpString _cName, ushort blueGainValue);
        public delegate void AutoFocusChange(SimplSharpString _cName, ushort value);
        public PrivacyChange onPrivacyChange { get; set; }
        public BrightnessChange onBrightnessChange { get; set; }
        public SaturationChange onSaturationChange { get; set; }
        public SharpnessChange onSharpnessChange { get; set; }
        public ContrastChange onContrastChange { get; set; }
        public ExposureModeChange onExposureModeChange { get; set; }
        public IrisChange onIrisChange { get; set; }
        public ShutterChange onShutterChange { get; set; }
        public GainChange onGainChange { get; set; }
        public AutoWhiteBalanceSensitivityChange onAutoWhiteBalanceSensitivityChange { get; set; }
        public AutoWhiteBalanceModeChange onAutoWhiteBalanceModeChange { get; set; }
        public WhiteBalanceHueChange onWhiteBalanceHueChange { get; set; }
        public WhiteBalanceRedGainChange onWhiteBalanceRedGainChange { get; set; }
        public WhiteBalanceBlueGainChange onWhiteBalanceBlueGainChange { get; set; }
        public AutoFocusChange onAutoFocusChange { get; set; }

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
            var component = new Component()
            {
                Name = componentName,
                Controls = new List<ControlName>() 
                    { 
                        new ControlName() { Name = "toggle_privacy" }, 
                        new ControlName() { Name = "img_brightness" },
                        new ControlName() { Name = "img_saturation" },
                        new ControlName() { Name = "img_sharpness" },
                        new ControlName() { Name = "img_contrast" },
                        new ControlName() { Name = "exp_mode" },
                        new ControlName() { Name = "exp_iris" },
                        new ControlName() { Name = "exp_shutter" },
                        new ControlName() { Name = "exp_gain" },
                        new ControlName() { Name = "wb_awb_sensitivity" },
                        new ControlName() { Name = "wb_awb_mode" },
                        new ControlName() { Name = "wb_hue" },
                        new ControlName() { Name = "wb_red_gain" },
                        new ControlName() { Name = "wb_blue_gain" },
                        new ControlName() { Name = "focus_auto" }

                    }
            };

            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == "toggle_privacy")
            {
                _currentPrivacy = Convert.ToBoolean(e.Value);

                if (onPrivacyChange != null)
                    onPrivacyChange(_cName, Convert.ToUInt16(e.Value));
            }
            else if (e.Name == "img_brightness")
            {
                if (onBrightnessChange != null)
                {
                    onBrightnessChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "img_saturation")
            {
                if (onSaturationChange != null)
                {
                    onSaturationChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "img_sharpness")
            {
                if (onSharpnessChange != null)
                {
                    onSharpnessChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "img_contrast")
            {
                if (onContrastChange != null)
                {
                    onContrastChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "exp_mode")
            {
                if (onExposureModeChange != null)
                {
                    onExposureModeChange(_cName, e.SValue);
                }
            }
            else if (e.Name == "exp_iris")
            {
                if (onIrisChange != null)
                {
                    onIrisChange(_cName, e.SValue);
                }
            }
            else if (e.Name == "exp_shutter")
            {
                if (onShutterChange != null)
                {
                    onShutterChange(_cName, e.SValue);
                }
            }
            else if (e.Name == "exp_gain")
            {
                if (onGainChange != null)
                {
                    onGainChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "wb_awb_sensitivity")
            {
                if (onAutoWhiteBalanceSensitivityChange != null)
                {
                    onAutoWhiteBalanceSensitivityChange(_cName, e.SValue);
                }
            }
            else if (e.Name == "wb_awb_mode")
            {
                if (onAutoWhiteBalanceModeChange != null)
                {
                    onAutoWhiteBalanceModeChange(_cName, e.SValue);
                }
            }
            else if (e.Name == "wb_hue")
            {
                if (onWhiteBalanceHueChange != null)
                {
                    onWhiteBalanceHueChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "wb_red_gain")
            {
                if (onWhiteBalanceRedGainChange != null)
                {
                    onWhiteBalanceRedGainChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "wb_blue_gain")
            {
                if (onWhiteBalanceBlueGainChange != null)
                {
                    onWhiteBalanceBlueGainChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "focus_auto")
            {
                if (onAutoFocusChange != null)
                {
                    onAutoFocusChange(_cName, (ushort)e.Value);
                }
            }
        }

        public void StartPTZ(PtzTypes type)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() } };

                ComponentSetValue camera = new ComponentSetValue();

                switch (type)
                {
                    case PtzTypes.Up:
                        camera.Name = "tilt_up";
                        camera.Value = 1;
                        break;
                    case PtzTypes.Down:
                        camera.Name = "tilt_down";
                        camera.Value = 1;
                        break;
                    case PtzTypes.Left:
                        camera.Name = "pan_left";
                        camera.Value = 1;
                        break;
                    case PtzTypes.Right:
                        camera.Name = "pan_right";
                        camera.Value = 1;
                        break;
                    case PtzTypes.ZoomIn:
                        camera.Name = "zoom_in";
                        camera.Value = 1;
                        break;
                    case PtzTypes.ZoomOut:
                        camera.Name = "zoom_out";
                        camera.Value = 1;
                        break;
                    default:
                        break;
                }

                cameraChange.Params.Controls.Add(camera);

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void StopPTZ(PtzTypes type)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() } };

                ComponentSetValue camera = new ComponentSetValue();

                switch (type)
                {
                    case PtzTypes.Up:
                        camera.Name = "tilt_up";
                        camera.Value = 0;
                        break;
                    case PtzTypes.Down:
                        camera.Name = "tilt_down";
                        camera.Value = 0;
                        break;
                    case PtzTypes.Left:
                        camera.Name = "pan_left";
                        camera.Value = 0;
                        break;
                    case PtzTypes.Right:
                        camera.Name = "pan_right";
                        camera.Value = 0;
                        break;
                    case PtzTypes.ZoomIn:
                        camera.Name = "zoom_in";
                        camera.Value = 0;
                        break;
                    case PtzTypes.ZoomOut:
                        camera.Name = "zoom_out";
                        camera.Value = 0;
                        break;
                    default:
                        break;
                }

                cameraChange.Params.Controls.Add(camera);

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void AutoFocus()
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "focus_auto", Value = 1 } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void FocusNear()
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "focus_near", Value = 1 } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void FocusNearStop()
        {
            if(_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "focus_near", Value = 0 } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void FocusFar()
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "focus_far", Value = 1 } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void FocusFarStop()
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "focus_far", Value = 0 } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void RecallHome()
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "preset_home_load", Value = 1 } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SaveHome()
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "preset_home_save_trigger", Value = 1 } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void PrivacyToggle(ushort value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "toggle_privacy", Value = value } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void PrivacyToggle(bool value)
        {
            PrivacyToggle(Convert.ToUInt16(value));
        }

        public void Brightness(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_brightness", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Brightness(ushort value)
        {
            Brightness((int)value);
        }

        public void Saturation(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_saturation", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Saturation(ushort value)
        {
            Saturation((int)value);
        }

        public void Sharpness(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_sharpness", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Sharpness(ushort value)
        {
            Sharpness((int)value);
        }

        public void Contrast(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_contrast", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Contrast(ushort value)
        {
            Contrast((int)value);
        }

        public void ExposureMode(string value)
        {
            if (_registered)
            {
                ComponentChangeString cameraChange = new ComponentChangeString() { Params = new ComponentChangeParamsString() { Name = _cName, Controls = new List<ComponentSetValueString>() { new ComponentSetValueString() { Name = "exp_mode", Value = value } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Iris(string value)
        {
            if (_registered)
            {
                ComponentChangeString cameraChange = new ComponentChangeString() { Params = new ComponentChangeParamsString() { Name = _cName, Controls = new List<ComponentSetValueString>() { new ComponentSetValueString() { Name = "exp_iris", Value = value } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Shutter(string value)
        {
            if (_registered)
            {
                ComponentChangeString cameraChange = new ComponentChangeString() { Params = new ComponentChangeParamsString() { Name = _cName, Controls = new List<ComponentSetValueString>() { new ComponentSetValueString() { Name = "exp_shutter", Value = value } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Gain(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "exp_gain", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Gain(ushort value)
        {
            Gain((int)value);
        }

        public void AutoWhiteBalanceMode(string value)
        {
            if (_registered)
            {
                ComponentChangeString cameraChange = new ComponentChangeString() { Params = new ComponentChangeParamsString() { Name = _cName, Controls = new List<ComponentSetValueString>() { new ComponentSetValueString() { Name = "wb_awb_mode", Value = value } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void AutoWhiteBalanceSensitivity(string value)
        {
            if (_registered)
            {
                ComponentChangeString cameraChange = new ComponentChangeString() { Params = new ComponentChangeParamsString() { Name = _cName, Controls = new List<ComponentSetValueString>() { new ComponentSetValueString() { Name = "wb_awb_sensitivity", Value = value } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Hue(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "wb_hue", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Hue(ushort value)
        {
            Hue((int)value);
        }

        public void RedGain(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "wb_red_gain", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void RedGain(ushort value)
        {
            RedGain((int)value);
        }

        public void BlueGain(int value)
        {
            if (_registered)
            {
                ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "wb_blue_gain", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void BlueGain(ushort value)
        {
            BlueGain((int)value);
        }

        public void TiltUp()
        {
            StartPTZ(QsysCamera.PtzTypes.Up);
        }

        public void StopTiltUp()
        {
            StopPTZ(QsysCamera.PtzTypes.Up);
        }

        public void TiltDown()
        {
            StartPTZ(QsysCamera.PtzTypes.Down);
        }

        public void StopTiltDown()
        {
            StopPTZ(QsysCamera.PtzTypes.Down);
        }

        public void PanLeft()
        {
            StartPTZ(QsysCamera.PtzTypes.Left);
        }

        public void StopPanLeft()
        {
            StopPTZ(QsysCamera.PtzTypes.Left);
        }

        public void PanRight()
        {
            StartPTZ(QsysCamera.PtzTypes.Right);
        }

        public void StopPanRight()
        {
            StopPTZ(QsysCamera.PtzTypes.Right);
        }

        public void ZoomIn()
        {
            StartPTZ(QsysCamera.PtzTypes.ZoomIn);
        }

        public void StopZoomIn()
        {
            StopPTZ(QsysCamera.PtzTypes.ZoomIn);
        }

        public void ZoomOut()
        {
            StartPTZ(QsysCamera.PtzTypes.ZoomOut);
        }

        public void StopZoomOut()
        {
            StopPTZ(QsysCamera.PtzTypes.ZoomOut);
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