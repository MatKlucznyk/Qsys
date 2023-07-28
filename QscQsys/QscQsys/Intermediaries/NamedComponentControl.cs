using System;

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
    }
}