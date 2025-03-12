using JetBrains.Annotations;
using QscQsys.Intermediaries;

namespace QscQsys.NamedComponents.GenericComponentControl
{
    /// <summary>
    /// Abstract implementation to control a single control of a named component
    /// </summary>
    public abstract class AbstractGenericComponentControl : AbstractQsysComponent
    {
        private NamedComponentControl _control;

        public NamedComponentControl Control
        {
            get { return _control; }
            private set
            {
                if (_control == value)
                    return;

                Unsubscribe(_control);
                _control = value;
                Subscribe(_control);

                if (_control != null)
                    UpdateState(_control.State);
            }
        }

        [PublicAPI]
        public string ControlName { get; private set; }

        public virtual void Initialize(string coreId, string componentName, string controlName)
        {
            ControlName = controlName;

            InternalInitialize(coreId, componentName);
        }

        #region Update State

        protected virtual void UpdateState(QsysStateData state)
        {}

        #endregion

        #region Component Callbacks

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null || string.IsNullOrEmpty(ControlName))
            {
                Control = null;
                return;
            }

            Control = component.LazyLoadComponentControl(ControlName);
        }

        #endregion

        #region NamedComponentControl Callbacks

        protected virtual void Subscribe(NamedComponentControl control)
        {
            if (control == null)
                return;

            control.OnStateChanged += ControlOnStateChanged;
        }

        protected virtual void Unsubscribe(NamedComponentControl control)
        {
            if (control == null)
                return;

            control.OnStateChanged -= ControlOnStateChanged;
        }

        protected virtual void ControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            UpdateState(args.Data);
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
                Control = null;
        }
    }
}