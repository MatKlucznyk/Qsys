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
        private string cName;
        private bool registered;

        public event EventHandler<QsysEventsArgs> QsysRouterEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public int CurrentSelectedInput { set; get; }

        public QsysRouter(string Name, int size)
        {
            cName = Name;

            Component component = new Component();
            component.Name = Name;
            List<ControlName> names = new List<ControlName>();
            for (int i = 0; i < size; i++)
            {
                names.Add(new ControlName() {Name = string.Format("output_1_input_{0}_select", i + 1) });
            }

            component.Controls = names;

            if (QsysProcessor.RegisterComponent(component))
            {
                QsysProcessor.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(QsysRouter_OnNewEvent);

                registered = true;
            }
        }

        public void InputSelect(int input)
        {
            ComponentChange newInputSelectedChange = new ComponentChange();
            newInputSelectedChange.Params = new ComponentChangeParams();

            newInputSelectedChange.Params.Name = cName;

            ComponentSetValue inputSelected = new ComponentSetValue();
            inputSelected.Name = string.Format("output_1_input_{0}_select", input);

            inputSelected.Value = 1;

            newInputSelectedChange.Params.Controls = new List<ComponentSetValue>();
            newInputSelectedChange.Params.Controls.Add(inputSelected);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(newInputSelectedChange));
        }

        private void QsysRouter_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name.Contains("output") && e.Name.Contains("input") && e.Name.Contains("select") && e.Data == 1)
            {
                var split = e.Name.Split('_');

                CurrentSelectedInput = Convert.ToInt16(split[3]);

                QsysRouterEvent(this, new QsysEventsArgs(eQscEventIds.RouterInputSelected, cName, true, Convert.ToInt16(split[3]), "true"));
            }
        }
    }
}