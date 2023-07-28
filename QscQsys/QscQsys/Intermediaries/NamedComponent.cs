using System;
using System.Collections.Generic;
using System.Linq;

namespace QscQsys.Intermediaries
{
    /// <summary>
    /// Acts as an intermediary between the QSys Core and the QsysNamedControls
    /// </summary>
    public sealed class NamedComponent : IQsysIntermediary
    {
        private readonly string _name;
        private readonly QsysCore _core;
        private readonly Dictionary<string, NamedComponentControl> _controls;
        private readonly Dictionary<string, Action<QsysStateData>>  _controlUpdateCallbacks; 
        private bool _subscribe;

        public event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;

        public event EventHandler<ComponentControlEventArgs> OnComponentControlAdded;

        public string Name { get { return _name; } }

        public QsysCore Core { get { return _core; } }

        public bool Subscribe { get { return _subscribe; } }

        private NamedComponent(string name, QsysCore core)
        {
            _subscribe = true;
            _controls = new Dictionary<string, NamedComponentControl>();
            _controlUpdateCallbacks = new Dictionary<string, Action<QsysStateData>>();
            _name = name;
            _core = core;
        }

        private void UpdateState(QsysStateData state)
        {
            var handler = OnFeedbackReceived;
            if (handler != null)
                handler(this, new QsysInternalEventsArgs(state));

            Action<QsysStateData> updateCallback;
            if (TryGetComponentUpdateCallback(state.Name, out updateCallback))
                updateCallback(state);
        }

        public NamedComponentControl LazyLoadComponentControl(string name)
        {
            NamedComponentControl control;

            lock (_controls)
            {                
                if (_controls.TryGetValue(name, out control))
                    return control;

                Action<QsysStateData> updateCallback;
                control = NamedComponentControl.Create(name, this, out updateCallback);
                _controls.Add(name, control);
                _controlUpdateCallbacks.Add(name, updateCallback);
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

        private bool TryGetComponentUpdateCallback(string name, out Action<QsysStateData> updateCallback)
        {
            lock (_controls)
            {
                return _controlUpdateCallbacks.TryGetValue(name, out updateCallback);
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

        public static NamedComponent Create(string name, QsysCore core, out Action<QsysStateData> updateCallback)
        {
            var component = new NamedComponent(name, core);
            updateCallback = component.UpdateState;
            return component;
        }

    }

}