using Crestron.SimplSharp;
using JetBrains.Annotations;
using QscQsys.Utils;

namespace QscQsys.NamedComponents.GenericComponentControl
{
    /// <summary>
    /// Control a boolean control as part of any named component
    /// </summary>
    public sealed class BoolGenericComponentControl : AbstractGenericComponentControl
    {
        public delegate void ControlBoolChangedDelegate(SimplSharpString componentName, SimplSharpString controlName, ushort data);

        [PublicAPI("S+")]
        public ControlBoolChangedDelegate DataChangedCallback { get; set; }

        [PublicAPI("S+")]
        public BoolGenericComponentControl()
        {
        }

        #region Set Values

        [PublicAPI("S+")]
        public void SetBoolean(ushort value)
        {
            if (Control == null)
                return;

            Control.SendChangeBoolValue(value.BoolFromSplus());
        }

        #endregion

        #region Update States

        protected override void UpdateState(QsysStateData state)
        {
            var callback = DataChangedCallback;

            if (state == null || callback == null)
                return;

            callback(Control.Component.Name, Control.Name, state.BoolValue.BoolToSplus());
        }

        #endregion

    }
}