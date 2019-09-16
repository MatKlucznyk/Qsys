using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace QscQsys
{
    public class QsysCamera
    {
        //Core
        private QsysCore myCore;

        //Named Component
        private string componentName;
        public string ComponentName { get { return componentName; } }
        private bool registered;
        public bool IsRegistered { get { return registered; } }
        private bool isComponent;

        //Internal Vars
        private double positionPan;
        public double PositionPan { get { return positionPan; } }
        private double positionTilt;
        public double PositionTilt { get { return positionTilt; } }
        private double positionZoom;
        public double PositionZoom { get { return positionZoom; } }
        private double speedPan;
        public double SpeedPan { get { return speedPan; } }
        private double speedTilt;
        public double SpeedTilt { get { return speedTilt; } }
        private double speedZoom;
        public double SpeedZoom { get { return speedZoom; } }
        private double speedRecall;
        public double SpeedRecall { get { return speedRecall; } }
        private bool afAutoMode;
        public bool AfAutoMode { get { return afAutoMode; } }
        private bool privacyMode;
        public bool PrivacyMode { get { return privacyMode; } }
        private bool homeMode;
        public bool HomeMode { get { return homeMode; } }


        //Events
        public event EventHandler<QsysEventsArgs> QsysCamEvent;

        public QsysCamera(int _coreID, string _componentName)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);

            Component component = new Component();
            component.Name = this.componentName;
            List<ControlName> names = new List<ControlName>();
            names.Add(new ControlName() { Name = "ptz_preset" });
            names.Add(new ControlName() { Name = "setup_pan_speed" });
            names.Add(new ControlName() { Name = "setup_tilt_speed" });
            names.Add(new ControlName() { Name = "setup_zoom_speed" });
            names.Add(new ControlName() { Name = "aaaa_setup_snapshot_speed" });
            names.Add(new ControlName() { Name = "focus_auto" });
            names.Add(new ControlName() { Name = "toggle_privacy" });
            names.Add(new ControlName() { Name = "preset_home_load" });
            component.Controls = names;

            if (this.myCore.RegisterNamedComponent(component))
            {
                this.myCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);
                this.registered = true;
                this.isComponent = true;
            }
        }

        void Component_OnNewEvent(object _sender, QsysInternalEventsArgs _e)
        {
            //CrestronConsole.PrintLine("cam got {0} - val:{1} ctr:{2}", _e.Name, _e.Data, _e.SData);
            switch (_e.Name)
            {
                case "ptz_preset":
                    string[] numbers = Regex.Split(_e.SData, @"\s+");
                    if (numbers.Length == 3)
                    {
                        positionPan = double.Parse(numbers[0]);
                        QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, "position pan", false, 0, numbers[0]));
                        positionTilt = double.Parse(numbers[1]);
                        QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, "position tilt", false, 0, numbers[1]));
                        positionZoom = double.Parse(numbers[2]);
                        QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, "position zoom", false, 0, numbers[2]));
                    }
                    break;
                case "setup_pan_speed":
                    this.speedPan = (int)_e.Data;
                    QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, _e.Name, false, (int)_e.Data, _e.SData));
                    break;
                case "setup_tilt_speed":
                    this.speedTilt = (int)_e.Data;
                    QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, _e.Name, false, (int)_e.Data, _e.SData));
                    break;
                case "setup_zoom_speed":
                    this.speedZoom = (int)_e.Data;
                    QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, _e.Name, false, (int)_e.Data, _e.SData));
                    break;
                case "aaaa_setup_snapshot_speed":
                    this.speedRecall = (int)_e.Data;
                    QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, _e.Name, false, (int)_e.Data, _e.SData));
                    break;
                case "focus_auto":
                    this.afAutoMode = Convert.ToBoolean(_e.Data);
                    QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, _e.Name, false, (int)_e.Data, _e.SData));
                    break;
                case "toggle_privacy":
                    this.privacyMode = Convert.ToBoolean(_e.Data);
                    QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, _e.Name, false, (int)_e.Data, _e.SData));
                    break;
                case "preset_home_load":
                    this.homeMode = Convert.ToBoolean(_e.Data);
                    QsysCamEvent(this, new QsysEventsArgs(eQscEventIds.CameraChange, _e.Name, false, (int)_e.Data, _e.SData));
                    break;
            }
        }

        public void StartPTZ(ePtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue camera = new ComponentSetValue();
            switch (type)
            {
                case ePtzTypes.Up:
                    camera.Name = "tilt_up";
                    camera.Value = 1;
                    break;
                case ePtzTypes.Down:
                    camera.Name = "tilt_down";
                    camera.Value = 1;
                    break;
                case ePtzTypes.Left:
                    camera.Name = "pan_left";
                    camera.Value = 1;
                    break;
                case ePtzTypes.Right:
                    camera.Name = "pan_right";
                    camera.Value = 1;
                    break;
                case ePtzTypes.ZoomIn:
                    camera.Name = "zoom_in";
                    camera.Value = 1;
                    break;
                case ePtzTypes.ZoomOut:
                    camera.Name = "zoom_out";
                    camera.Value = 1;
                    break;
                case ePtzTypes.FocusFar:
                    camera.Name = "focus_far";
                    camera.Value = 1;
                    break;
                case ePtzTypes.FocusNear:
                    camera.Name = "focus_near";
                    camera.Value = 1;
                    break;
                default:
                    break;
            }
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(camera);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void StopPTZ(ePtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue camera = new ComponentSetValue();
            switch (type)
            {
                case ePtzTypes.Up:
                    camera.Name = "tilt_up";
                    camera.Value = 0;
                    break;
                case ePtzTypes.Down:
                    camera.Name = "tilt_down";
                    camera.Value = 0;
                    break;
                case ePtzTypes.Left:
                    camera.Name = "pan_left";
                    camera.Value = 0;
                    break;
                case ePtzTypes.Right:
                    camera.Name = "pan_right";
                    camera.Value = 0;
                    break;
                case ePtzTypes.ZoomIn:
                    camera.Name = "zoom_in";
                    camera.Value = 0;
                    break;
                case ePtzTypes.ZoomOut:
                    camera.Name = "zoom_out";
                    camera.Value = 0;
                    break;
                case ePtzTypes.FocusFar:
                    camera.Name = "focus_far";
                    camera.Value = 0;
                    break;  
                case ePtzTypes.FocusNear:
                    camera.Name = "focus_near";
                    camera.Value = 0;
                    break;
                default:
                    break;
            }
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(camera);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void SetPanSpeed(double _value)
        {
            double newVal = clamp(_value, 0, 1);
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "setup_pan_speed", Value = newVal };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }
        
        public void SetTiltSpeed(double _value)
        {
            double newVal = clamp(_value, 0, 1);
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "setup_tilt_speed", Value = newVal };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }
       
        public void SetZoomSpeed(double _value)
        {
            double newVal = clamp(_value, 0, 1);
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "setup_zoom_speed", Value = newVal };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }
        
        public void SetRecallSpeed(double _value)
        {
            double newVal = clamp(_value, 0, 1);
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "aaaa_setup_snapshot_speed", Value = newVal };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void FocusAutoMode()
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "focus_auto", Value = 1 };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void SetPrivacyMode(bool _state)
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "toggle_privacy", Value = Convert.ToInt16(_state) };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void SavePrivacyPosition()
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "preset_private_save_trigger", Value = 1 };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void GoHome()
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "preset_home_load", Value = 1 };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void SaveHome()
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;
            ComponentSetValue setVal = new ComponentSetValue() { Name = "preset_home_save_trigger", Value = 1 };
            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(setVal);
            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }


        internal double scale(double _in, double _inMin, double _inMax, double _outMin, double _outMax)
        {
            double percentage = (_in - _inMin) / (_inMin - _inMax);
            return (percentage) * (_outMin - _outMax) + _outMin;
        }

        private double clamp(double _in, double _min, double _max)
        {
            double newVal;
            if (_in > _max)
                newVal = _max;
            else if (_in < _min)
                newVal = _min;
            else
                newVal = _in;
            return newVal;
        }
    }

    public enum ePtzTypes
    {
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4,
        ZoomIn = 5,
        ZoomOut = 6,
        FocusFar = 7,
        FocusNear = 8
    }
}


