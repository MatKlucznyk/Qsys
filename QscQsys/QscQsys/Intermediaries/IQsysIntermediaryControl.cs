namespace QscQsys.Intermediaries
{
    public interface IQsysIntermediaryControl : IQsysIntermediary
    {
         QsysStateData State { get; }
    }
}