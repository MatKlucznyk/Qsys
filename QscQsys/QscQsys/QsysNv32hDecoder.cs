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
        private string cName;
        private bool registered;
        private int currentSource;
        private bool isComponent;

        public event EventHandler<QsysEventsArgs> QsysNv32hDecoderEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public int CurrentSource { get { return currentSource; } }

        public QsysNv32hDecoder(string Name)
        {
            cName = Name;

            Component component = new Component();
            component.Name = Name;
            List<ControlName> names = new List<ControlName>();
            names.Add(new ControlName());
            names[0].Name = "hdmi_out_0_select_index";

            component.Controls = names;

            if (QsysCore.RegisterComponent(component))
            {
                QsysCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                registered = true;
                isComponent = true;
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            currentSource = Convert.ToInt16(e.Data);

            QsysNv32hDecoderEvent(this, new QsysEventsArgs(eQscEventIds.Nv32hDecoderInputChange, cName, Convert.ToBoolean(currentSource), currentSource, currentSource.ToString()));
        }

        public void ChangeInput(int source)
        {
            ComponentChange inputChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "hdmi_out_0_select_index", Value = source } } } };

            QsysCore.Enqueue(JsonConvert.SerializeObject(inputChange));
        }
    }
}