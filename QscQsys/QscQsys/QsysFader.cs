using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysFader : QsysComponent
    {
        public delegate void VolumeChange(SimplSharpString cName, ushort value);
        public delegate void MuteChange(SimplSharpString cName, ushort value);
        public VolumeChange newVolumeChange { get; set; }
        public MuteChange newMuteChange { get; set; }

        private bool _currentMute;
        private int _currentLvl;

        public bool MuteValue { get { return _currentMute; } }
        public int VolumeValue { get { return _currentLvl; } }

        public void Initialize(string coreId, string componentName)
        {
            var component = new Component() { Name = componentName, Controls = new List<ControlName>() { new ControlName() { Name = "gain" }, new ControlName() { Name = "mute" } } };
            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == "gain")
            {
                _currentLvl = (int)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                if (newVolumeChange != null)
                    newVolumeChange(_cName, (ushort)_currentLvl);
            }
            else if (e.Name == "mute")
            {
                if (e.Value == 1)
                {
                    _currentMute = true;
                }
                else if (e.Value == 0)
                {
                    _currentMute = false;
                }

                if (newMuteChange != null)
                    newMuteChange(_cName, (ushort)e.Value);
            }
        }

        /// <summary>
        /// Sets the current volume.
        /// </summary>
        /// <param name="value">The volume level to set to.</param>
        public void Volume(int value)
        {
            if (_registered)
            {
                ComponentChange newVolumeChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "gain", Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(newVolumeChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        /// <summary>
        /// Sets the current volume value.
        /// </summary>
        /// <param name="value">Volume value to set the fader to.</param>
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
            if (_currentMute != value && _registered)
            {
                var intValue = Convert.ToInt16(value);

                ComponentChange newMuteChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "mute", Value = intValue } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(newMuteChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(ushort value)
        {
            Mute(Convert.ToBoolean(value));
        }
    }
}