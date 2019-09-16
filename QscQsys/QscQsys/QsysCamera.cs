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
        //Core
        private QsysCore myCore;

        //Named Component
        private string componentName;
        public string ComponentName { get { return componentName; } }
        private bool registered;
        public bool IsRegistered { get { return registered; } }
        private bool isComponent;

        //Internal Vars

        //Events
        public event EventHandler<QsysEventsArgs> QsysCameraEvent;

        public QsysCamera(int _coreID, string _componentName)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);

            //Component component = new Component();
            //component.Name = this.componentName;
            //List<ControlName> names = new List<ControlName>();
            //names.Add(new ControlName());
            //names.Add(new ControlName());
            //names[0].Name = "gain";
            //names[1].Name = "mute";
            //component.Controls = names;

            //if (this.myCore.RegisterNamedComponent(component))
            //{
            //    this.myCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);
            //    this.registered = true;
            //    this.isComponent = true;
            //}
        }

        void Component_OnNewEvent(object _sender, QsysInternalEventsArgs _e)
        {
            //if (_e.Name == "gain")
            //{
            //    if (_e.Data >= min && _e.Data <= max)
            //    {
            //        currentLvl = (int)Math.Round((65535 / (max - min)) * (_e.Data + (min * (-1))));
            //        currentLvlDb = _e.Data;
            //        lastSentLvl = -1;
            //        QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, this.componentName, true, this.currentLvl, this.currentLvl.ToString()));
            //    }
            //}
        }

        public void StartPTZ(PtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;

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

            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
        }

        public void StopPTZ(PtzTypes type)
        {
            ComponentChange cameraChange = new ComponentChange();
            cameraChange.Params = new ComponentChangeParams();
            cameraChange.Params.Name = this.componentName;

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

            this.myCore.Enqueue(JsonConvert.SerializeObject(cameraChange));
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