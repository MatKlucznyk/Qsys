using System;
namespace QscQsys.Intermediaries
{
    public abstract class AbstractIntermediaryControl : IQsysIntermediaryControl
    {
        public event EventHandler<QsysInternalEventsArgs> OnStateChanged;
        public event EventHandler<BoolEventArgs> OnSubscribeChanged;

        private readonly string _name;
        private QsysStateData _state;
        private bool _subscribe;

        public abstract QsysCore Core { get; }

        public string Name { get { return _name; } }

        public QsysStateData State
        {
            get { return _state; }
            protected set
            {
                if (_state == value)
                    return;

                _state = value;

                var handler = OnStateChanged;
                if (handler != null)
                    handler(this, new QsysInternalEventsArgs(_state));
            }
        }

        public bool Subscribe
        {
            get { return _subscribe; }
            protected set
            {
                if (_subscribe == value)
                    return;

                _subscribe = value;

                var handler = OnSubscribeChanged;
                if (handler != null)
                    handler(this, new BoolEventArgs(_subscribe));
            }
        }

        public abstract void SendChangePosition(double position);
        public abstract void SendChangeDoubleValue(double value);
        public abstract void SendChangeStringValue(string value);

        public void SendChangeBoolValue(bool value)
        {
            SendChangeDoubleValue(value ? 1 : 0);
        }

        protected AbstractIntermediaryControl(string name)
        {
            _name = name;
        }

        protected void StateChanged(QsysStateData state)
        {
            State = state;
        }


        public void SetSubscribe()
        {
            Subscribe = true;
        }

    }
}