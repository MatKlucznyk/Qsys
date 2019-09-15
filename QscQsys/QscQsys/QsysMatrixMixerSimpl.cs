//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Crestron.SimplSharp;

//namespace QscQsys
//{
//    public class QsysMatrixMixerSimpl
//    {
//        public delegate void CrossPointValueChange(ushort value);
//        public CrossPointValueChange newCrossPointValueChange { get; set; }

//        private QsysMatrixMixer mixer;

//        private ushort Input;
//        private ushort Output;
//        private string cName;
//        private string crossName;
//        private bool registered;

//        public void Initialize(string name, ushort input, ushort output)
//        {
//            cName = name;
//            Input = input;
//            Output = output;

//            Component component = new Component();
//            component.Name = cName;
//            crossName = string.Format("input_{0}_output_{1}_mute", input, output);
//            List<ControlName> names = new List<ControlName>() { new ControlName{Name = crossName} };
//            component.Controls = names;

//            if (QsysCore.RegisterComponent(component))
//            {
//                QsysCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(QsysMatrixMixerSimpl_OnNewEvent);

//                registered = true;
//            }

//            mixer = new QsysMatrixMixer(cName);
//        }

//        void QsysMatrixMixerSimpl_OnNewEvent(object sender, QsysInternalEventsArgs e)
//        {
//            if (e.Name == crossName && newCrossPointValueChange != null)
//            {
//                newCrossPointValueChange(Convert.ToUInt16(e.Data));
//            }
//        }

//        public void SetCrossPoint(ushort value)
//        {
//            switch (value)
//            {
//                case(1):
//                    mixer.SetCrossPointMute(Input.ToString(), Output.ToString(), true);
//                    break;
//                case(0):
//                    mixer.SetCrossPointMute(Input.ToString(), Output.ToString(), false);
//                    break;
//                default:
//                    break;
//            }
//        }
//    }
//}