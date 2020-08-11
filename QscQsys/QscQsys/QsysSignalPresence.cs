using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysSignalPresence
    {
        public delegate void SignalPresenceChange(ushort index, ushort value);
        public delegate void PeakThresholdChange(SimplSharpString value);
        public delegate void HoldTimeChange(SimplSharpString value);
        public delegate void InfiniteHoldChange(ushort value);
        public SignalPresenceChange newSignalPresenceChange { get; set; }
        public PeakThresholdChange newPeakThresholdChange { get; set; }
        public HoldTimeChange newHoldTimeChange { get; set; }
        public InfiniteHoldChange newInfiniteHoldChange { get; set; }

        private string cName;
        private bool registered;
        private string coreId;
        private ushort count;
        private ushort threshold;
        private ushort holdTime;
        private bool infiniteHold;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }

        public void Initialize(string coreId, string Name, ushort Count)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
            cName = Name;
            count = Count;

            this.coreId = coreId;

            if (!registered)
                RegisterWithCore();
        }

        private void RegisterWithCore()
        {
            if (QsysCoreManager.Cores.ContainsKey(coreId))
            {
                Component component = new Component()
                {
                    Name = cName,
                    Controls = new List<ControlName>() 
                    { 
                        new ControlName() { Name = "threshold" }, 
                        new ControlName() { Name = "hold_time" },
                        new ControlName() {Name = "infinite_hold"} 
                    }
                };

                if (count > 1)
                {
                    for (int i = 1; i <= count; i++)
                    {
                        component.Controls.Add(new ControlName() { Name = string.Format("signal_presence_{0}", i) });
                    }
                }
                else
                {
                    component.Controls.Add(new ControlName() { Name = "signal_presence" });
                }

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(QsysSignalPresence_OnNewEvent);

                    registered = true;
                }
            }
        } 

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                RegisterWithCore();
            }
        }

        void QsysSignalPresence_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == "threshold")
            {
                threshold = (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                if (newPeakThresholdChange != null)
                    newPeakThresholdChange(e.SValue);
            }
            else if (e.Name == "hold_time")
            {
                holdTime = (ushort)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                if (newHoldTimeChange != null)
                    newHoldTimeChange(e.SValue);
            }
            else if (e.Name == "infinite_hold")
            {
                infiniteHold = Convert.ToBoolean(e.Value);

                if (newInfiniteHoldChange != null)
                    newInfiniteHoldChange((ushort)e.Value);
            }
            else if(e.Name.Contains("signal_presence"))
            {
                if(e.Name.Contains("signal_presence_"))
                {
                    var splitName = e.Name.Split('_');

                    if (newSignalPresenceChange != null)
                        newSignalPresenceChange(Convert.ToUInt16(splitName[2]), (ushort)e.Value);
                }
                else
                {
                    if(newSignalPresenceChange != null)
                        newSignalPresenceChange(1, (ushort)e.Value);
                }
            }
        }

        public void ThresholdIncrement()
        {
            if (registered)
            {
                double newThreshold;

                if ((threshold + 6553.5) <= 65535)
                {
                    newThreshold = QsysCoreManager.ScaleDown(threshold + 6553.5);
                }
                else
                {
                    newThreshold = QsysCoreManager.ScaleDown(65535);
                }

                ComponentChange newChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "threshold", Position = newThreshold } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void ThresholdDecrement()
        {
            if (registered)
            {
                double newThreshold;

                if ((threshold - 6553.5) >= 0)
                {
                    newThreshold = QsysCoreManager.ScaleDown(threshold - 6553.5);
                }
                else
                {
                    newThreshold = QsysCoreManager.ScaleDown(0);
                }

                ComponentChange newChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "threshold", Position = newThreshold } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void HoldTimeIncrement()
        {
            if (registered)
            {
                double newHoldtime;

                if ((holdTime + 6553.5) <= 65535)
                {
                    newHoldtime = QsysCoreManager.ScaleDown(holdTime + 6553.5);
                }
                else
                {
                    newHoldtime = QsysCoreManager.ScaleDown(65535);
                }

                ComponentChange newChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "hold_time", Position = newHoldtime } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void HoldTimeDecrement()
        {
            if (registered)
            {
                double newHoldtime;

                if ((holdTime - 6553.5) >= 0)
                {
                    newHoldtime = QsysCoreManager.ScaleDown(holdTime - 6553.5);
                }
                else
                {
                    newHoldtime = QsysCoreManager.ScaleDown(0);
                }

                ComponentChange newChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "hold_time", Position = newHoldtime } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void InfiniteHold(bool value)
        {
            if (infiniteHold != value && registered)
            {
                var intValue = Convert.ToInt16(value);

                ComponentChange newChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "infinite_hold", Value = intValue } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void InfiniteHold(ushort value)
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