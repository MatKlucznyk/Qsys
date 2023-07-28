using System;

namespace QscQsys.Intermediaries
{
    /// <summary>
    /// Acts as an intermediary between the QSys Core and the QsysNamedControls
    /// </summary>
    public sealed class NamedControl : AbstractIntermediaryControl
    {
        private readonly QsysCore _core;

        public override QsysCore Core {get { return _core; }}

        public NamedControl(string name, QsysCore core) : base(name)
        {
            _core = core;
        }

        internal void RaiseFeedbackReceived(QsysInternalEventsArgs args)
        {
            State = args.Data;
        }
    }
}