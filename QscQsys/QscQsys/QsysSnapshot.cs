using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysSnapshot
    {
        public delegate void RecalledSnapshot(ushort snapshot);
        public delegate void UnrecalledSnapshot(ushort snapshot);
        public RecalledSnapshot onRecalledSnapshot { get; set; }
        public UnrecalledSnapshot onUnrecalledSnapshot { get; set; }

        private string cName;
        private string coreId;
        private bool registered;

        public void Initialize(string coreId, string name)
        {
            this.cName = name;
            this.coreId = coreId;

            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

            if (!registered)
                RegisterWithCore();
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                RegisterWithCore();
            }
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
                        new ControlName() { Name = "load_1" }, 
                        new ControlName() { Name = "load_2" },
                        new ControlName() { Name = "load_3" },
                        new ControlName() { Name = "load_4" },
                        new ControlName() { Name = "load_5" },
                        new ControlName() { Name = "load_6" },
                        new ControlName() { Name = "load_7" },
                        new ControlName() { Name = "load_8" }
                    }
                };

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if(e.Name.Contains("load"))
            {
                var load = Convert.ToUInt16(e.Name.Split('_')[1]);

                if (e.Position == 1.0)
                {
                    if (onRecalledSnapshot != null)
                        onRecalledSnapshot(load);
                }
                else if (e.Position < 1.0)
                {
                    if (onUnrecalledSnapshot != null)
                        onUnrecalledSnapshot(load);
                }
            }
        }

        public void LoadSnapshot(ushort number)
        {
            if (registered)
            {
                ComponentChange loadSnapshot = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("load_{0}", number), Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(loadSnapshot, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SaveSnapshot(ushort number)
        {
            if (registered)
            {
                ComponentChange saveSnapshot = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("save_{0}", number), Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(saveSnapshot, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }
    }
}