using System;

namespace QscQsys.Intermediaries
{
    public sealed class NamedComponentControl
    {
        private readonly NamedComponent _component;
        private readonly string _name;

        public event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;

        public NamedComponent Component { get { return _component; } }

        public string Name { get { return _name; } }

        public NamedComponentControl(string name, NamedComponent component)
        {
            _name = name;
            _component = component;
        }

        internal void RaiseFeedbackReceived(QsysInternalEventsArgs args)
        {
            var handler = OnFeedbackReceived;
            if (handler != null)
                handler(this, args);
        }

        public ControlName ToControlName()
        {
            return ControlName.Instantiate(Name);
        }
    }
}