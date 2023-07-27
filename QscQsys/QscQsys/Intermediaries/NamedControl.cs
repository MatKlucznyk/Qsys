using System;

namespace QscQsys.Intermediaries
{
    /// <summary>
    /// Acts as an intermediary between the QSys Core and the QsysNamedControls
    /// </summary>
    public sealed class NamedControl
    {
        private readonly string _name;
        private readonly QsysCore _core;

        public event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;

        public string Name {get { return _name; }}

        public QsysCore Core {get { return _core; }}

        public NamedControl(string name, QsysCore core)
        {
            _name = name;
            _core = core;
        }

        internal void RaiseFeedbackReceived(QsysInternalEventsArgs args)
        {
            var handler = OnFeedbackReceived;
            if (handler != null)
                handler(this, args);
        }
    }
}