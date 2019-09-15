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
        private int currentLvl;
        public int CurrentVolume { get { return currentLvl; } }
        private double currentLvlDb;
        public double CurrentVolumeDb { get { return currentLvlDb; } }
        private int lastSentLvl;
        private double max;
        private double min;
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
            names.Add(new ControlName());
            names.Add(new ControlName());
            names[0].Name = "gain";
            names[1].Name = "mute";
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
            if (_e.Name == "gain")
            {
                if (_e.Data >= min && _e.Data <= max)
                {
                    currentLvl = (int)Math.Round((65535 / (max - min)) * (_e.Data + (min * (-1))));
                    currentLvlDb = _e.Data;
                    lastSentLvl = -1;
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, this.componentName, true, this.currentLvl, this.currentLvl.ToString()));
                }
            }
            else if (_e.Name == "mute")
            {
                if (_e.Data == 1)
                {
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, this.componentName, true, 1, "true"));
                    currentMute = true;
                }
                else if (_e.Data == 0)
                {
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, this.componentName, false, 0, "false"));
                    currentMute = false;
                }
            }
            else if (_e.Name == "max_gain")
            {
                this.max = _e.Data;
            }
            else if (_e.Name == "min_gain")
            {
                this.min = _e.Data;
            }
        }

        /// <summary>
        /// Sets the current volume.
        /// </summary>
        /// <param name="value">The volume level to set to.</param>
        public void Volume(int _value)
        {
            double newVal = Math.Round((_value / (65535 / (this.max - this.min))) + this.min);
            if (this.lastSentLvl == newVal || newVal == this.currentLvl) //avoid repeats
                return;
            this.lastSentLvl = (int)newVal;

            ComponentChange newVolumeChange = new ComponentChange();
            newVolumeChange.Params = new ComponentChangeParams();
            newVolumeChange.Params.Name = this.componentName;

            ComponentSetValue volume = new ComponentSetValue();

            volume.Name = "gain";
            volume.Value = newVal;
            volume.Ramp = rampTime;

            newVolumeChange.Params.Controls = new List<ComponentSetValue>();
            newVolumeChange.Params.Controls.Add(volume);

            this.myCore.Enqueue(JsonConvert.SerializeObject(newVolumeChange));
        }

        /// <summary>
        /// Sets the current volume in DB.
        /// </summary>
        /// <param name="value">The volume level to set to.</param>
        public void VolumeDb(int _value)
        {
            double newVal = _value;
            if (newVal > this.max && _value < this.min) //ensure within range
                return;
            if (this.lastSentLvl == newVal || newVal == this.currentLvl) //avoid repeats
                return;
            this.lastSentLvl = (int)newVal;

            ComponentChange newVolumeChange = new ComponentChange();
            newVolumeChange.Params = new ComponentChangeParams();
            newVolumeChange.Params.Name = this.componentName;

            ComponentSetValue volume = new ComponentSetValue();

            volume.Name = "gain";
            volume.Value = newVal;
            volume.Ramp = rampTime;

            newVolumeChange.Params.Controls = new List<ComponentSetValue>();
            newVolumeChange.Params.Controls.Add(volume);

            this.myCore.Enqueue(JsonConvert.SerializeObject(newVolumeChange));
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(bool _value)
        {
            if (this.currentMute != _value)
            {
                ComponentChange newMuteChange = new ComponentChange();
                newMuteChange.Params = new ComponentChangeParams();

                newMuteChange.Params.Name = this.componentName;

                ComponentSetValue mute = new ComponentSetValue();
                mute.Name = "mute";

                if (_value)
                    mute.Value = 1;
                else
                    mute.Value = 0;

                newMuteChange.Params.Controls = new List<ComponentSetValue>();
                newMuteChange.Params.Controls.Add(mute);

                this.myCore.Enqueue(JsonConvert.SerializeObject(newMuteChange));
            }
        }

        /// <summary>
        /// Sets the QSys ramp time for the gain
        /// </summary>
        /// <param name="time"></param>
        public void RampTimeMS(double time)
        {
            this.rampTime = time / 1000; //ms to sec
        }


    }
}