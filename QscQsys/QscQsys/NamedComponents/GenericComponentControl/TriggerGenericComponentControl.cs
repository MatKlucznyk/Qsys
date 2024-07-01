using JetBrains.Annotations;

namespace QscQsys.NamedComponents.GenericComponentControl
{
    /// <summary>
    /// Control a trigger as part of any kind of named component
    /// </summary>
    public sealed class TriggerGenericComponentControl : AbstractQsysComponent
    {
        [PublicAPI]
        public string ControlName { get; private set; }

        [PublicAPI("S+")]
        public TriggerGenericComponentControl()
        {
        }

        [PublicAPI("S+")]
        public void Initialize(string coreId, string componentName, string controlName)
        {
            ControlName = controlName;

            InternalInitialize(coreId, componentName);
        }

        #region Set Values

        [PublicAPI("S+")]
        public void Trigger()
        {
            if (Component == null || string.IsNullOrEmpty(ControlName))
                return;

            Component.SendChangeDoubleValue(ControlName, 1);
        }

        #endregion

    }
}