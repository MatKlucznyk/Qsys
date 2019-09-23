using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysCameraSimpl
    {

        public delegate void PositionChange(SimplSharpString pos, ushort value);
        public PositionChange newPositionChange { get; set; }
        public delegate void AutoFocusChange(ushort state);
        public AutoFocusChange newAutoFocusChange { get; set; }
        public delegate void HomeChange(ushort state);
        public HomeChange newHomeChange { get; set; }
        public delegate void PrivacyChange(ushort state);
        public PrivacyChange newPrivacyChange { get; set; }

        private QsysCamera cam;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent)
        {
            this.cam = new QsysCamera((int)_coreID, _namedComponent.ToString());
            this.cam.QsysCamEvent += new EventHandler<QsysEventsArgs>(cam_QsysFaderEvent);
        }

        public void MoveUp(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.Up);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.Up);
                    break;
            }
        }
        public void MoveDown(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.Down);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.Down);
                    break;
            }
        }
        public void MoveLeft(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.Left);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.Left);
                    break;
            }
        }
        public void MoveRight(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.Right);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.Right);
                    break;
            }
        }
        public void ZoomIn(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.ZoomIn);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.ZoomIn);
                    break;
            }
        }
        public void ZoomOut(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.ZoomOut);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.ZoomOut);
                    break;
            }
        }
        public void FocusFar(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.FocusFar);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.FocusFar);
                    break;
            }
        }
        public void FocusNear(ushort _state)
        {
            switch (_state)
            {
                case 0:
                    this.cam.StopPTZ(ePtzTypes.FocusNear);
                    break;
                case 1:
                    this.cam.StartPTZ(ePtzTypes.FocusNear);
                    break;
            }
        }

        public void SetPanSpeed(ushort _value)
        {
            this.cam.SetPanSpeed(this.cam.scale(_value, 0, 65535, 0.0, 1.0));
        }
        public void SetTiltSpeed(ushort _value)
        {
            this.cam.SetTiltSpeed(this.cam.scale(_value, 0, 65535, 0.0, 1.0));
        }
        public void SetZoomSpeed(ushort _value)
        {
            this.cam.SetZoomSpeed(this.cam.scale(_value, 0, 65535, 0.0, 1.0));
        }
        public void SetRecallSpeed(ushort _value)
        {
            this.cam.SetRecallSpeed(this.cam.scale(_value, 0, 65535, 0.0, 1.0));
        }

        public void FocusAutoMode()
        {
            this.cam.FocusAutoMode();
        }

        public void SetPrivacyMode(ushort _state)
        {
            this.cam.SetPrivacyMode(Convert.ToBoolean(_state));
        }
        public void TogglePrivacyMode()
        {
            this.cam.SetPrivacyMode(!this.cam.PrivacyMode);
        }

        public void SavePrivacyPosition()
        {
            this.cam.SavePrivacyPosition();
        }

        public void GoHome()
        {
            this.cam.GoHome();
        }

        public void SaveHome()
        {
            this.cam.SaveHome();
        }

        
        private void cam_QsysFaderEvent(object _sender, QsysEventsArgs _e)
        {
            switch (_e.ControlName)
            {
                case "position pan":
                    if (newPositionChange != null)
                        newPositionChange((SimplSharpString)"pan", (ushort)this.cam.scale(this.cam.PositionPan, -1.0, 1.0, 0, 65535));
                    break;
                case "position tilt":
                    if (newPositionChange != null)
                        newPositionChange((SimplSharpString)"tilt", (ushort)this.cam.scale(this.cam.PositionTilt, -1.0, 3.0, 0, 65535));
                    break;
                case "position zoom":
                    if (newPositionChange != null)
                        newPositionChange((SimplSharpString)"zoom", (ushort)this.cam.scale(this.cam.PositionZoom, -1.0, 1.0, 0, 65535));
                    break;
                case "setup_pan_speed":

                    break;
                case "setup_tilt_speed":

                    break;
                case "setup_zoom_speed":

                    break;
                case "aaaa_setup_snapshot_speed":

                    break;
                case "focus_auto":
                    if (newAutoFocusChange != null)
                        newAutoFocusChange((ushort)_e.NumberValue);
                    break;
                case "toggle_privacy":
                    if (newPrivacyChange != null)
                        newPrivacyChange((ushort)_e.NumberValue);
                    break;
                case "preset_home_load":
                    if (newHomeChange != null)
                        newHomeChange((ushort)_e.NumberValue);
                    break;
            }
        }
    }
}