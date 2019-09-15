using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronIO;
using System;
using System.Collections.Generic;

namespace QscQsys
{
    public static class QsysMain
    {
        internal static Dictionary<int, QsysCore> Cores = new Dictionary<int, QsysCore>();
        private static string ProgramID = Directory.GetApplicationDirectory();
        public static string progslot = ProgramID.Substring(ProgramID.Length - 2);


        public static QsysCore AddOrGetCoreObject(int _coreID)
        {
            try
            {
                lock (QsysMain.Cores)
                {
                    if (QsysMain.Cores.ContainsKey(_coreID))
                    {
                        return QsysMain.Cores[_coreID];
                    }
                    else
                    {
                        QsysCore c = new QsysCore();
                        QsysMain.Cores.Add(_coreID, c);
                        return QsysMain.Cores[_coreID];
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Error("Program {0} - QSYS Error - Adding Core {1} - Details:{3}", (object)progslot, (object)_coreID, (object)ex.Message);
                return (QsysCore)null;
            }
        }


        public static bool AddCore(QsysCore _core, int _coreID)
        {
            try
            {
                lock (QsysMain.Cores)
                {
                    if (QsysMain.Cores.ContainsKey(_coreID))
                        return false;
                    QsysMain.Cores.Add(_coreID, _core);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Error("Program {0} - QSYS Error - Adding Core {1} @ {2} - Details:{3}", (object)progslot, (object)_coreID, (object)_core.getCoreIP, (object)ex.Message);
                return false;
            }
        }
    }
}