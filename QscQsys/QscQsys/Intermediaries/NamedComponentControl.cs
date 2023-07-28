using System;

namespace QscQsys.Intermediaries
{
    public sealed class NamedComponentControl : AbstractIntermediaryControl
    {
        private readonly NamedComponent _component;

        public NamedComponent Component { get { return _component; } }

        public override QsysCore Core { get { return _component.Core; } }

        public NamedComponentControl(string name, NamedComponent component):base(name)
        {
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