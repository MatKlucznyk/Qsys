using Crestron.SimplSharp;
using JetBrains.Annotations;
using QscQsys.Utils;

namespace QscQsys.NamedComponents.GenericComponentControl
{
    /// <summary>
    ///Control an integer control as part of any named component 
    /// </summary>
    public sealed class IntegerGenericComponentControl : AbstractGenericComponentControl
    {

        public delegate void ControlIntegerChangedDelegate(
            SimplSharpString componentName, SimplSharpString controlName, ushort data);

        public delegate void ControlSignedIntegerChangedDelegate(
            SimplSharpString componentName, SimplSharpString controlName, short data);

        [PublicAPI("S+")]
        public ControlIntegerChangedDelegate UnsignedDataChangedCallback { get; set; }

        [PublicAPI("S+")]
        public ControlSignedIntegerChangedDelegate SignedDataChangedCallback { get; set; }

        

        [PublicAPI("S+")]
        public IntegerGenericComponentControl()
        {}

        #region Set Values

        [PublicAPI("S+")]
        public void SetUnsignedInteger(ushort value, ushort scaled)
        {
            if (Control == null)
                return;

            if (scaled == 1)
                Control.SendChangePosition(SimplUtils.ScaleToDouble(value));
            else
                Control.SendChangeDoubleValue(value);
        }

        [PublicAPI("S+")]
        public void SetSignedInteger(int value, ushort scaled)
        {
            if (Control == null)
                return;

            if (scaled == 1)
                Control.SendChangePosition(SimplUtils.ScaleToDouble(value));
            else
                Control.SendChangeDoubleValue(value);
        }

        #endregion

        #region Update States

        protected override void UpdateState(QsysStateData state)
        {
            base.UpdateState(state);

            if (state == null)
                return;

            switch (state.Type)
            {
                case "position":
                    FireUnsignedCallback(state);
                    break;
                case "value":
                    FireSignedCallback(state);
                    break;
                case "change":
                    FireUnsignedCallback(state);
                    FireSignedCallback(state);
                    break;
            }
        }

        private void FireUnsignedCallback(QsysStateData state)
        {
            var unsignedCallback = UnsignedDataChangedCallback;

            if (unsignedCallback != null)
                unsignedCallback(Control.Component.Name, Control.Name, SimplUtils.ScaleToUshort(state.Position));
        }

        private void FireSignedCallback(QsysStateData state)
        {
            var signedCallback = SignedDataChangedCallback;

            if (signedCallback != null)
                signedCallback(Control.Component.Name, Control.Name, (short)state.Value);
        }

        #endregion

    }
}