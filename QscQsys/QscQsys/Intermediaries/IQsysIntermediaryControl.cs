namespace QscQsys.Intermediaries
{
    public interface IQsysIntermediaryControl : IQsysIntermediary
    {
        QsysStateData State { get; }

        void SendChangePosition(double position);
        void SendChangeDoubleValue(double value);
        void SendChangeStringValue(string value);
    }
}