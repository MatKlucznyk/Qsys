using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysSignalPresence : QsysComponent
    {
        public delegate void SignalPresenceChange(SimplSharpString cName, ushort index, ushort value);
        public delegate void PeakThresholdChange(SimplSharpString cName, SimplSharpString value);
        public delegate void HoldTimeChange(SimplSharpString cName, SimplSharpString value);
        public delegate void InfiniteHoldChange(SimplSharpString cName, ushort value);
        public SignalPresenceChange newSignalPresenceChange { get; set; }
        public PeakThresholdChange newPeakThresholdChange { get; set; }
        public HoldTimeChange newHoldTimeChange { get; set; }
        public InfiniteHoldChange newInfiniteHoldChange { get; set; }

        private ushort _count;
        private ushort _threshold;
        private ushort _holdTime;
        private bool _infiniteHold;

        public void Initialize(string coreId, string componentName, ushort count)
        {
            _count = count;

            var component = new Component(true)
                {
                    Name = componentName,
                    Controls = new List<ControlName>() 
                    { 
                        new ControlName() { Name = "threshold" }, 
                        new ControlName() { Name = "hold_time" },
                        new ControlName() {Name = "infinite_hold"} 
                    }
                };

            for (int i = 1; i <= count; i++)
            {
                component.Controls.Add(new ControlName() { Name = string.Format("signal_presence_{0}", i) });
            }

            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == "threshold")
            {
                _threshold = (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                if (newPeakThresholdChange != null)
                    newPeakThresholdChange(_cName, e.SValue);
            }
            else if (e.Name == "hold_time")
            {
                _holdTime = (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                if (newHoldTimeChange != null)
                    newHoldTimeChange(_cName, e.SValue);
            }
            else if (e.Name == "infinite_hold")
            {
                _infiniteHold = Convert.ToBoolean(e.Value);

                if (newInfiniteHoldChange != null)
                    newInfiniteHoldChange(_cName, (ushort)e.Value);
            }
            else if(e.Name.Contains("signal_presence"))
            {
                if(e.Name.Contains("signal_presence_"))
                {
                    var splitName = e.Name.Split('_');

                    if (newSignalPresenceChange != null)
                        newSignalPresenceChange(_cName, Convert.ToUInt16(splitName[2]), (ushort)e.Value);
                }
                else
                {
                    if(newSignalPresenceChange != null)
                        newSignalPresenceChange(_cName, 1, (ushort)e.Value);
                }
            }
        }

        public void ThresholdIncrement()
        {
            if (_registered)
            {
                double newThreshold;

                if ((_threshold + 6553.5) <= 65535)
                {
                    newThreshold = QsysCoreManager.ScaleDown(_threshold + 6553.5);
                }
                else
                {
                    newThreshold = QsysCoreManager.ScaleDown(65535);
                }

                SendComponentChangePosition("threshold", newThreshold);
            }
        }

        public void ThresholdDecrement()
        {
            if (_registered)
            {
                double newThreshold;

                if ((_threshold - 6553.5) >= 0)
                {
                    newThreshold = QsysCoreManager.ScaleDown(_threshold - 6553.5);
                }
                else
                {
                    newThreshold = QsysCoreManager.ScaleDown(0);
                }

                SendComponentChangePosition("threshold", newThreshold);
            }
        }

        public void HoldTimeIncrement()
        {
            if (_registered)
            {
                double newHoldtime;

                if ((_holdTime + 6553.5) <= 65535)
                {
                    newHoldtime = QsysCoreManager.ScaleDown(_holdTime + 6553.5);
                }
                else
                {
                    newHoldtime = QsysCoreManager.ScaleDown(65535);
                }

                SendComponentChangePosition("hold_time", newHoldtime);
            }
        }

        public void HoldTimeDecrement()
        {
            if (_registered)
            {
                double newHoldtime;

                if ((_holdTime - 6553.5) >= 0)
                {
                    newHoldtime = QsysCoreManager.ScaleDown(_holdTime - 6553.5);
                }
                else
                {
                    newHoldtime = QsysCoreManager.ScaleDown(0);
                }

                SendComponentChangePosition("hold_time", newHoldtime);
            }
        }

        public void InfiniteHold(bool value)
        {
            if (_infiniteHold != value && _registered)
            {
                SendComponentChangeDoubleValue("infinite_hold", Convert.ToDouble(value));
            }
        }

        public void InfiniteHold(ushort value)
        {
            if (_registered)
            {
                switch (value)
                {
                    case (0):
                        this.InfiniteHold(false);
                        break;
                    case (1):
                        this.InfiniteHold(true);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}