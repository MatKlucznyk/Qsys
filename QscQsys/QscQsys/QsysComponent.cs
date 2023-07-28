using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using QscQsys.Intermediaries;

namespace QscQsys
{
    public abstract class QsysComponent : IDisposable
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

        protected virtual void HandleComponentUpdated(NamedComponent component)
        { }

        #region NamedComponent Callbacks

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

        private void RegisterWithCore()
        {
            if (Component != null)
                return;

            QsysCore core;
            if (!QsysCoreManager.TryGetCore(CoreId, out core))
                return;

            Component = core.LazyLoadNamedComponent(ComponentName);
        }

        protected NamedComponentControl AddControl(string controlName)
        {
            if (Component == null)
                return null;

            return Component.LazyLoadComponentControl(controlName);
        }

        private void QsysCoreManager_CoreAdded(object sender, CoreEventArgs e)
        {
            if (e.CoreId == CoreId)
            {
                RegisterWithCore();
            }
        }

        protected void SendComponentChangePosition(string method, double position)
        {
            if (Component == null)
                return;

            var change = new ComponentChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "position",
                        Caller = ComponentName,
                        Method = method,
                        Position = position
                    }),
                Params =
                    new ComponentChangeParams()
                    {
                        Name = ComponentName,
                        Controls =
                            new List<ComponentSetValue>() {new ComponentSetValue() {Name = method, Position = position}}
                    }
            };

            Component.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        protected void SendComponentChangeDoubleValue(string method, double value)
        {
            if (Component == null)
                return;

            var change = new ComponentChange()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "value",
                        Caller = ComponentName,
                        Method = method,
                        Value = value,
                        StringValue = value.ToString()
                    }),
                Params =
                    new ComponentChangeParams()
                    {
                        Name = ComponentName,
                        Controls =
                            new List<ComponentSetValue>() {new ComponentSetValue() {Name = method, Value = value}}
                    }
            };

            Component.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

        protected void SendComponentChangeStringValue(string method, string value)
        {
            if (Component == null)
                return;

            var change = new ComponentChangeString()
            {
                ID =
                    JsonConvert.SerializeObject(new CustomResponseId()
                    {
                        ValueType = "string_value",
                        Caller = ComponentName,
                        Method = method,
                        StringValue = value
                    }),
                Params =
                    new ComponentChangeParamsString()
                    {
                        Name = ComponentName,
                        Controls =
                            new List<ComponentSetValueString>()
                            {
                                new ComponentSetValueString() {Name = method, Value = value}
                            }
                    }
            };

            Component.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None,
                                                                               new JsonSerializerSettings
                                                                               {
                                                                                   NullValueHandling =
                                                                                       NullValueHandling.Ignore
                                                                               }));
        }

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