using System;

namespace QscQsys.Intermediaries
{
    public interface IQsysIntermediary
    {
        event EventHandler<QsysInternalEventsArgs> OnFeedbackReceived;
        string Name { get; }
        QsysCore Core { get; }
    }
}