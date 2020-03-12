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
        private string cName;

        public QsysCamera(string Name)
        {
            cName = Name;
        }

        public void StartPTZ(PtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = cName;

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

            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(camera);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void StopPTZ(PtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = cName;

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

            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(camera);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void RecallHome()
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = cName;

            ComponentSetValue camera = new ComponentSetValue();
            camera.Name = "preset_home_load";
            camera.Value = 1;

            cameraChange.Params.Controls = new List<ComponentSetValue>();
            cameraChange.Params.Controls.Add(camera);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(cameraChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
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