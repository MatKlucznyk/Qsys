using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysNamedControl
    {

        //Core
        private QsysCore myCore;

        //Named Control
        private string controlName;
        public string ControlName { get { return this.controlName; } }
        private bool registered;
        public bool IsRegistered { get { return this.registered; } }
        private eControlType controlType;
        public eControlType ControlType { get { return this.controlType; } }

        //Internal Vars
        private double val = 0;
        public double Val { get { return val; } }
        private double valScaled = 0;
        public double ValScaled { get { return valScaled; } }
        private double lastSentVal = 0;
        private string sVal = "";
        public string S_Val { get { return sVal; } }
        private bool bVal = false;
        public bool b_Val { get { return bVal; } }
        private double max;
        private double min;
        private double rampTime;

        //Event
        public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;


        /// <summary>
        /// Default constructor for a QsysNamedControl
        /// </summary>
        /// <param name="Name">The component name of the gain.</param>
        public QsysNamedControl(int _coreID, string _controlName, eControlType _controlType)
        {
            this.controlName = _controlName;
            this.controlType = _controlType;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);
            if (this.myCore.RegisterNamedControl(this.controlName))
            {
                this.myCore.Controls[_controlName].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Control_OnNewEvent);
                this.registered = true;
            }
        }

        void Control_OnNewEvent(object _sender, QsysInternalEventsArgs _e)
        {
            switch (this.controlType)
            {
                case eControlType.isIntegerValue:
                case eControlType.isFloatValue:
                    this.val = _e.Data;
                    this.valScaled = Math.Round(scale(val, this.min, this.max, 0, 65535));
                    this.sVal = _e.SData;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, "[[VAL]]", false, this.val, this.sVal));
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, "[[VAL-SCALED]]", false, this.valScaled, this.valScaled.ToString()));
                    break;
                case eControlType.isButton:
                    this.bVal = Convert.ToBoolean(_e.Data);
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, this.controlName, this.bVal, 0, ""));
                    break;
                case eControlType.isTrigger:
                    this.bVal = false;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, this.controlName, false, 0, ""));
                    break;
                case eControlType.isString:
                    this.sVal = _e.SData;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, this.controlName, false, 0, this.sVal));
                    break;
            }
        }


        public void SetValueScaled(double _value)
        {
            if (this.controlType != eControlType.isIntegerValue || this.controlType != eControlType.isFloatValue)
                return;
            double newRawVal = Math.Round(scale(_value, 0, 65535, this.min, this.max), 2);
            if (newRawVal == this.lastSentVal) //avoid repeats
                return;
            this.lastSentVal = newRawVal;

            ControlSetDouble newValChange = new ControlSetDouble();
            newValChange.Params = new ControlSetValueDouble();
            newValChange.method = "Control.Set";
            newValChange.Params.Name = this.controlName;
            newValChange.Params.Value = newRawVal;
            newValChange.Params.Ramp = this.rampTime;
            this.myCore.Enqueue(JsonConvert.SerializeObject(newValChange));
        }


        public void SetValueRaw(double _value)
        {
            if (this.controlType != eControlType.isIntegerValue || this.controlType != eControlType.isFloatValue)
                return;

            double newRawVal = _value;
            if (newRawVal > this.max && _value < this.min) //ensure within range
                return;
            if (this.lastSentVal == newRawVal) //avoid repeats
                return;
            this.lastSentVal = newRawVal;

            ControlSetDouble newValChange = new ControlSetDouble();
            newValChange.Params = new ControlSetValueDouble();
            newValChange.method = "Control.Set";
            newValChange.Params.Name = this.controlName;
            newValChange.Params.Value = newRawVal;
            newValChange.Params.Ramp = this.rampTime;
            this.myCore.Enqueue(JsonConvert.SerializeObject(newValChange));
        }

        public void SetState(bool _value)
        {
            if (ControlType != eControlType.isButton)
                return;

            ControlSetBool newStateChange = new ControlSetBool();
            newStateChange.Params = new ControlSetValueBool();
            newStateChange.method = "Control.Set";
            newStateChange.Params.Name = this.controlName;
            newStateChange.Params.Value = _value;
            this.myCore.Enqueue(JsonConvert.SerializeObject(newStateChange));
        }

        public void SetStateToggle()
        {
            if (ControlType != eControlType.isButton)
                return;

            this.bVal = !this.bVal;
            ControlSetBool newStateChange = new ControlSetBool();
            newStateChange.Params = new ControlSetValueBool();
            newStateChange.method = "Control.Set";
            newStateChange.Params.Name = this.controlName;
            newStateChange.Params.Value = this.bVal;
            this.myCore.Enqueue(JsonConvert.SerializeObject(newStateChange));
        }

        public void Trigger()
        {
            if (ControlType != eControlType.isTrigger)
                return;

            ControlSetBool newTriggerChange = new ControlSetBool();
            newTriggerChange.Params = new ControlSetValueBool();
            newTriggerChange.method = "Control.Set";
            newTriggerChange.Params.Name = this.controlName;
            newTriggerChange.Params.Value = true;
            this.myCore.Enqueue(JsonConvert.SerializeObject(newTriggerChange));
        }

        public void SetString(string _value)
        {
            ControlSetString newStringChange = new ControlSetString();
            newStringChange.Params = new ControlSetValueString();
            newStringChange.method = "Control.Set";
            newStringChange.Params.Name = this.controlName;
            newStringChange.Params.Value = _value;
            this.myCore.Enqueue(JsonConvert.SerializeObject(newStringChange));
        }

        /// <summary>
        /// Sets the QSys ramp time for the gain
        /// </summary>
        /// <param name="time"></param>
        public void RampTimeMS(double _time)
        {
            this.rampTime = _time / 1000; //ms to sec
        }

        public void SetMinMax(double _newMin, double _newMax)
        {
            this.min = _newMin;
            this.max = _newMax;
        }
        public void SetMinMaxViaString(string _newMin, string _newMax)
        {
            if (ControlType != eControlType.isIntegerValue || this.controlType != eControlType.isFloatValue)
                return;
            this.min = Convert.ToDouble(_newMin);
            this.max = Convert.ToDouble(_newMax);
        }

        private double scale(double A, double A1, double A2, double Min, double Max)
        {
            double percentage = (A - A1) / (A1 - A2);
            return (percentage) * (Min - Max) + Min;
        }

    }

    public enum eControlType
    {
        isIntegerValue = 0,
        isFloatValue = 1,
        isButton = 2,
        isTrigger = 3,
        isString = 4
    }
}