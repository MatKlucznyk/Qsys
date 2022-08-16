using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public abstract class QsysComponent : IDisposable
    {
        protected string _cName;
        protected bool _registered;
        protected string _coreId;
        private Component _component;
        private bool _disposed;

        public string ComponentName { get { return _cName; } }
        public bool IsRegistered { get { return _registered; } }
        public string CoreID { get { return _coreId; } }

        /// <summary>
        /// Default constructor for a QsysFader
        /// </summary>
        /// <param name="Name">The component name of the gain.</param>
        public void Initialize(string coreId, Component component)
        {
            if (!_registered)
            {
                _coreId = coreId;
                _component = component;
                _cName = component.Name;

                QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

                RegisterWithCore();
            }
        }

        private void RegisterWithCore()
        {
            if (!_registered)
            {
                if (QsysCoreManager.Cores.ContainsKey(_coreId) && _component != null)
                {

                    if (QsysCoreManager.Cores[_coreId].RegisterComponent(_component))
                    {
                        QsysCoreManager.Cores[_coreId].Components[_component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                        _registered = true;
                    }
                }
            }
        }

        private void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (e.CoreId == _coreId)
            {
                RegisterWithCore();
            }
        }

        protected virtual void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            _disposed = true;
            if (disposing)
            {
                QsysCoreManager.CoreAdded -= QsysCoreManager_CoreAdded;
                if(_registered) QsysCoreManager.Cores[_coreId].Components[_component].OnNewEvent -= Component_OnNewEvent;
            }
        }
    }
}