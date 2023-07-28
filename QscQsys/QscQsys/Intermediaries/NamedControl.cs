using System;
using Newtonsoft.Json;

namespace QscQsys.Intermediaries
{
    /// <summary>
    /// Acts as an intermediary between the QSys Core and the QsysNamedControls
    /// </summary>
    public sealed class NamedControl : AbstractIntermediaryControl
    {
        private readonly QsysCore _core;

        public override QsysCore Core {get { return _core; }}

        private NamedControl(string name, QsysCore core) : base(name)
        {
            _core = core;
        }

        public static NamedControl Create(string name, QsysCore core, out Action<QsysStateData> updateCallback)
        {
            var control = new NamedControl(name, core);
            updateCallback = control.StateChanged;
            return control;
        }

        public override void SendChangePosition(double value)
        {
            var change = new ControlIntegerChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "position",
                        Caller = Name,
                        Method = "Control.Set",
                        Position = value
                    }),
                Params = new ControlIntegerParams() {Name = Name, Position = value}
            };

            Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                     new JsonSerializerSettings
                                                     {
                                                         NullValueHandling = NullValueHandling.Ignore
                                                     }));
        }

        public override void SendChangeDoubleValue(double value)
        {
            var change = new ControlIntegerChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "value",
                        Caller = Name,
                        Method = "Control.Set",
                        Value = value,
                        StringValue = value.ToString()
                    }),
                Params = new ControlIntegerParams() {Name = Name, Value = value}
            };

            Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                     new JsonSerializerSettings
                                                     {
                                                         NullValueHandling = NullValueHandling.Ignore
                                                     }));
        }

        public override void SendChangeStringValue(string value)
        {
            var change = new ControlStringChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "string_value",
                        Caller = Name,
                        Method = "Control.Set",
                        StringValue = value
                    }),
                Params = new ControlStringParams() {Name = Name, Value = value}
            };

            Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                     new JsonSerializerSettings
                                                     {
                                                         NullValueHandling = NullValueHandling.Ignore
                                                     }));
        }
    }
}