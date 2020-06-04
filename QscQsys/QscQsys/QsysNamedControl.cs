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
        public delegate void NamedControlChange(ushort intData, SimplSharpString stringData);
        public NamedControlChange newNamedControlChange { get; set; }

        private string cName;
        private string coreId;
        private bool registered;

        public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public ushort IsInteger { get; set; }

        public void Initialize(string coreId, string Name)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
            cName = Name;
            this.coreId = coreId;

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
                Control control = new Control() { Name = cName };

                if (QsysCoreManager.Cores[coreId].RegisterControl(control))
                {
                    QsysCoreManager.Cores[coreId].Controls[control].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Control_OnNewEvent);

                    registered = true;
                }
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
                intValue = (int)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(intValue), intValue, Convert.ToString(e.Position), null));
            }

            if (newNamedControlChange != null)
                newNamedControlChange(Convert.ToUInt16(e.Value), e.SValue);
        }

        public void SetInteger(int value)
        {
            if (registered)
            {
                double newValue = QsysCoreManager.ScaleDown(value);

                ControlIntegerChange integer = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = cName, Position = newValue } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(integer, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SetString(string value)
        {
            if(registered)
            {
            ControlStringChange str = new ControlStringChange() { Params = new ControlStringParams() { Name = cName, Value = value } };

            QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(str));
            }
        }

        public void SetBoolean(int value)
        {
            if (registered)
            {
                ControlIntegerChange boolean = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = cName, Value = value } };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(boolean, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }
    }
}