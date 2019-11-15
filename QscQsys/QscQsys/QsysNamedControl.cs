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

        public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public ushort IsInteger { get; set; }

        public QsysNamedControl(string Name)
        {
            cName = Name;

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

            if (IsInteger == 0)
            {
                QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(e.Value), Convert.ToUInt16(e.Value), e.SValue, null));
            }
            else
            {
                intValue = (int)Math.Round(QsysProcessor.ScaleUp(e.Position));

                QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(intValue), intValue, Convert.ToString(e.Position), null));
            }
        }

        public void SetInteger(int value)
        {
            double newValue = QsysProcessor.ScaleDown(value);

            ControlIntegerChange integer = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = cName, Position = newValue } };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(integer, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void SetString(string value)
        {
            ControlStringChange str = new ControlStringChange() { Params = new ControlStringParams() { Name = cName, Value = value } };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(str));
        }

        public void SetBoolean(int value)
        {
            ControlIntegerChange boolean = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = cName, Value = value } };

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(boolean, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}