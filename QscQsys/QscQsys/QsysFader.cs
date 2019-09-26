using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysFader
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
        private bool currentMute;
        public bool CurrentMute { get { return currentMute; } }
        private double volumeLevel;
        public double VolumeLevel { get { return volumeLevel; } }
        private double volumePosition;
        public double VolumePosition { get { return volumePosition; } }
        private string volumeString = "";
        public string VolumeString { get { return volumeString; } }
        private double rampTime;

        //Events
        public event EventHandler<QsysEventsArgs> QsysFaderEvent;
        

        /// <summary>
        /// Default constructor for a QsysFader
        /// </summary>
        /// <param name="Name">The component name of the gain.</param>
        public QsysFader(int _coreID, string _componentName)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);
            
            Component component = new Component();
            component.Name = this.componentName;
            List<ControlName> names = new List<ControlName>();
            names.Add(new ControlName { Name = "gain" });
            names.Add(new ControlName { Name = "mute" });
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
            if (_e.changeResult.Name == "gain")
            {
                if (this.volumeLevel != _e.changeResult.Value)
                {
                    this.volumeLevel = _e.changeResult.Value;
                    this.volumeString = _e.changeResult.String;
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, "[[VAL]]", false, _e.changeResult.Value, _e.changeResult.String));
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, "[[POS]]", false, _e.changeResult.Position, ""));
                }
            }
            else if (_e.changeResult.Name == "mute")
            {
                bool b = Convert.ToBoolean(_e.changeResult.Value);
                currentMute = b;
                QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, this.componentName, b, Convert.ToInt16(b), Convert.ToString(b)));
            }
        }





        public void SetPosition(double _position)
        {
            double newP = clamp(_position, 0.0, 1.0);
            ComponentChange newVolumeChange = new ComponentChange();
            newVolumeChange.Params = new ComponentChangeParams();
            newVolumeChange.Params.Name = this.componentName;
            ComponentSetValue volume = new ComponentSetValue { Name = "gain", Position = Math.Round(newP, 8), Ramp = rampTime };
            newVolumeChange.Params.Controls = new List<ComponentSetValue>();
            newVolumeChange.Params.Controls.Add(volume);

            string jsonIgnoreNullValues = JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings{ NullValueHandling = NullValueHandling.Ignore });
            this.myCore.Enqueue(jsonIgnoreNullValues);
        }


        public void SetVolume(double _value)
        {
            ComponentChange newVolumeChange = new ComponentChange();
            newVolumeChange.Params = new ComponentChangeParams();
            newVolumeChange.Params.Name = this.componentName;
            ComponentSetValue volume = new ComponentSetValue { Name = "gain", Value = Math.Round(_value, 8), Ramp = rampTime };
            newVolumeChange.Params.Controls = new List<ComponentSetValue>();
            newVolumeChange.Params.Controls.Add(volume);

            string jsonIgnoreNullValues = JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            this.myCore.Enqueue(jsonIgnoreNullValues);
        }


        public void SetMute(bool _value)
        {
            if (this.currentMute != _value)
            {
                ComponentChange newMuteChange = new ComponentChange();
                newMuteChange.Params = new ComponentChangeParams();
                newMuteChange.Params.Name = this.componentName;
                ComponentSetValue mute = new ComponentSetValue { Name = "mute", Value = Convert.ToDouble(_value) };
                newMuteChange.Params.Controls = new List<ComponentSetValue>();
                newMuteChange.Params.Controls.Add(mute);
                string jsonIgnoreNullValues = JsonConvert.SerializeObject(newMuteChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
                this.myCore.Enqueue(jsonIgnoreNullValues);
            }
        }


        public void ToggleMute()
        {
            this.currentMute = !this.currentMute;
            ComponentChange newMuteChange = new ComponentChange();
            newMuteChange.Params = new ComponentChangeParams();
            newMuteChange.Params.Name = this.componentName;
            ComponentSetValue mute = new ComponentSetValue { Name = "mute", Value = Convert.ToDouble(this.currentMute) };
            newMuteChange.Params.Controls = new List<ComponentSetValue>();
            newMuteChange.Params.Controls.Add(mute);
            string jsonIgnoreNullValues = JsonConvert.SerializeObject(newMuteChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            this.myCore.Enqueue(jsonIgnoreNullValues);
        }


        /// <summary>
        /// Sets the QSys ramp time for the gain
        /// </summary>
        /// <param name="time"></param>
        public void RampTimeMS(double time)
        {
            this.rampTime = time / 1000; //ms to sec
        }


        public double scale(double A, double A1, double A2, double Min, double Max)
        {
            double percentage = (A - A1) / (A1 - A2);
            return (percentage) * (Min - Max) + Min;
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
}