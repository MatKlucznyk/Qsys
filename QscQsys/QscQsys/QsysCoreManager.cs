using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    internal class QsysCoreManager
    {
        internal static Dictionary<string, QsysCore> Cores = new Dictionary<string, QsysCore>();

        internal static event EventHandler<CoreAddedEventArgs> CoreAdded;

        internal static void AddCore(QsysCore core)
        {
            try
            {
                lock (Cores)
                {
                    if (!Cores.ContainsKey(core.CoreId))
                    {
                        Cores.Add(core.CoreId, core);

                        OnCoreAdded(new CoreAddedEventArgs(core.CoreId));
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        private static void OnCoreAdded(CoreAddedEventArgs e)
        {
            EventHandler<CoreAddedEventArgs> handler = CoreAdded; ;

            if (handler != null)
            {
                handler(null, e);
            }
        }

        internal static double ScaleUp(double level)
        {
            double scaleLevel = level;
            double levelScaled = (scaleLevel * 65535.0);
            return levelScaled;
        }

        internal static double ScaleDown(double level)
        {
            double scaleLevel = level;
            double levelScaled = (scaleLevel / 65535.0);
            return levelScaled;
        }
    }
}