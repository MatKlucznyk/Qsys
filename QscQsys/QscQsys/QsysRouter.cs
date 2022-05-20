using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysRouter : QsysComponent
    {
        public delegate void RouterInputChange(SimplSharpString cName, ushort input);
        public RouterInputChange newRouterInputChange { get; set; }

        private int _output;
        private int _currentSelectedInput;

        public int CurrentSelectedInput { get { return _currentSelectedInput; } }
        public int Output { get { return _output; } }

        public void Initialize(string coreId, string componentName, int output)
        {
            _output = output;

            var component = new Component()
            {
                Name = componentName,
                Controls = new List<ControlName>() { new ControlName() { Name = string.Format("select_{0}", output) } }
            };

            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name.Contains(string.Format("select_{0}", _output)))
            {
                _currentSelectedInput = Convert.ToInt16(e.Value);

                if (newRouterInputChange != null)
                    newRouterInputChange(_cName, Convert.ToUInt16(e.Value));
            }
        }

        public void InputSelect(int input)
        {
            if (_registered)
            {
                ComponentChange newInputSelectedChange = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = _cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("select_{0}", _output), Value = input } }
                    }
                };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(newInputSelectedChange, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }
    }
}