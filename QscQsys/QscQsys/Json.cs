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

    public class CreateChangeGroupAutoPoll
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

    public class ClearChangeGroup
    {
        [JsonProperty]
        static string jsonrpc = "2.0";
        [JsonProperty]
        static string id = "crestron";
        [JsonProperty]
        static string method = "ChangeGroup.Clear";
        [JsonProperty("params")]
        ClearChangeGroupParams Params = new ClearChangeGroupParams();
    }
    public class ClearChangeGroupParams
    {
        [JsonProperty]
        static string Id = "1";
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
        public IList<string> Controls { get; set; }
    }

    public class AddComponentToChangeGroup
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

    public class AddComponentToChangeGroupParams
    {
        [JsonProperty]
        static string Id = "1";
        public Component Component { get; set; }

    }

    public class Component
    {
        public string Name { get; set; }
        public IList<ControlName> Controls { get; set; }
    }

    public class ControlName
    {
        public string Name { get; set; }
    }

    public class ChangeResult
    {
        public string Component { get; set; }
        public string Name { get; set; }
        public string String { get; set; }
        public double Value { get; set; }
        public double Position { get; set; }
        public IList<String> Choices { get; set; }
        public string Color { get; set; }
        public bool Indeterminate { get; set; }
        public bool Invisible { get; set; }
        public bool Disabled { get; set; }
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
    public class ComponentChangeParams
    {
        public string Name { get; set; }
        public IList<ComponentSetValue> Controls { get; set; }
    }
    public class ComponentSetValue
    {
        public string Name { get; set; }
        public double Value { get; set; }
        public double Position { get; set; }
        public double Ramp { get; set; }

        [JsonIgnore]
        public bool typePos { get; set; }

        public bool ShouldSerializeValue()
        {
            return !typePos;
        }
        public bool ShouldSerializePosition()
        {
            return typePos;
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

    public class ControlSetValue
    {
        [JsonProperty]
        public string jsonrpc = "2.0";
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty("params")]
        public ControlSetValueParams Params { get; set; }
    }
    public class ControlSetValueParams
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public double Position { get; set; }
        public double Ramp { get; set; }

        [JsonIgnore]
        public bool typePos { get; set; }

        public bool ShouldSerializeValue()
        {
            return !typePos;
        }
        public bool ShouldSerializePosition()
        {
            return typePos;
        }
    }

 
    /// <summary>
    /// Engine Status
    /// </summary>
    public class EngineStatusResult
    {
        [JsonProperty]
        public string jsonrpc { get; set; }
        [JsonProperty]
        public string method { get; set; }
        [JsonProperty("params")]
        public EngineStatusPropertiesResult Properties { get; set; }
    }
    public class EngineStatusPropertiesResult
    {
        public string Platform { get; set; }
        public string State { get; set; }
        public string DesignName { get; set; }
        public string DesignCode { get; set; }
        public string IsRedundant { get; set; }
        public string IsEmulator { get; set; }
        public EngineStatusParameterStateResult Status { get; set; }
    }
    public class EngineStatusParameterStateResult
    {
        public int Code { get; set; }
        public string String { get; set; }
    }

    /// <summary>
    /// Heartbeat
    /// </summary>
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


    public class CoreLogon
    {
        [JsonProperty]
        public string jsonrpc = "2.0";
        [JsonProperty]
        public string method = "Logon";
        [JsonProperty("params")]
        public CoreLogonParams Params { get; set; }
    }
    public class CoreLogonParams
    {
        [JsonProperty]
        public string User { get; set; }
        [JsonProperty]
        public string Password { get; set; }
    }


    public class CoreError
    {
        [JsonProperty("jsonrpc")]
        public string jsonrpc { get; set; }
        [JsonProperty("id")]
        public string id { get; set; }
        [JsonProperty("error")]
        public CoreErrorCode Error { get; set; }
    }
    public class CoreErrorCode
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

}   