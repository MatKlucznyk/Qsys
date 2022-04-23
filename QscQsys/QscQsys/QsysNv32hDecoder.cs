using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysNv32hDecoder
    {
        public delegate void Nv32hDecoderInputChange(SimplSharpString cName, ushort input);
        public Nv32hDecoderInputChange newNv32hDecoderInputChange { get; set; }

        private string cName;
        private string coreId;
        private bool registered;
        private int currentSource;

        //public event EventHandler<QsysEventsArgs> QsysNv32hDecoderEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public int CurrentSource { get { return currentSource; } }

        public void Initialize(string coreId, string Name)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
            cName = Name;
            this.coreId = coreId;

            if(!registered)
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
                Component component = new Component(){Name = cName, Controls = new List<ControlName>(){new ControlName(){Name = "hdmi_out_0_select_index"}}};

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            currentSource = Convert.ToInt16(e.Value);

            //QsysNv32hDecoderEvent(this, new QsysEventsArgs(eQscEventIds.Nv32hDecoderInputChange, cName, Convert.ToBoolean(currentSource), currentSource, currentSource.ToString(), null));

            if (newNv32hDecoderInputChange != null)
                newNv32hDecoderInputChange(cName, Convert.ToUInt16(currentSource));
        }

        public void ChangeInput(int source)
        {
            if (registered)
            {
                ComponentChange inputChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "hdmi_out_0_select_index", Value = source } } } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(inputChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }
    }
}