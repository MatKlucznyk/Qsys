using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysMatrixMixer
    {
        //Core
        private QsysCore myCore;

        //Named Component
        private string componentName;
        public string ComponentName { get { return this.componentName; } }
        private bool registered;
        public bool IsRegistered { get { return this.registered; } }
        private bool isComponent;

        //Internal Vars
        private int input;
        public int Input { get { return this.input; } }
        private int output;
        public int Output { get { return this.output; } }
        private string crossName;
        public string CrossName { get { return this.crossName; } }
        private bool mute;
        public bool Mute { get { return this.mute; } }

        //Events
        public event EventHandler<QsysEventsArgs> QsysMatrixMixerEvent;


        public QsysMatrixMixer(int _coreID, string _componentName, int _input, int _output)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);

            this.crossName = string.Format("input_{0}_output_{1}_mute", input, output);
            this.input = _input;
            this.output = _output;

            Component component = new Component();
            component.Name = this.componentName;
            List<ControlName> names = new List<ControlName>() { new ControlName { Name = this.crossName } };
            component.Controls = names;

            if (this.myCore.RegisterNamedComponent(component))
            {
                this.myCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);
                this.registered = true;
                this.isComponent = true;
            }
        }


        void Component_OnNewEvent(object _sender, QsysInternalEventsArgs _e)
        {
            if (_e.Name == crossName)
            {
                this.mute = Convert.ToBoolean(_e.Data);
                QsysMatrixMixerEvent(this, new QsysEventsArgs(eQscEventIds.MuteChange, this.componentName, this.mute, Convert.ToInt16(this.mute), Convert.ToString(this.mute)));
            }
        }



        /// <summary>
        /// Sets a crosspoint mute ex. *=everything, 1 2 3=channels 1, 2, 3,  1-6=channels 1 through 6, 1-8 !3=channels 1 through 8 except 3, * !3-5=everything but 3 through 5
        /// </summary>
        /// <param name="inputs">The input channels.</param>
        /// <param name="outputs">The output channels.</param>
        /// <param name="value">The value of the crosspoint mute.</param>
        public void SetCrossPointMute(string _inputs, string _outputs, bool _value)
        {
            SetCrossPointMute set = new SetCrossPointMute();
            set.Params = new SetCrossPointMuteParams();
            set.Params.Name = this.componentName;
            set.Params.Inputs = _inputs;
            set.Params.Outputs = _outputs;
            set.Params.Value = _value;

            this.myCore.Enqueue(JsonConvert.SerializeObject(set));
        }
    }
}