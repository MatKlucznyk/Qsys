using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysSoftphoneController : QsysPotsController
    {
        public QsysSoftphoneController(int _coreID, string _componentName)
            : base(_coreID, _componentName)
        {
        }
    }
}