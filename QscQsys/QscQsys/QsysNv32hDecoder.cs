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

        //Core
        private QsysCore myCore;

        //Named Component
        private string componentName;
        public string ComponentName { get { return componentName; } }
        private bool registered;
        public bool IsRegistered { get { return registered; } }
        private bool isComponent;

        //Internal Vars
        private int currentSource;
        public int CurrentSource { get { return currentSource; } }

        //Events
        public event EventHandler<QsysEventsArgs> QsysNv32hDecoderEvent;



        public QsysNv32hDecoder(int _coreID, string _componentName)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);

            Component component = new Component();
            component.Name = this.componentName;
            List<ControlName> names = new List<ControlName>();
            names.Add(new ControlName());
            names[0].Name = "hdmi_out_0_select_index";
            component.Controls = names;

            if (this.myCore.RegisterNamedComponent(component))
            {
                this.myCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);
                this.registered = true;
                this.isComponent = true;
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            currentSource = Convert.ToInt16(e.changeResult.Value);

            QsysNv32hDecoderEvent(this, new QsysEventsArgs(eQscEventIds.Nv32hDecoderInputChange, this.componentName, Convert.ToBoolean(currentSource), currentSource, currentSource.ToString()));
        }

        public void ChangeInput(int source)
        {
            ComponentChange inputChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = this.componentName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = "hdmi_out_0_select_index", Value = source } } } };
            this.myCore.Enqueue(JsonConvert.SerializeObject(inputChange));
        }
    }
}