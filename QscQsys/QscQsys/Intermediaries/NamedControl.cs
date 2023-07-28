using System;

namespace QscQsys.Intermediaries
{
    /// <summary>
    /// Acts as an intermediary between the QSys Core and the QsysNamedControls
    /// </summary>
    public sealed class NamedControl : IQsysIntermediaryControl
    {
        private readonly string _name;
        private readonly QsysCore _core;
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

        public string Name {get { return _name; }}

        public QsysCore Core {get { return _core; }}

        public NamedControl(string name, QsysCore core)
        {
            _name = name;
            _core = core;
        }

        internal void RaiseFeedbackReceived(QsysInternalEventsArgs args)
        {
            State = args.Data;
        }
    }
}