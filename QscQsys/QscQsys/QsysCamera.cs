using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysCamera
    {
        public delegate void PrivacyChange(ushort privacyValue);
        public delegate void BrightnessChange(ushort brightnessValue);
        public delegate void SaturationChange(ushort saturationValue);
        public delegate void SharpnessChange(ushort sharpnessValue);
        public delegate void ContrastChange(ushort contrastValue);
        public delegate void ExposureModeChange(SimplSharpString mode);
        public delegate void IrisChange(ushort irisValue);
        public delegate void ApertureChange(ushort apertureValue);
        public delegate void GainChange(ushort gainValue);
        public delegate void AutoWhiteBalanceSensitivityChange(SimplSharpString awbSens);
        public delegate void AutoWhiteBalanceModeChange(SimplSharpString awbMode);
        public delegate void WhiteBalanceHueChange(ushort hueValue);
        public delegate void WhiteBalanceRedGainChange(ushort redGainValue);
        public delegate void WhiteBalanceBlueGainChange(ushort blueGainValue);
        public PrivacyChange onPrivacyChange { get; set; }
        public BrightnessChange onBrightnessChange { get; set; }
        public SaturationChange onSaturationChange { get; set; }
        public SharpnessChange onSharpnessChange { get; set; }
        public ContrastChange onContrastChange { get; set; }
        public ExposureModeChange onExposureModeChange { get; set; }
        public IrisChange onIrisChange { get; set; }
        public ApertureChange onApertureChange { get; set; }
        public GainChange onGainChange { get; set; }
        public AutoWhiteBalanceSensitivityChange onAutoWhiteBalanceSensitivityChange { get; set; }
        public AutoWhiteBalanceModeChange onAutoWhiteBalanceModeChange { get; set; }
        public WhiteBalanceHueChange onWhiteBalanceHueChange { get; set; }
        public WhiteBalanceRedGainChange onWhiteBalanceRedGainChange { get; set; }
        public WhiteBalanceBlueGainChange onWhiteBalanceBlueGainChange { get; set; }

        private string cName;
        private string coreId;
        private bool registered;
        private bool currentPrivacy;

        public void Initialize(string coreId, string name)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

            cName = name;
            this.coreId = coreId;

            if (!registered)
                RegisterWithCore();
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                RegisterWithCore();
            }
        }

        private void RegisterWithCore()
        {
            if (QsysCoreManager.Cores.ContainsKey(coreId))
            {
                Component component = new Component()
                {
                    Name = cName,
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
                        new ControlName() { Name = "wb_blue_gain" }

                    }
                };

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        private void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            //QsysMeterEvent(this, new QsysEventsArgs(eQscEventIds.MeterUpdate, cName, Convert.ToBoolean(e.Value), Convert.ToInt16(e.Value), e.SValue, null));

            if (e.Name == "toggle_privacy")
            {
                currentPrivacy = Convert.ToBoolean(e.Value);

                if (onPrivacyChange != null)
                    onPrivacyChange(Convert.ToUInt16(e.Value));
            }
            else if (e.Name == "img_brightness")
            {
                if (onBrightnessChange != null)
                {
                    onBrightnessChange((ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "img_saturation")
            {
                if (onSaturationChange != null)
                {
                    onSaturationChange((ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "img_sharpness")
            {
                if (onSharpnessChange != null)
                {
                    onSharpnessChange((ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
            else if (e.Name == "img_contrast")
            {
                if (onContrastChange != null)
                {
                    onContrastChange((ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
        }

        public void StartPTZ(PtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>()} };

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

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void StopPTZ(PtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() } };

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

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void RecallHome()
        {
            ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "preset_home_load", Value = 1 } } } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void SaveHome()
        {
            ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "preset_home_save_trigger", Value = 1 } } } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void PrivacyToggle(ushort value)
        {
            ComponentChange cameraChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "toggle_privacy", Value = value } } } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void PrivacyToggle(bool value)
        {
            PrivacyToggle(Convert.ToUInt16(value));
        }

        public void Brightness(int value)
        {
            ComponentChange newVolumeChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_brightness", Position = QsysCoreManager.ScaleDown(value) } } } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Brightness(ushort value)
        {
            Brightness((int)value);
        }

        public void Saturation(int value)
        {
            ComponentChange newVolumeChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_saturation", Position = QsysCoreManager.ScaleDown(value) } } } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Saturation(ushort value)
        {
            Saturation((int)value);
        }

        public void Sharpness(int value)
        {
            ComponentChange newVolumeChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_sharpness", Position = QsysCoreManager.ScaleDown(value) } } } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Sharpness(ushort value)
        {
            Sharpness((int)value);
        }

        public void Contrast(int value)
        {
            ComponentChange newVolumeChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "img_contrast", Position = QsysCoreManager.ScaleDown(value) } } } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void Contrast(ushort value)
        {
            Contrast((int)value);
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