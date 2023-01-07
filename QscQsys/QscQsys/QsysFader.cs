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
        public delegate void GainStringChange(SimplSharpString cName, SimplSharpString value);
        public VolumeChange newVolumeChange { get; set; }
        public MuteChange newMuteChange { get; set; }
        public GainStringChange newGainStringChange { get; set; }

        private bool _currentMute;
        private string _currentGainString;
        private int _currentLvl;

        public bool MuteValue { get { return _currentMute; } }
        public int VolumeValue { get { return _currentLvl; } }
        public string GainValue { get { return _currentGainString; } }

        public void Initialize(string coreId, string componentName)
        {
            var component = new Component(true) { Name = componentName, Controls = new List<ControlName>() { new ControlName() { Name = "gain" }, new ControlName() { Name = "mute" } } };
            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == "gain")
            {
                if (e.Type == "position" || e.Type == "change")
                {
                    _currentLvl = (int)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                    if (newVolumeChange != null)
                        newVolumeChange(_cName, (ushort)_currentLvl);
                }

                if (e.Type == "value" || e.Type == "change")
                {
                    _currentGainString = e.SValue;

                    if (newGainStringChange != null && e.SValue.Length > 0)
                        newGainStringChange(_cName, _currentGainString);
                }
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
                SendComponentChangePosition("gain", QsysCoreManager.ScaleDown(value));
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

        public void Decibels(double value)
        {
            if (_registered)
            {
                SendComponentChangeDoubleValue("gain", value);
            }
        }

        public void Decibels(short value)
        {
            this.Decibels((double)value);
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(bool value)
        {
            if (_registered)
            {
                SendComponentChangeDoubleValue("mute", Convert.ToDouble(value));
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