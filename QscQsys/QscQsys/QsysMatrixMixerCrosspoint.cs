using System;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysMatrixMixerCrosspoint
    {
        public delegate void CrossPointMuteChange(SimplSharpString cName, ushort value);
        public delegate void CrossPointGainChange(SimplSharpString cName, ushort value);
        public CrossPointMuteChange newCrossPointMuteChange { get; set; }
        public CrossPointGainChange newCrossPointGainChange { get; set; }

        private QsysMatrixMixer _mixer;

        private ushort _input;
        private ushort _output;
        private string _cName;
        private string _coreId;
        private string _crossName;
        private bool _registered;

        internal string CrosspointName { get { return _crossName; } }

        public void Initialize(string coreId, string name, ushort input, ushort output)
        {
            if (!_registered)
            {
                _cName = name;
                _coreId = coreId;
                _input = input;
                _output = output;
                _crossName = string.Format("input_{0}_output_{1}_", input, output);

                QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

                RegisterWithMatrixMixer();
            }
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (e.CoreId == _coreId)
            {
                RegisterWithMatrixMixer();
            }
        }

        private void RegisterWithMatrixMixer()
        {
            if (!_registered)
            {
                if (QsysCoreManager.Cores.ContainsKey(_coreId))
                {
                    _mixer = QsysCoreManager.Cores[_coreId].GetMatrixMixer(_cName);

                    if (_mixer != null)
                    {
                        _mixer.RegisterCrosspoint(this);
                        _registered = true;
                    }
                }
            }
        }

        internal void ComponentUpdate(QsysInternalEventsArgs e)
        {
            if (e.Name == string.Format("{0}mute", _crossName))
            {
                if (newCrossPointMuteChange != null)
                {
                    newCrossPointMuteChange(_crossName, Convert.ToUInt16(e.Value));
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
        public void SetCrossPointMute(ushort value)
        {
            if (_registered)
            {
                _mixer.SetCrosspointMute(_input, _output, value);
            }
        }

        public void SetCrossPointGain(ushort value)
        {
            _mixer.SetCrosspointGain(_input, _output, value);
        }
    }
}