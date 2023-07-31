using System;

namespace QscQsys.Intermediaries
{
    public interface IQsysIntermediary
    {
        string Name { get; }
        QsysCore Core { get; }
    }
}