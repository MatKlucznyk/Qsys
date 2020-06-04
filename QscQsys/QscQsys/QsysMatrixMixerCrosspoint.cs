using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMatrixMixerCrosspoint
    {
        public delegate void CrossPointValueChange(ushort value);
        public CrossPointValueChange newCrossPointValueChange { get; set; }

        private QsysMatrixMixer mixer;

        private ushort Input;
        private ushort Output;
        private string cName;
        private string coreId;
        private string crossName;
        private bool registered;

        public void Initialize(string coreId, string name, ushort input, ushort output)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

            cName = name;
            this.coreId = coreId;
            Input = input;
            Output = output;
            crossName = string.Format("input_{0}_output_{1}_mute", input, output);

            mixer = new QsysMatrixMixer();
            mixer.Initialize(coreId, name);

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
                Component component = new Component() { Name = cName, Controls = new List<ControlName>() { new ControlName() { Name = crossName } } };

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == crossName && newCrossPointValueChange != null)
            {
                newCrossPointValueChange(Convert.ToUInt16(e.Value));
            }
        }

        public void SetCrossPoint(ushort value)
        {
            if (registered)
            {
                switch (value)
                {
                    case (1):
                        mixer.SetCrossPointMute(Input.ToString(), Output.ToString(), true);
                        break;
                    case (0):
                        mixer.SetCrossPointMute(Input.ToString(), Output.ToString(), false);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}