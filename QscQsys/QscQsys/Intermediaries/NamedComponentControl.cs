using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QscQsys.Intermediaries
{
    public sealed class NamedComponentControl : AbstractIntermediaryControl
    {
        private readonly NamedComponent _component;

        public NamedComponent Component { get { return _component; } }

        public override QsysCore Core { get { return _component.Core; } }

        private NamedComponentControl(string name, NamedComponent component):base(name)
        {
            _component = component;
        }

        public ControlName ToControlName()
        {
            return ControlName.Instantiate(Name);
        }

        public static NamedComponentControl Create(string name, NamedComponent component,
                                                   out Action<QsysStateData> updateCallback)
        {
            var control = new NamedComponentControl(name, component);
            updateCallback = control.StateChanged;
            return control;
        }

        #region Send Data

        public override void SendChangePosition(double position)
        {
            var change = new ComponentChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "position",
                        Caller = Component.Name,
                        Method = Name,
                        Position = position
                    }),
                Params =
                    new ComponentChangeParams()
                    {
                        Name = Component.Name,
                        Controls =
                            new List<ComponentSetValue>() { new ComponentSetValue() { Name = Name, Position = position } }
                    }
            };

            Component.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        public override void SendChangeDoubleValue(double value)
        {
            if (Component == null)
                return;

            var change = new ComponentChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "value",
                        Caller = Component.Name,
                        Method = Name,
                        Value = value,
                        StringValue = value.ToString()
                    }),
                Params =
                    new ComponentChangeParams()
                    {
                        Name = Component.Name,
                        Controls =
                            new List<ComponentSetValue>() { new ComponentSetValue() { Name = Name, Value = value } }
                    }
            };

            Component.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        public override void SendChangeStringValue(string value)
        {
            if (Component == null)
                return;

            var change = new ComponentChangeString()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "string_value",
                        Caller = Component.Name,
                        Method = Name,
                        StringValue = value
                    }),
                Params =
                    new ComponentChangeParamsString()
                    {
                        Name = Component.Name,
                        Controls =
                            new List<ComponentSetValueString>()
                            {
                                new ComponentSetValueString() {Name = Name, Value = value}
                            }
                    }
            };

            Component.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        #endregion
    }
}