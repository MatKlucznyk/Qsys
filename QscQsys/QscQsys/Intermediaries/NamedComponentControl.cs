using System;

namespace QscQsys.Intermediaries
{
    public sealed class NamedComponentControl : IQsysIntermediaryControl
    {
        private readonly NamedComponent _component;
        private readonly string _name;
        private QsysStateData _state;

        public QsysStateData State
        {
            get { return _state; }
            private set
            {
                if (_state == value)
                    return;

                _state = value;

                var handler = OnFeedbackReceived;
                if (handler != null)
                    handler(this, new QsysInternalEventsArgs(_state));
            }
        }

        public event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;

        public NamedComponent Component { get { return _component; } }

        public string Name { get { return _name; } }
        public QsysCore Core { get { return _component.Core; } }

        public NamedComponentControl(string name, NamedComponent component)
        {
            _name = name;
            _component = component;
        }

        internal void RaiseFeedbackReceived(QsysInternalEventsArgs args)
        {
            State = args.Data;
        }

        public ControlName ToControlName()
        {
            return ControlName.Instantiate(Name);
        }
    }
}