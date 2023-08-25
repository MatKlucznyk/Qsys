using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public class QsysSignalPresence : AbstractQsysComponent
    {
        private const int INCREMENT_VALUE = 6553;

        public delegate void SignalPresenceChange(SimplSharpString cName, ushort index, ushort value);
        public delegate void PeakThresholdChange(SimplSharpString cName, SimplSharpString value);
        public delegate void HoldTimeChange(SimplSharpString cName, SimplSharpString value);
        public delegate void InfiniteHoldChange(SimplSharpString cName, ushort value);
        public SignalPresenceChange newSignalPresenceChange { get; set; }
        public PeakThresholdChange newPeakThresholdChange { get; set; }
        public HoldTimeChange newHoldTimeChange { get; set; }
        public InfiniteHoldChange newInfiniteHoldChange { get; set; }

        private ushort _count;
        public ushort Threshold { get; private set; }
        public ushort HoldTime { get; private set; }
        public bool InfiniteHoldValue { get; private set; }

        private NamedComponentControl _thresholdControl;
        private NamedComponentControl _holdTimeControl;
        private NamedComponentControl _infiniteHoldControl;
        private readonly Dictionary<NamedComponentControl, int> _signalPresenceControls;

        public NamedComponentControl ThresholdControl
        {
            get { return _thresholdControl; }
            private set
            {
                if (_thresholdControl == value)
                    return;

                UnsubscribeThresholdControl(_thresholdControl);
                _thresholdControl = value;
                SubscribeThresholdControl(_thresholdControl);
            }
        }

        public NamedComponentControl HoldTimeControl
        {
            get { return _holdTimeControl; }
            private set
            {
                if (_holdTimeControl == value)
                    return;

                UnsubscribeHoldTimeControl(_holdTimeControl);
                _holdTimeControl = value;
                SubscribeHoldTimeControl(_holdTimeControl);
            }
        }

        public NamedComponentControl InfiniteHoldControl
        {
            get { return _infiniteHoldControl; }
            private set
            {
                if (_infiniteHoldControl == value)
                    return;

                UnsubscribeInfiniteHoldControl(_infiniteHoldControl);
                _infiniteHoldControl = value;
                SubscribeInfiniteHoldControl(_infiniteHoldControl);
            }
        }

        public QsysSignalPresence()
        {
            _signalPresenceControls = new Dictionary<NamedComponentControl, int>();
        }


        public void Initialize(string coreId, string componentName, ushort count)
        {
            _count = count;
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            lock (_signalPresenceControls)
            {
                ThresholdControl = null;
                HoldTimeControl = null;
                InfiniteHoldControl = null;

                foreach (var control in _signalPresenceControls.Keys)
                    UnsubscribeSignalPresenceControl(control);
                _signalPresenceControls.Clear();

                if (component == null)
                    return;

                ThresholdControl = component.LazyLoadComponentControl(ControlNameUtils.GetThresholdControlName());
                HoldTimeControl = component.LazyLoadComponentControl(ControlNameUtils.GetHoldTimeControlName());
                InfiniteHoldControl = component.LazyLoadComponentControl(ControlNameUtils.GetInfiniteHoldControlName());
                
                for (int i = 1; i <= _count; i++)
                {
                    var control = component.LazyLoadComponentControl(ControlNameUtils.GetSignalPresenceMeterName(i, _count));
                    _signalPresenceControls.Add(control, i);
                    SubscribeSignalPresenceControl(control);
                }
            }
        }

        #region S+ Control

        public void ThresholdIncrement()
        {
            if (ThresholdControl == null)
                return;

            double newThreshold = SimplUtils.ScaleToDouble(Math.Min((Threshold + INCREMENT_VALUE), ushort.MaxValue));
            ThresholdControl.SendChangePosition(newThreshold);
        }

        public void ThresholdDecrement()
        {
            if (ThresholdControl == null)
                return;

            double newThreshold = SimplUtils.ScaleToDouble(Math.Max(Threshold - INCREMENT_VALUE, ushort.MinValue));
            ThresholdControl.SendChangePosition(newThreshold);
        }

        public void HoldTimeIncrement()
        {
            if (HoldTimeControl == null)
                return;

            double newHoldtime = SimplUtils.ScaleToDouble(Math.Min(HoldTime + INCREMENT_VALUE, ushort.MaxValue));
            HoldTimeControl.SendChangePosition(newHoldtime);
        }

        public void HoldTimeDecrement()
        {
            if (HoldTimeControl == null)
                return;

            double newHoldtime = SimplUtils.ScaleToDouble(Math.Max(HoldTime - INCREMENT_VALUE, ushort.MinValue));
            HoldTimeControl.SendChangePosition(newHoldtime);
        }

        public void InfiniteHold(bool value)
        {
            if (InfiniteHoldControl != null)
                InfiniteHoldControl.SendChangeDoubleValue(Convert.ToDouble(value));
        }

        public void InfiniteHold(ushort value)
        {
            if (Component == null)
                return;

            switch (value)
            {
                case (0):
                    InfiniteHold(false);
                    break;
                case (1):
                    InfiniteHold(true);
                    break;
            }
        }

        #endregion

        #region Threshold Control Callbacks

        private void SubscribeThresholdControl(NamedComponentControl thresholdControl)
        {
            if (thresholdControl == null)
                return;

            thresholdControl.OnStateChanged += ThresholdControlOnStateChanged;
        }

        private void UnsubscribeThresholdControl(NamedComponentControl thresholdControl)
        {
            if (thresholdControl == null)
                return;

            thresholdControl.OnStateChanged -= ThresholdControlOnStateChanged;
        }

        private void ThresholdControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            Threshold = SimplUtils.ScaleToUshort(args.Position);

            var callback = newPeakThresholdChange;
            if (callback != null)
                callback(ComponentName, args.StringValue);
        }

        #endregion

        #region HoldTime Control Callbacks

        private void SubscribeHoldTimeControl(NamedComponentControl holdTimeControl)
        {
            if (holdTimeControl == null)
                return;

            holdTimeControl.OnStateChanged += HoldTimeControlOnStateChanged;
        }

        private void UnsubscribeHoldTimeControl(NamedComponentControl holdTimeControl)
        {
            if (holdTimeControl == null)
                return;

            holdTimeControl.OnStateChanged -= HoldTimeControlOnStateChanged;
        }

        private void HoldTimeControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            HoldTime = SimplUtils.ScaleToUshort(args.Position);

            var callback = newHoldTimeChange;
            if (callback != null)
                callback(ComponentName, args.StringValue);
        }

        #endregion

        #region Infinite Hold Control Callbacks

        private void SubscribeInfiniteHoldControl(NamedComponentControl infiniteHoldControl)
        {
            if (infiniteHoldControl == null)
                return;

            infiniteHoldControl.OnStateChanged += InfiniteHoldControlOnStateChanged;
        }

        private void UnsubscribeInfiniteHoldControl(NamedComponentControl infiniteHoldControl)
        {
            if (infiniteHoldControl == null)
                return;

            infiniteHoldControl.OnStateChanged -= InfiniteHoldControlOnStateChanged;
        }

        private void InfiniteHoldControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            InfiniteHoldValue = args.BoolValue;

            var callback = newInfiniteHoldChange;
            if (callback != null)
                callback(ComponentName, InfiniteHoldValue.BoolToSplus());
        }

        #endregion

        #region Signal Presence Control Callbacks

        private void SubscribeSignalPresenceControl(NamedComponentControl signalPresenceControl)
        {
            if (signalPresenceControl == null)
                return;

            signalPresenceControl.OnStateChanged += SignalPresenceControlOnStateChanged;
        }

        private void UnsubscribeSignalPresenceControl(NamedComponentControl signalPresenceControl)
        {
            if (signalPresenceControl == null)
                return;

            signalPresenceControl.OnStateChanged -= SignalPresenceControlOnStateChanged;
        }

        private void SignalPresenceControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            var control = sender as NamedComponentControl;
            
            if (control == null)
                return;

            int index;
            lock (_signalPresenceControls)
            {
                if (!_signalPresenceControls.TryGetValue(control, out index))
                    return;
            }

            var callback = newSignalPresenceChange;
            if (callback != null)
                callback(ComponentName, (ushort)index, args.BoolValue.BoolToSplus());
        }

        #endregion
    }
}