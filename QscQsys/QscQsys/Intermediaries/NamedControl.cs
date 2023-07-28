using System;
using System.Data;

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
    }
}