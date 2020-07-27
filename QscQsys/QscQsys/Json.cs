using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QscQsys
{
    public class GetComponents
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
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
        static string id = "crestron";
        [JsonProperty]
        static string method = "ChangeGroup.AutoPoll";
        [JsonProperty("params")]
        CreateChangeGroupParams Params = new CreateChangeGroupParams();
    }

    public class CreateChangeGroupParams
    {
        [JsonProperty]
        static string Id = "1";
        [JsonProperty]
        static double Rate = 0.15;
    }

    public class AddComoponentToChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty("params")]
        public AddComponentToChangeGroupParams ComponentParams { get; set; }
    }

    public class AddControlToChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty("params")]
        public AddControlToChangeGroupParams ControlParams { get; set; }
    }

    public class AddControlToChangeGroupParams
    {
        [JsonProperty]
        static string Id = "1";
        public List<string> Controls { get; set; }
    }

    public class AddComponentToChangeGroupParams
    {
        [JsonProperty]
        static string Id = "1";
        public Component Component { get; set; }

    }


    public class Component : IEquatable<Component>
    {
        public string Name { get; set; }
        public IList<ControlName> Controls { get; set; }

        public bool Equals(Component other)
        {
            return this.Name == other.Name;
        }
    }

    public class Control : IEquatable<Control>
    {
        public string Name { get; set; }

        public bool Equals(Control other)
        {
            return this.Name == other.Name;
        }
    }

    public class ControlName
    {
        public string Name { get; set; }
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
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "Component.Set";
        [JsonProperty("params")]
        public ComponentChangeParams Params { get; set; }
    }

    public class ControlIntegerChange
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "Control.Set";
        [JsonProperty("params")]
        public ControlIntegerParams Params { get; set; }
    }

    public class ControlStringChange
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "Control.Set";
        [JsonProperty("params")]
        public ControlStringParams Params { get; set; }
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
        static string id = "crestron";
        [JsonProperty]
        static string method = "Mixer.SetCrossPointMute";
        [JsonProperty("params")]
        public SetCrossPointMuteParams Params { get; set; }
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
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "Component.Set";
        [JsonProperty("params")]
        public ComponentChangeParamsString Params { get; set; }
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
}