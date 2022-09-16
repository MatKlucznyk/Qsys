using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysNv32hDecoder : QsysComponent
    {
        public delegate void Nv32hDecoderInputChange(SimplSharpString cName, ushort input);
        public Nv32hDecoderInputChange newNv32hDecoderInputChange { get; set; }

        private int _currentSource;

        public string ComponentName { get { return _cName; } }
        public int CurrentSource { get { return _currentSource; } }

        public void Initialize(string coreId, string componentName)
        {
            var component = new Component(true) { Name = componentName, Controls = new List<ControlName>() { new ControlName() { Name = "hdmi_out_0_select_index" } } };
            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            _currentSource = Convert.ToInt16(e.Value);

            if (newNv32hDecoderInputChange != null)
                newNv32hDecoderInputChange(_cName, Convert.ToUInt16(_currentSource));
        }

        public void ChangeInput(int source)
        {
            if (_registered)
            {
                SendComponentChangeDoubleValue("hdmi_out_0_select_index", source);
            }
        }
    }
}