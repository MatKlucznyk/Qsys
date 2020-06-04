using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysFader
    {
        public delegate void VolumeChange(ushort value);
        public delegate void MuteChange(ushort value);
        public VolumeChange newVolumeChange { get; set; }
        public MuteChange newMuteChange { get; set; }

        private string cName;
        private bool registered;
        private bool currentMute;
        private int currentLvl;
        private double max;
        private double min;
        private string coreId;

        public event EventHandler<QsysEventsArgs> QsysFaderEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public bool CurrentMute { get { return currentMute; } }
        public int CurrentVolume { get { return currentLvl; } }

        /// <summary>
        /// Default constructor for a QsysFader
        /// </summary>
        /// <param name="Name">The component name of the gain.</param>
        public void Initialize(string coreId, string Name)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
            cName = Name;

            this.coreId = coreId;

            if (!registered)
                RegisterWithCore();
        }

        private void RegisterWithCore()
        {
            if (QsysCoreManager.Cores.ContainsKey(coreId))
            {
                Component component = new Component() { Name = cName, Controls = new List<ControlName>() { new ControlName() { Name = "gain" }, new ControlName() { Name = "mute" } } };

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        private void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                RegisterWithCore();
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == "gain")
            {
                /*if (e.Value >= min && e.Value <= max)
                {
                    currentLvl = (int)Math.Round((65535 / (max - min)) * (e.Value + (min * (-1))));
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, cName, true, currentLvl, currentLvl.ToString()));
                }*/

                currentLvl = (int)Math.Round(QsysCoreManager.ScaleUp(e.Position));
                QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, cName, true, currentLvl, currentLvl.ToString(), null));

                if (newVolumeChange != null)
                    newVolumeChange((ushort)currentLvl);
            }
            else if (e.Name == "mute")
            {
                if (e.Value == 1)
                {
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, cName, true, 1, "true", null));
                    currentMute = true;
                }
                else if (e.Value == 0)
                {
                    QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, cName, false, 0, "false", null));
                    currentMute = false;
                }

                if (newMuteChange != null)
                    newMuteChange((ushort)e.Value);
            }
            else if (e.Name == "max_gain")
            {
                max = e.Value;
            }
            else if (e.Name == "min_gain")
            {
                min = e.Value;
            }
        }

        private double newValue = -1;

        /// <summary>
        /// Sets the current volume.
        /// </summary>
        /// <param name="value">The volume level to set to.</param>
        public void Volume(int value)
        {
            if (registered)
            {
                ComponentChange newVolumeChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "gain", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Volume(ushort value)
        {
            this.Volume((int)value);
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(bool value)
        {
            if (currentMute != value && registered)
            {
                var intValue = Convert.ToInt16(value);

                ComponentChange newMuteChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "mute", Value = intValue } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newMuteChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void Mute(ushort value)
        {
            switch (value)
            {
                case (0):
                    this.Mute(false);
                    break;
                case (1):
                    this.Mute(true);
                    break;
                default:
                    break;
            }
        }
    }
}