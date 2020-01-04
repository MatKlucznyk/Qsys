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
        private double controlValue = 0;
        public double ControlValue { get { return controlValue; } }
        private string controlString = "";
        public string ControlString { get { return controlString; } }
        private bool controlBool = false;
        public bool ControlBool { get { return controlBool; } }
        private double controlPosition = 0;
        public double ControlPosition { get { return controlPosition; } }
        private string controlColor = "";
        public string ControlColor { get { return controlColor; } }
        private bool controlIndeterminate = false;
        public bool ControlIndeterminate { get { return controlIndeterminate; } }
        private bool controlInvisible = false;
        public bool ControlInvisible { get { return controlInvisible; } }
        private bool controlDisabled = false;
        public bool ControlDisabled { get { return controlDisabled; } }
        //Legend
        //Choices
        private double lastSentVal = 0;
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

            if (_e.changeResult.String != null)
                this.controlString = _e.changeResult.String;

            if (_e.changeResult.Value != null)
                this.controlValue = _e.changeResult.Value;

            if (_e.changeResult.Position != null)
                this.controlPosition = _e.changeResult.Position;

            //if (_e.changeResult.Color != null)
            //    ; //later

            if (_e.changeResult.Indeterminate != null)
                this.controlIndeterminate = _e.changeResult.Indeterminate;

            if (_e.changeResult.Invisible != null)
                this.controlInvisible = _e.changeResult.Invisible;
            
            if (_e.changeResult.Disabled != null)
                this.controlDisabled = _e.changeResult.Disabled;
            
            //legend
            //changes

            switch (this.controlType)
            {
                case eControlType.isIntegerValue:
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, "[[VAL]]", false, this.controlValue, this.controlString));
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, "[[POS]]", false, this.controlPosition, ""));
                    break;
                case eControlType.isFloatValue:
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, "[[VAL]]", false, this.controlValue, this.controlString));
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, "[[POS]]", false, this.controlPosition, ""));
                    break;
                case eControlType.isButton:
                    this.controlBool = Convert.ToBoolean(this.controlValue);
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, this.controlName, this.controlBool, Convert.ToInt16(this.controlBool), Convert.ToString(this.controlBool)));
                    break;
                case eControlType.isTrigger:
                    this.controlBool = false;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, this.controlName, false, 0, ""));
                    break;
                case eControlType.isString:
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, this.controlName, false, 0, this.controlString));
                    break;
            }
        }


        public void SetPosition(double _position)
        {
            if (this.controlType == eControlType.isIntegerValue || this.controlType == eControlType.isFloatValue)
            {
                double p = clamp(_position, 0.0, 1.0);
                ControlSetValue cs = new ControlSetValue();
                cs.method = "Control.Set";
                cs.Params = new ControlSetValueParams { Name = this.controlName, Position = Math.Round(p, 8), Ramp = this.rampTime, typePos = true };
                this.myCore.Enqueue(JsonConvert.SerializeObject(cs));
            }
        }


        public void SetValue(double _value)
        {
            if (this.controlType == eControlType.isIntegerValue || this.controlType == eControlType.isFloatValue)
            {
                ControlSetValue cs = new ControlSetValue();
                cs.method = "Control.Set";
                cs.Params = new ControlSetValueParams { Name = this.controlName, Value = Math.Round(_value, 8).ToString(), Ramp = this.rampTime };
                CrestronConsole.PrintLine(String.Format("setting val to {0}", _value.ToString()));
                this.myCore.Enqueue(JsonConvert.SerializeObject(cs));
            }
        }

        public void SetState(bool _value)
        {
            if (this.ControlType == eControlType.isButton)
            {
                ControlSetValue cs = new ControlSetValue();
                cs.method = "Control.Set";
                cs.Params = new ControlSetValueParams { Name = this.controlName, Value = _value ? "1":"0" };
                this.myCore.Enqueue(JsonConvert.SerializeObject(cs));
            }
        }

        public void SetStateToggle()
        {
            if (ControlType == eControlType.isButton)
            {
                this.controlBool = !this.controlBool;
                ControlSetValue cs = new ControlSetValue();
                cs.method = "Control.Set";
                cs.Params = new ControlSetValueParams { Name = this.controlName, Value = this.controlBool ? "1":"0" };
                this.myCore.Enqueue(JsonConvert.SerializeObject(cs));
            }
        }

        public void Trigger()
        {
            if (ControlType == eControlType.isTrigger)
            {
                ControlSetValue cs = new ControlSetValue();
                cs.method = "Control.Set";
                cs.Params = new ControlSetValueParams { Name = this.controlName, Value = "1" };
                this.myCore.Enqueue(JsonConvert.SerializeObject(cs));
            }
        }

        public void SetString(string _value)
        {
            ControlSetValue cs = new ControlSetValue();
            cs.method = "Control.Set";
            cs.Params = new ControlSetValueParams { Name = this.controlName, Value = _value };
            this.myCore.Enqueue(JsonConvert.SerializeObject(cs));
        }

        /// <summary>
        /// Sets the QSys ramp time for the gain
        /// </summary>
        /// <param name="time"></param>
        public void RampTimeMS(double _time)
        {
            this.rampTime = _time / 1000; //ms to sec
        }


        public double scale(double A, double A1, double A2, double Min, double Max)
        {
            double percentage = (A - A1) / (A1 - A2);
            return (percentage) * (Min - Max) + Min;
        }

        private double clamp(double _in, double _min, double _max)
        {
            double newVal;
            if (_in > _max)
                newVal = _max;
            else if (_in < _min)
                newVal = _min;
            else
                newVal = _in;
            return newVal;
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