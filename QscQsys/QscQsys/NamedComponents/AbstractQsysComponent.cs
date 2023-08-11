using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using QscQsys.Intermediaries;

namespace QscQsys.NamedComponents
{
    public abstract class AbstractQsysComponent : IDisposable
    {
        //protected bool _registered;
        //private Component _component;
        private bool _isInitialized;
        private NamedComponent _component;
        private bool _disposed;

        public string ComponentName { get; private set; }
        public bool IsRegistered { get { return Component != null; } }
        public string CoreId { get; private set; }

        public NamedComponent Component
        {
            get { return _component; }
            private set
            {
                if (_component == value)
                    return;

                Unsubscribe(_component);
                _component = value;
                Subscribe(_component);

                HandleComponentUpdated(_component);
            }
        }

        /// <summary>
        /// Initialize to be called from concrete's initialize method
        /// </summary>
        /// <param name="coreId"></param>
        /// <param name="componentName"></param>
        protected void InternalInitialize(string coreId, string componentName)
        {
            if (_isInitialized)
                return;

            _isInitialized = true;

            CoreId = coreId;
            ComponentName = componentName;

            QsysCoreManager.CoreAdded += QsysCoreManager_CoreAdded;

            RegisterWithCore();
        }

        #region NamedComponent Callbacks

        protected virtual void HandleComponentUpdated(NamedComponent component)
        { }

        private void Subscribe(NamedComponent component)
        {
            if (component == null)
                return;

            component.OnFeedbackReceived += ComponentOnFeedbackReceived;
        }

        private void Unsubscribe(NamedComponent component)
        {
            if (component == null)
                return;

            component.OnFeedbackReceived -= ComponentOnFeedbackReceived;
        }

        protected virtual void ComponentOnFeedbackReceived(object sender, QsysInternalEventsArgs qsysInternalEventsArgs)
        {
        }

        #endregion

        #region Core Manger Callbacks

        private void QsysCoreManager_CoreAdded(object sender, CoreEventArgs e)
        {
            if (e.CoreId == CoreId)
                RegisterWithCore();
        }

        private void RegisterWithCore()
        {

            QsysCore core;
            if (!QsysCoreManager.TryGetCore(CoreId, out core))
                return;

            Component = core.LazyLoadNamedComponent(ComponentName);
        }

        #endregion

        /// <summary>
        /// Clean up of unmanaged resources
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            _disposed = true;
            if (disposing)
            {
                QsysCoreManager.CoreAdded -= QsysCoreManager_CoreAdded;
                Component = null;
            }
        }
    }
}