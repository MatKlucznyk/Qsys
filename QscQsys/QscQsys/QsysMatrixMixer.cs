using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMatrixMixer : QsysComponent
    {
        private readonly object _crosspointsLock = new object();
        private readonly Dictionary<string, QsysMatrixMixerCrosspoint> _crosspoints = new Dictionary<string, QsysMatrixMixerCrosspoint>();

        public void Initialize(string coreId, string componentName)
        {
            var controls = new List<ControlName>();

            lock (_crosspointsLock)
            {
                foreach (var crosspoint in _crosspoints)
                {
                    controls.Add(new ControlName() { Name = string.Format("{0}mute", crosspoint.Value.CrosspointName) });
                    controls.Add(new ControlName() { Name = string.Format("{0}gain", crosspoint.Value.CrosspointName) });
                }
            }

            var component = new Component(true) { Name = componentName, Controls = controls };
            base.Initialize(coreId, component);
        }

        internal void RegisterCrosspoint(QsysMatrixMixerCrosspoint crosspoint)
        {
            lock (_crosspointsLock)
            {
                if(_crosspoints.ContainsKey(crosspoint.CrosspointName))
                    return;

                _crosspoints.Add(crosspoint.CrosspointName, crosspoint);
                AddControl(string.Format("{0}mute", crosspoint.CrosspointName));
                AddControl(string.Format("{0}gain", crosspoint.CrosspointName));
            }
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            var crosspoint = _crosspoints.SingleOrDefault(x => e.Name.Contains(x.Key));

            if (crosspoint.Equals(default(KeyValuePair<string, QsysMatrixMixerCrosspoint>)))
                return;

            crosspoint.Value.ComponentUpdate(e);
        }

        public void SetCrosspointMute(ushort input, ushort output, ushort value)
        {
            if (_registered)
            {
                SendComponentChangeDoubleValue(string.Format("input_{0}_output_{1}_mute", input, output), Convert.ToDouble(value));
            }
        }

        public void SetCrosspointGain(ushort input, ushort output, ushort value)
        {
            if (_registered)
            {
                SendComponentChangePosition(string.Format("input_{0}_output_{1}_gain", input, output), QsysCoreManager.ScaleDown(value));
            }
        }
    }
}