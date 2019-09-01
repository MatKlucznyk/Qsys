using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysFader
    {
        private string cName;
        private bool registered;
        private bool currentMute;
        private int currentLvl;
        private int lastSentLvl;
        private bool isComponent;
        private double max;
        private double min;

        public event EventHandler<QsysEventsArgs> QsysFaderEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public bool CurrentMute { get { return currentMute; } }
        public int CurrentVolume { get { return currentLvl; } }
        public ushort RampTime { get; set; }

        /// <summary>
        /// Default constructor for a QsysFader
        /// </summary>
        /// <param name="Name">The component name of the gain.</param>
        public QsysFader(string Name)
        {
            cName = Name;

            Component component = new Component();
            component.Name = Name;
            List<ControlName> names = new List<ControlName>();
            names.Add(new ControlName());
            names.Add(new ControlName());
            names[0].Name = "gain";
            names[1].Name = "mute";


            component.Controls = names;

            if (QsysProcessor.RegisterComponent(component))
            {
                QsysProcessor.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                registered = true;
                isComponent = true;
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == "gain")
            {
                if (e.Data >= min && e.Data <= max)
                {
                    currentLvl = (int)Math.Round((65535 / (max - min)) * (e.Data + (min * (-1))));
                    lastSentLvl = -1;
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, cName, true, currentLvl, currentLvl.ToString()));
                }
            }
            else if (e.Name == "mute")
            {
                if (e.Data == 1)
                {
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, cName, true, 1, "true"));
                    currentMute = true;
                }
                else if (e.Data == 0)
                {
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, cName, false, 0, "false"));
                    currentMute = false;
                }
            }
            else if (e.Name == "max_gain")
            {
                max = e.Data;
            }
            else if (e.Name == "min_gain")
            {
                min = e.Data;
            }
        }

        /// <summary>
        /// Sets the current volume.
        /// </summary>
        /// <param name="value">The volume level to set to.</param>
        public void Volume(int value)
        {
            double newVal = Math.Round((value / (65535 / (max - min))) + min);
            if (lastSentLvl == newVal || newVal == currentLvl) //avoid repeats
                return;
            lastSentLvl = (int)newVal;

            ComponentChange newVolumeChange = new ComponentChange();
            newVolumeChange.Params = new ComponentChangeParams();
            newVolumeChange.Params.Name = cName;
            
            ComponentSetValue volume = new ComponentSetValue();

            volume.Name = "gain";
            volume.Value = newVal;
            volume.Ramp = RampTime;

            newVolumeChange.Params.Controls = new List<ComponentSetValue>();
            newVolumeChange.Params.Controls.Add(volume);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(newVolumeChange));
            CrestronConsole.PrintLine("Setting new level to: {0}", volume.Value);
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(bool value)
        {
            if (currentMute != value)
            {
                ComponentChange newMuteChange = new ComponentChange();
                newMuteChange.Params = new ComponentChangeParams();

                newMuteChange.Params.Name = cName;

                ComponentSetValue mute = new ComponentSetValue();
                mute.Name = "mute";

                if (value)
                    mute.Value = 1;
                else
                    mute.Value = 0;

                newMuteChange.Params.Controls = new List<ComponentSetValue>();
                newMuteChange.Params.Controls.Add(mute);

                QsysProcessor.Enqueue(JsonConvert.SerializeObject(newMuteChange));
            }
        }
    }
}