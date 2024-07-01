using Crestron.SimplSharp;
using JetBrains.Annotations;

namespace QscQsys.NamedComponents.GenericComponentControl
{
    public sealed class StringGenericComponentControl : AbstractGenericComponentControl
    {
        public delegate void NamedControlStringChange(SimplSharpString cNmae, SimplSharpString stringData);

        [PublicAPI("S+")]
        public NamedControlStringChange newNamedControlStringChange { get; set; }

        public StringGenericComponentControl()
        {}


        [PublicAPI("S+")]
        public void SetString(string value)
        {
            if (Control == null)
                return;

            Control.SendChangeStringValue(value);
        }

        protected override void UpdateState(QsysStateData state)
        {
            base.UpdateState(state);

            if (state == null)
                return;

            var stringCallback = newNamedControlStringChange;
            if (stringCallback != null)
                stringCallback(ControlName, state.StringValue);
        }
    }
}