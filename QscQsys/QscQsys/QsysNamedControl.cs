using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public enum eControlType
    {
        isFloat = 0,
        isInteger = 1,
        isMomentary = 2,
        isToggle = 3,
        isTrigger = 4,
        isString = 5
    }
    public class QsysNamedControl
    {
        private string cName;
        private bool registered;
        private eControlType ctrlType;

        private double dVal = 0;
        private string sVal = "";
        private bool bVal = false;

        //private bool currentMute;
        //private int currentLvl;
        //private double currentLvlDb;
        //private int lastSentLvl;
        //private double max;
        //private double min;
        //private double rampTime;

        public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;

        public string ControlName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public eControlType ControlType { get { return ctrlType; } }

        public double D_Val { get { return dVal; } }
        public string S_Val { get { return sVal; } }
        public bool b_Val { get { return bVal; } }


        //public bool CurrentMute { get { return currentMute; } }
        //public int CurrentVolume { get { return currentLvl; } }
        //public double CurrentVolumeDb { get { return currentLvlDb; } }

        

        /// <summary>
        /// Default constructor for a QsysNamedControl
        /// </summary>
        /// <param name="Name">The component name of the gain.</param>
        public QsysNamedControl(string Name, eControlType Type)
        {
            cName = Name;
            if (QsysProcessor.RegisterControl(cName))
            {
                CrestronConsole.PrintLine("adding control with name: {0}", cName);
                QsysProcessor.Controls[cName].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Control_OnNewEvent);

                registered = true;
            }
            else
            {
                CrestronConsole.PrintLine("adding new control {0} failed..", Name);
            }
        }

        void Control_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            CrestronConsole.PrintLine("control name: {0}, value: {1}, string: {2}", e.Name, e.Data, e.SData);
            switch (ctrlType)
            {
                case eControlType.isFloat:
                    dVal = e.Data * 10;
                    sVal = e.SData;
                    bVal = false;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, bVal, (int)dVal, sVal));
                    break;
                case eControlType.isInteger:
                    dVal = e.Data;
                    sVal = e.SData;
                    bVal = false;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, bVal, (int)dVal, sVal));
                    break;
                case eControlType.isMomentary:
                    dVal = e.Data;
                    sVal = e.SData;
                    bVal = Convert.ToBoolean(e.Data);
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, bVal, (int)dVal, sVal));
                    break;
                case eControlType.isToggle:
                    dVal = e.Data;
                    sVal = e.SData;
                    bVal = Convert.ToBoolean(e.Data);
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, bVal, (int)dVal, sVal));
                    break;
                case eControlType.isTrigger:
                    dVal = e.Data;
                    sVal = e.SData;
                    bVal = false;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, bVal, (int)dVal, sVal));
                    break;
                case eControlType.isString:
                    dVal = 0;
                    sVal = e.SData;
                    bVal = false;
                    QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControl, cName, bVal, (int)dVal, sVal));
                    break;
            }

            //if (e.Name == "gain")
            //{
            //    if (e.Data >= min && e.Data <= max)
            //    {
            //        currentLvl = (int)Math.Round((65535 / (max - min)) * (e.Data + (min * (-1))));
            //        currentLvlDb = e.Data;
            //        lastSentLvl = -1;
            //        QsysFaderEvent(this, new QsysEventsArgs(eQscEventIds.GainChange, cName, true, currentLvl, currentLvl.ToString()));
            //    }
            //}

        }



































        /// <summary>
        /// Sets the current volume.
        /// </summary>
        /// <param name="value">The volume level to set to.</param>
        public void Volume(int value)
        {
            //double newVal = Math.Round((value / (65535 / (max - min))) + min);
            //if (lastSentLvl == newVal || newVal == currentLvl) //avoid repeats
            //    return;
            //lastSentLvl = (int)newVal;

            //ComponentChange newVolumeChange = new ComponentChange();
            //newVolumeChange.Params = new ComponentChangeParams();
            //newVolumeChange.Params.Name = cName;

            //ComponentSetValue volume = new ComponentSetValue();

            //volume.Name = "gain";
            //volume.Value = newVal;
            //volume.Ramp = rampTime;

            //newVolumeChange.Params.Controls = new List<ComponentSetValue>();
            //newVolumeChange.Params.Controls.Add(volume);

            //QsysProcessor.Enqueue(JsonConvert.SerializeObject(newVolumeChange));
        }

        /// <summary>
        /// Sets the current volume in DB.
        /// </summary>
        /// <param name="value">The volume level to set to.</param>
        public void VolumeDb(int value)
        {
            //double newVal = value;
            //if (newVal > max && value < min) //ensure within range
            //    return;
            //if (lastSentLvl == newVal || newVal == currentLvl) //avoid repeats
            //    return;
            //lastSentLvl = (int)newVal;

            //ComponentChange newVolumeChange = new ComponentChange();
            //newVolumeChange.Params = new ComponentChangeParams();
            //newVolumeChange.Params.Name = cName;

            //ComponentSetValue volume = new ComponentSetValue();

            //volume.Name = "gain";
            //volume.Value = newVal;
            //volume.Ramp = rampTime;

            //newVolumeChange.Params.Controls = new List<ComponentSetValue>();
            //newVolumeChange.Params.Controls.Add(volume);

            //QsysProcessor.Enqueue(JsonConvert.SerializeObject(newVolumeChange));
        }

        /// <summary>
        /// Sets the current mute state.
        /// </summary>
        /// <param name="value">The state to set the mute.</param>
        public void Mute(bool value)
        {
            //if (currentMute != value)
            //{
            //    ComponentChange newMuteChange = new ComponentChange();
            //    newMuteChange.Params = new ComponentChangeParams();

            //    newMuteChange.Params.Name = cName;

            //    ComponentSetValue mute = new ComponentSetValue();
            //    mute.Name = "mute";

            //    if (value)
            //        mute.Value = 1;
            //    else
            //        mute.Value = 0;

            //    newMuteChange.Params.Controls = new List<ComponentSetValue>();
            //    newMuteChange.Params.Controls.Add(mute);

            //    QsysProcessor.Enqueue(JsonConvert.SerializeObject(newMuteChange));
            //}
        }

        /// <summary>
        /// Sets the QSys ramp time for the gain
        /// </summary>
        /// <param name="time"></param>
        public void RampTimeMS(double time)
        {
            //rampTime = time / 1000; //ms to sec
        }

    }
}