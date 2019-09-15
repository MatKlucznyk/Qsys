using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysSnapshotSimpl
    {
        private QsysSnapshot snapshot;

        public void Initialize(ushort _coreID, SimplSharpString _namedComponent)
        {
            this.snapshot = new QsysSnapshot((int)_coreID, _namedComponent.ToString());
        }

        public void LoadSnapshot(ushort _number)
        {
            this.snapshot.LoadSnapshot(_number);
        }

        public void SaveSnapshot(ushort _number)
        {
            this.snapshot.SaveSnapshot(_number);
        }
    }
}