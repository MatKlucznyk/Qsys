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

        public static bool AddCore(QsysCore core, int id)
        {
            try
            {
                lock (QsysMain.Cores)
                {
                    if (QsysMain.Cores.ContainsKey(id))
                        return false;
                    QsysMain.Cores.Add(id, core);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.Error("Program {0} - QSYS Error - Adding Core {1} @ {2} - Details:{3}", (object)progslot, (object)id, (object)core.getCoreIP, (object)ex.Message);
                return false;
            }
        }
    }
}