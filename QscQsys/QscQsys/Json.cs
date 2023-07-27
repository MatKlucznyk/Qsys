using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace QscQsys
{
    public class GetComponents
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = JsonConvert.SerializeObject(new CustomResponseId() { Method = "Component.GetComponents" });
        [JsonProperty]
        static string method = "Component.GetComponents";
        [JsonProperty("params")]
        static string Params = "crestron";

    }

    public class ComponentResults
    {
        public IList<ComponentProperties> Properties { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }

    public class ComponentProperties
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    public class CreateChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = JsonConvert.SerializeObject(new CustomResponseId() { Method = "ChangeGroup.AutoPoll" });
        [JsonProperty]
        static string method = "ChangeGroup.AutoPoll";
        [JsonProperty("params")]
        internal CreateChangeGroupParams Params = new CreateChangeGroupParams();
    }

    public class CreateChangeGroupParams
    {
        [JsonProperty]
        internal static string Id = "crestron";
        [JsonProperty]
        static double Rate;

        public CreateChangeGroupParams()
        {
            if (!QsysCoreManager.Is3Series)
            {
                Rate = 0.3;
            }
            else
            {
                Rate = 0.5;
            }
        }
    }

    public class AddComponentToChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = JsonConvert.SerializeObject(new CustomResponseId());
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty("params")]
        public AddComponentToChangeGroupParams ComponentParams { get; set; }

        public static AddComponentToChangeGroup Instantiate(Component component)
        {
            return new AddComponentToChangeGroup
            {
                method = "ChangeGroup.AddComponentControl",
                ComponentParams = AddComponentToChangeGroupParams.Instantiate(component)
            };
        }
    }

    public class AddControlToChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = JsonConvert.SerializeObject(new CustomResponseId());
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty("params")]
        public AddControlToChangeGroupParams ControlParams { get; set; }

        public static AddControlToChangeGroup Instantiate(IEnumerable<string> controls)
        {
            return new AddControlToChangeGroup
            {
                method = "ChangeGroup.AddControl",
                ControlParams =
                    AddControlToChangeGroupParams.Instantiate(controls)
            };
        }
    }

    public class AddControlToChangeGroupParams
    {
        [JsonProperty]
        static string Id = "crestron";
        public List<string> Controls { get; set; }

        public static AddControlToChangeGroupParams Instantiate(IEnumerable<string> controls)
        {
            return new AddControlToChangeGroupParams 
            {Controls = new List<string>(controls)};
        }
    }

    public class AddComponentToChangeGroupParams
    {
        [JsonProperty]
        static string Id = "crestron";
        public Component Component { get; set; }

        public static AddComponentToChangeGroupParams Instantiate(Component component)
        {
            return new AddComponentToChangeGroupParams
            {
                Component = component
            };
        }

    }


    public class Component : IEquatable<Component>
    {
        internal bool Subscribe;
        public string Name { get; set; }
        public IList<ControlName> Controls { get; set; }

        public Component(bool subscribe)
        {
            Subscribe = subscribe;
        }

        public bool Equals(Component other)
        {
            return this.Name == other.Name;
        }

        public static Component Instantiate(string name, IEnumerable<ControlName> controls)
        {
            return new Component(true)
            {
                Name = name,
                Controls = new List<ControlName>(controls)
            };
        }

        public static Component Instantiate(string name, IEnumerable<string> controls)
        {
            return Instantiate(name, controls.Select(controlName => ControlName.Instantiate(controlName)));
        } 
    }

    public class Control : IEquatable<Control>
    {
        internal bool Subscribe;
        public string Name { get; set; }

        public Control(bool subscribe)
        {
            Subscribe = subscribe;
        }

        public bool Equals(Control other)
        {
            return this.Name == other.Name;
        }
    }

    public class ControlName
    {
        public string Name { get; set; }

        public static ControlName Instantiate(string name)
        {
            return new ControlName
            {
                Name = name
            };
        }
    }

    public class Heartbeat
    {
        [JsonProperty]
        static public string jsonrpc = "2.0";
        [JsonProperty]
        static public string method = "NoOp";
        [JsonProperty("params")]
        HeartbeatParams Params = new HeartbeatParams();
    }

    public class HeartbeatParams
    {
    }

    public class ChangeResult
    {
        [JsonProperty(Required = Required.Default)]
        public string Component { get; set; }
        public string Name { get; set; }
        public string String { get; set; }
        public double Value { get; set; }
        public double Position { get; set; }
        public IList<string> Choices { get; set; }
    }

    public class ComponentChange
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty("id")]
        public string ID;
        [JsonProperty]
        static string method = "Component.Set";
        [JsonProperty("params")]
        public ComponentChangeParams Params { get; set; }

        public ComponentChange()
        {
            ID = JsonConvert.SerializeObject(new CustomResponseId() { Method = "Component.Set" });
        }
    }

    public class ControlIntegerChange
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty("id")]
        public string ID;
        [JsonProperty]
        static string method = "Control.Set";
        [JsonProperty("params")]
        public ControlIntegerParams Params { get; set; }

        public ControlIntegerChange()
        {
            ID = JsonConvert.SerializeObject(new CustomResponseId() { Method = "ControlSet.Set" });
        }
    }

    public class ControlStringChange
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty("id")]
        public string ID;
        [JsonProperty]
        static string method = "Control.Set";
        [JsonProperty("params")]
        public ControlStringParams Params { get; set; }

        public ControlStringChange()
        {
            ID = JsonConvert.SerializeObject(new CustomResponseId() { Method = "ControlSet.Set" });
        }
    }

    public class ComponentChangeParams
    {
        public string Name { get; set; }
        public IList<ComponentSetValue> Controls { get; set; }
    }

    public class ControlIntegerParams
    {
        public string Name { get; set; }
        [JsonProperty(Required = Required.Default)]
        public double? Value { get; set; }
        [JsonProperty(Required = Required.Default)]
        public double? Position { get; set; }

        public ControlIntegerParams()
        {
            this.Value = null;
            this.Position = null;
        }
    }

    public class ControlStringParams
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ComponentSetValue
    {
        public string Name { get; set; }
        [JsonProperty(Required = Required.Default)]
        public double? Value { get; set; } 
        [JsonProperty(Required = Required.Default)]
        public double? Position { get; set; }

        public ComponentSetValue()
        {
            this.Value = null;
            this.Position = null;
        }
    }

    public class SetCrossPointMute
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        public string ID;
        [JsonProperty]
        static string method = "Mixer.SetCrossPointMute";
        [JsonProperty("params")]
        public SetCrossPointMuteParams Params { get; set; }

        public SetCrossPointMute()
        {
            ID = JsonConvert.SerializeObject(new CustomResponseId() { Method = "Mixer.SetCrossPointMute" });
        }
    }

    public class SetCrossPointMuteParams
    {
        public string Name { get; set; }
        public string Inputs { get; set; }
        public string Outputs { get; set; }
        public bool Value { get; set; }
    }

    public class ComponentChangeString
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty("id")]
        public string ID;
        [JsonProperty]
        static string method = "Component.Set";
        [JsonProperty("params")]
        public ComponentChangeParamsString Params { get; set; }

        public ComponentChangeString()
        {
            ID = JsonConvert.SerializeObject(new CustomResponseId() { Method = "Component.Set" });
        }
    }

    public class ComponentChangeParamsString
    {
        public string Name { get; set; }
        public IList<ComponentSetValueString> Controls { get; set; }
    }

    public class ComponentSetValueString
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class ListBoxChoice
    {
        public string Text { get; set; }
        public string Color { get; set; }
        public string Icon { get; set; }
    }

    public class Logon
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = JsonConvert.SerializeObject(new CustomResponseId() { Method = "Logon", });
        [JsonProperty]
        static string method = "Logon";
        [JsonProperty("params")]
        public LogonParams Params { get; set; }
    }

    public class LogonParams
    {
        [JsonProperty]
        public string User { get; set; }
        [JsonProperty]
        public string Password { get; set; }
    }

    public class CustomResponseId
    {
        [JsonProperty("app")]
        public string App { get; set;}
        [JsonProperty("caller")]
        public string Caller { get; set;}
        [JsonProperty("valueType")]
        public string ValueType { get; set; }
        [JsonProperty("method")]
        public string Method { get; set;}
        [JsonProperty("value")]
        public double Value { get; set; }
        [JsonProperty("stringValue")]
        public string StringValue { get; set;}
        [JsonProperty("position")]
        public double Position { get; set; }

        public CustomResponseId()
        {
            App = "crestron";
            ValueType = string.Empty;
            Caller = string.Empty;
            Method = string.Empty;
            Value = new double();
            StringValue = string.Empty;
            Position = new double();
        }

        public CustomResponseId(string valueType, string caller, string method, double value, string stringValue, double position)
        {
            App = "crestron";
            ValueType = valueType;
            Caller = caller;
            Method = method;
            Value = value;
            StringValue = stringValue;
            Position = position;
        }
    }
}