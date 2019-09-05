using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysNamedControl
    {
        private string cName;
        private bool registered;
        private double max;
        private double min;

        public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }

        public QsysNamedControl(string Name, int Max, int Min)
        {
            cName = Name;
            max = Max;
            min = Min;

            Control control = new Control();

            control.Name = Name;

            if (QsysProcessor.RegisterControl(control))
            {
                QsysProcessor.Controls[control].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Control_OnNewEvent);

                registered = true;
            }
        }

        //add event handling
        private void Control_OnNewEvent(object o, QsysInternalEventsArgs e)
        {
            int intValue;

            if (max > 0)
            {
                intValue = (int)Math.Round((65535 / (max - min)) * (e.Data + (min * (-1))));
            }
            else
            {
                intValue = (int)e.Data;
            }

            QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(e.Data), intValue, e.SData));
        }

        public void SetInteger(int value)
        {
            double newValue = Math.Round((value / (65535 / (max - min))) + min);

            ControlIntegerChange integer = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = cName, Value = Convert.ToInt16(newValue) } };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(integer));
        }

        public void SetString(string value)
        {
            ControlStringChange str = new ControlStringChange() { Params = new ControlStringParams() { Name = cName, Value = value } };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(str));
        }

        public void SetBoolean(int value)
        {
            ControlIntegerChange boolean = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = cName, Value = value } };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(boolean));
        }
    }
}