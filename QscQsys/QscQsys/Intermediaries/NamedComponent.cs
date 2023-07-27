using System;
using System.Collections.Generic;
using System.Linq;

namespace QscQsys.Intermediaries
{
    /// <summary>
    /// Acts as an intermediary between the QSys Core and the QsysNamedControls
    /// </summary>
    public sealed class NamedComponent
    {
        private readonly string _name;
        private readonly QsysCore _core;
        private readonly Dictionary<string, NamedComponentControl> _controls;
        private bool _subscribe;

        public event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;

        public event EventHandler<ComponentControlEventArgs> OnComponentControlAdded;

        public string Name { get { return _name; } }

        public QsysCore Core { get { return _core; } }

        public bool Subscribe { get { return _subscribe; } }

        public NamedComponent(string name, QsysCore core)
        {
            _subscribe = true;
            _controls = new Dictionary<string, NamedComponentControl>();
            _name = name;
            _core = core;
        }

        internal void RaiseFeedbackReceived(QsysInternalEventsArgs args)
        {
            var handler = OnFeedbackReceived;
            if (handler != null)
                handler(this, args);

            NamedComponentControl control;
            if (TryGetComponentControl(args.Name, out control))
                control.RaiseFeedbackReceived(args);
        }

        public NamedComponentControl LazyLoadComponentControl(string name)
        {
            NamedComponentControl control;

            lock (_controls)
            {                
                if (_controls.TryGetValue(name, out control))
                    return control;

                control = new NamedComponentControl(name, this);
                _controls.Add(name, control);
            }

            var handler = OnComponentControlAdded;
            if (handler != null)
                handler(this, new ComponentControlEventArgs(control));

            return control;
        }

        public bool TryGetComponentControl(string name, out NamedComponentControl control)
        {
            lock (_controls)
            {
                return _controls.TryGetValue(name, out control);
            }
        }

        public IEnumerable<NamedComponentControl> GetComponentControls()
        {
            lock (_controls)
            {
                return _controls.Values.ToArray();
            }
        }

        public Component ToComponent()
        {
            return Component.Instantiate(
                Name,
                GetComponentControls().Select(control => control.ToControlName())
                );
        }

    }

}