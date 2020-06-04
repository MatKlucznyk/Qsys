using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysRouter
    {
        public delegate void RouterInputChange(ushort input);
        public RouterInputChange newRouterInputChange { get; set; }

        private string cName;
        private string coreId;
        private bool registered;
        private int myOutput;

        public event EventHandler<QsysEventsArgs> QsysRouterEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public int CurrentSelectedInput { set; get; }

        public void Initialize(string coreId, string Name, int output)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

            cName = Name;
            this.coreId = coreId;
            myOutput = output;

            Component component = new Component()
            {
                Name = cName,
                Controls = new List<ControlName>() { new ControlName() { Name = string.Format("select_{0}", output) } }
            };

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
                    Controls = new List<ControlName>() { new ControlName() { Name = string.Format("select_{0}", myOutput) } }
                };

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        public void InputSelect(int input)
        {
            if (registered)
            {
                ComponentChange newInputSelectedChange = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("select_{0}", myOutput), Value = input } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newInputSelectedChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        private void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name.Contains(string.Format("select_{0}", myOutput)))
            {
                CurrentSelectedInput = Convert.ToInt16(e.Value);

                QsysRouterEvent(this, new QsysEventsArgs(eQscEventIds.RouterInputSelected, cName, Convert.ToBoolean(e.Value), Convert.ToInt16(e.Value), e.Value.ToString(), null));

                if (newRouterInputChange != null)
                    newRouterInputChange(Convert.ToUInt16(e.Value));
            }
        }
    }
}