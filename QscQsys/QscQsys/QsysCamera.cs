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
        public PrivacyChange onPrivacyChange { get; set; }

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
                Component component = new Component() { Name = cName, Controls = new List<ControlName>() { new ControlName() { Name = string.Format("toggle_privacy") } } };

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

            currentPrivacy = Convert.ToBoolean(e.Value);

            if (onPrivacyChange != null)
                onPrivacyChange(Convert.ToUInt16(e.Value));
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