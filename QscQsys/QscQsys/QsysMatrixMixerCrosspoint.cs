using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysMatrixMixerCrosspoint
    {
        public delegate void CrossPointValueChange(SimplSharpString cName, ushort value);
        public delegate void CrossPointGainChange(SimplSharpString cName, ushort value);
        public CrossPointValueChange newCrossPointValueChange { get; set; }
        public CrossPointGainChange newCrossPointGainChange { get; set; }

        private QsysMatrixMixer mixer;

        private ushort _input;
        private ushort _output;
        private string _cName;
        private string _coreId;
        private string _crossName;
        private bool _registered;

        public void Initialize(string coreId, string name, ushort input, ushort output)
        {
            if (!_registered)
            {
                _cName = name;
                this._coreId = coreId;
                _input = input;
                _output = output;
                _crossName = string.Format("input_{0}_output_{1}_", input, output);

                mixer = new QsysMatrixMixer();
                mixer.Initialize(coreId, name);

                QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

                    RegisterWithCore();
            }
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (e.CoreId == _coreId)
            {
                RegisterWithCore();
            }
        }

        private void RegisterWithCore()
        {
            if (!_registered)
            {
                if (QsysCoreManager.Cores.ContainsKey(_coreId))
                {
                    Component component = new Component(true) { Name = _cName, Controls = new List<ControlName>() { new ControlName() { Name = string.Format("{0}mute", _crossName) }, new ControlName() { Name = string.Format("{0}gain", _crossName) } } };

                    if (QsysCoreManager.Cores[_coreId].RegisterComponent(component))
                    {
                        QsysCoreManager.Cores[_coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                        _registered = true;
                    }
                }
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name == string.Format("{0}mute", _crossName))
            {
                if (newCrossPointValueChange != null)
                {
                    newCrossPointValueChange(_crossName, Convert.ToUInt16(e.Value));
                }
            }
            else if (e.Name == string.Format("{0}gain", _crossName))
            {
                if (newCrossPointGainChange != null)
                {
                    newCrossPointGainChange(_crossName, Convert.ToUInt16(QsysCoreManager.ScaleUp(e.Position)));
                }
            }
        }
        public void SetCrossPoint(ushort value)
        {
            if (_registered)
            {
                switch (value)
                {
                    case (1):
                        mixer.SetCrossPointMute(_input.ToString(), _output.ToString(), true);
                        break;
                    case (0):
                        mixer.SetCrossPointMute(_input.ToString(), _output.ToString(), false);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetCrossPointGain(int value)
        {
            if (_registered)
            {
                ComponentChange newGainChange = new ComponentChange() { Params = new ComponentChangeParams() { Name = _cName, Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("{0}gain", _crossName), Position = QsysCoreManager.ScaleDown(value) } } } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(newGainChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SetCrossPointGain(ushort value)
        {
            this.SetCrossPointGain((int)value);
        }
    }
}