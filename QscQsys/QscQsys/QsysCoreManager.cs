using System;
using System.Collections.Generic;
using Crestron.SimplSharp;

namespace QscQsys
{
    public static class QsysCoreManager
    {
        private static readonly object _coresLock = new object();
        private static readonly Dictionary<string, QsysCore> _cores = new Dictionary<string, QsysCore>();

        internal static event EventHandler<CoreEventArgs> CoreAdded;
        internal static event EventHandler<CoreEventArgs> CoreRemoved;

        internal static bool Is3Series
        {
            get
            {
                return InitialParametersClass.ControllerPromptName.Contains("3");
            }
        }

        public static bool TryGetCore(string coreId, out QsysCore core)
        {
            lock (_coresLock)
            {
                return _cores.TryGetValue(coreId, out core);
            }
        }

        internal static void AddCore(QsysCore core)
        {
            try
            {
                lock (_coresLock)
                {
                    if (!_cores.ContainsKey(core.CoreId))
                    {
                        _cores.Add(core.CoreId, core);

                        RaiseCoreAdded(new CoreEventArgs(core.CoreId));
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in QsysCoreManager AddCore: {0}", e.Message);
            }
        }

        internal static void RemoveCore(QsysCore core)
        {
            try
            {
                lock (_coresLock)
                {
                    if(_cores.ContainsKey(core.CoreId))
                    {
                        _cores.Remove(core.CoreId);

                        RaiseCoreRemoved(new CoreEventArgs(core.CoreId));
                    }
                }
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in QsysCoreManager RemoveCore: {0}", e.Message);
            }
        }

        private static void RaiseCoreAdded(CoreEventArgs e)
        {
            var handler = CoreAdded; ;

            if (handler != null)
                handler(null, e);
        }

        private static void RaiseCoreRemoved(CoreEventArgs e)
        {
            var handler = CoreRemoved;

            if (handler != null)
                handler(null, e);
        }
    }
}