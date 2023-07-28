using System;
using System.Collections.Generic;
using QscQsys.Intermediaries;

namespace QscQsys
{
    public sealed class QsysStateData
    {
        private readonly string _type;
        private readonly string _name;
        private readonly double _value;
        private readonly double _position;
        private readonly string _stringValue;
        private readonly List<string> _choices;

        public string Type { get { return _type; } }
        public string Name { get { return _name; } }
        public double Value { get { return _value; } }
        public double Position {get { return _position; }}
        public string StringValue {get { return _stringValue; }}
        public IEnumerable<string> Choices {get { return _choices.ToArray(); }}

        public bool BoolValue
        {
            get { return Math.Abs(_value) > QsysCore.TOLERANCE; }
        }

        public QsysStateData(string type, string name, double value, double position, string stringValue,
                             IEnumerable<string> choices)
        {
            _type = type;
            _name = name;
            _value = value;
            _position = position;
            _stringValue = stringValue;
            
            _choices = new List<string>();
            if (choices != null)
                _choices.AddRange(choices);

        }
    }

    /// <summary>
    /// Used only for internal methods.
    /// </summary>
    public sealed class QsysInternalEventsArgs : EventArgs
    {
        private readonly QsysStateData _data;

        public QsysStateData Data { get { return _data; } }

        public string Type { get { return Data.Type; } }
        public string Name { get { return Data.Name; } }

        public double Value { get { return Data.Value; } }

        public double Position { get { return Data.Position; } }
        public string StringValue { get { return Data.StringValue; } }
        public bool BoolValue { get { return Data.BoolValue; } }
        public IEnumerable<string> Choices { get { return Data.Choices; } }

        public QsysInternalEventsArgs(string type, string name, double data, double position, string sData,
                                      IEnumerable<string> choices)
            : this(new QsysStateData(type, name, data, position, sData, choices))
        {
        }

        public QsysInternalEventsArgs(QsysStateData data)
        {
            _data = data;
        }
    }

    public sealed class CoreEventArgs : EventArgs
    {
        private string _coreId;

        public string CoreId { get { return _coreId; } }

        public CoreEventArgs(string coreId)
        {
            _coreId = coreId;
        }
    }

    public sealed class ComponentControlEventArgs : EventArgs
    {
        private readonly NamedComponentControl _control;

        public NamedComponentControl Control {get { return _control; }}

        public ComponentControlEventArgs(NamedComponentControl control)
        {
            _control = control;
        }
    }
}