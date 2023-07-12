using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysRouter : QsysComponent
    {
        public delegate void RouterInputChange(SimplSharpString cName, ushort input);
        public delegate void MuteChange(SimplSharpString cName, ushort value);
        public RouterInputChange newRouterInputChange { get; set; }
        public MuteChange newOutputMuteChange { get; set; }

        private int _output;
        private int _currentSelectedInput;
        private bool _currentMute;

        public int CurrentSelectedInput { get { return _currentSelectedInput; } }
        public int Output { get { return _output; } }
        public bool CurrentMute {get { return _currentMute; }}

        public void Initialize(string coreId, string componentName, int output)
        {
            _output = output;

            var component = new Component(true)
            {
                Name = componentName,
                Controls = new List<ControlName>() { new ControlName() { Name = string.Format("select_{0}", output)}, new ControlName() { Name = string.Format("mute_{0}", output)} }
            };

            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == string.Format("select_{0}", _output))
            {
                _currentSelectedInput = Convert.ToInt16(e.Value);

                if (newRouterInputChange != null)
                    newRouterInputChange(_cName, Convert.ToUInt16(e.Value));
            }
            else if (e.Name == string.Format("mute_{0}", _output))
            {
                if (e.Value == 1)
                {
                    _currentMute = true;
                } else if (e.Value == 0)
                {
                    _currentMute = false;
                }

                if (newOutputMuteChange != null)
                    newOutputMuteChange(_cName, (ushort)e.Value);
            }
        }

        public void InputSelect(int input)
        {
            if (_registered)
            {
                SendComponentChangeDoubleValue(string.Format("select_{0}", _output), input);
            }
        }

        public void OutputMute(bool value)
        {
            if (_registered)
            {
                SendComponentChangeDoubleValue(string.Format("mute_{0}", _output), Convert.ToDouble(value));
            }
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void OutputMute(ushort value)
        {
            OutputMute(Convert.ToBoolean(value));
        }
    }
}