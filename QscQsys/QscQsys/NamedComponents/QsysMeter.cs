using System;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public sealed class QsysMeter : AbstractQsysComponent
    {
        public delegate void MeterChange(SimplSharpString cName, ushort meterValue);
        public MeterChange onMeterChange { get; set; }

        private int _meterIndex;
        private NamedComponentControl _meter;

        public NamedComponentControl Meter
        {
            get { return _meter; }
            private set
            {
                if (_meter == value)
                    return;

                UnsubscribeMeter(_meter);
                _meter = value;
                SubscribeMeter(_meter);
            }
        }

        public void Initialize(string coreId, string componentName, int meterIndex)
        {
            _meterIndex = meterIndex;
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
            {
                Meter = null;
                return;
            }

            Meter = component.LazyLoadComponentControl(ControlNameUtils.GetMeterName(_meterIndex));
        }

        #region Meter Control Callbacks

        private void SubscribeMeter(NamedComponentControl meter)
        {
            if (meter == null)
                return;

            meter.OnFeedbackReceived += MeterOnFeedbackReceived;
        }

        private void UnsubscribeMeter(NamedComponentControl meter)
        {
            if (meter == null)
                return;

            meter.OnFeedbackReceived += MeterOnFeedbackReceived;
        }

        private void MeterOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            var callback = onMeterChange;
            if (callback == null)
                return;

            double value = SimplUtils.ScaleToUshort(args.Position);
            callback(ControlNameUtils.GetMeterName(_meterIndex), Convert.ToUInt16(value));
        }

        #endregion
    }
}