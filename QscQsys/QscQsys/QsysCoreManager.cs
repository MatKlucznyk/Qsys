using System;
using System.Collections.Generic;
using Crestron.SimplSharp;

namespace QscQsys
{
    public static class QsysCoreManager
    {
        private static readonly object CoresLock = new object();
        private static readonly Dictionary<string, QsysCore> Cores = new Dictionary<string, QsysCore>();

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
            lock (CoresLock)
            {
                return Cores.TryGetValue(coreId, out core);
            }
        }

        internal static void AddCore(QsysCore core)
        {
            try
            {
                lock (CoresLock)
                {
                    if (!Cores.ContainsKey(core.CoreId))
                        Cores.Add(core.CoreId, core);
                }

                RaiseCoreAdded(new CoreEventArgs(core.CoreId));
            }
            catch (Exception e)
            {
                ErrorLog.Exception(string.Format("Error in QsysCoreManager AddCore: {0}", e.Message), e);
            }
        }

        internal static void RemoveCore(QsysCore core)
        {
            try
            {
                lock (CoresLock)
                {
                    if (Cores.ContainsKey(core.CoreId))
                        Cores.Remove(core.CoreId);
                }

                RaiseCoreRemoved(new CoreEventArgs(core.CoreId));
            }
            catch (Exception e)
            {
                ErrorLog.Exception(string.Format("Error in QsysCoreManager RemoveCore: {0}", e.Message), e);
            }
        }

        private static void RaiseCoreAdded(CoreEventArgs e)
        {
            try
            {
                var handler = CoreAdded;

                if (handler != null)
                    handler(null, e);
            }
            catch (Exception ex)
            {
                ErrorLog.Exception("RaiseCoreAdded Exception:", ex);
            }
        }

        private static void RaiseCoreRemoved(CoreEventArgs e)
        {
            var handler = CoreRemoved;

            if (handler != null)
                handler(null, e);
        }
    }
}