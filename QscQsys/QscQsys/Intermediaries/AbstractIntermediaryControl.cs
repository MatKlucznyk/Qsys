using System;
namespace QscQsys.Intermediaries
{
    public abstract class AbstractIntermediaryControl : IQsysIntermediaryControl
    {
        public event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;

        private readonly string _name;
        private QsysStateData _state;

        public string Name { get { return _name; } }

        public QsysStateData State
        {
            get { return _state; }
            protected set
            {
                if (_state == value)
                    return;

                _state = value;

                var handler = OnFeedbackReceived;
                if (handler != null)
                    handler(this, new QsysInternalEventsArgs(_state));
            }
        }

        public abstract QsysCore Core { get; }

        protected AbstractIntermediaryControl(string name)
        {
            _name = name;
        }

    }
}