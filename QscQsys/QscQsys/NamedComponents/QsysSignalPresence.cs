using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public class QsysSignalPresence : AbstractQsysComponent
    {
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

        #region Threshold Control Callbacks

        private void SubscribeThresholdControl(NamedComponentControl thresholdControl)
        {
            if (thresholdControl == null)
                return;

            thresholdControl.OnFeedbackReceived += ThresholdControlOnFeedbackReceived;
        }

        private void UnsubscribeThresholdControl(NamedComponentControl thresholdControl)
        {
            if (thresholdControl == null)
                return;

            thresholdControl.OnFeedbackReceived -= ThresholdControlOnFeedbackReceived;
        }

        private void ThresholdControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            Threshold = (ushort)Math.Round(QsysCoreManager.ScaleUp(args.Position));

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

            holdTimeControl.OnFeedbackReceived += HoldTimeControlOnFeedbackReceived;
        }

        private void UnsubscribeHoldTimeControl(NamedComponentControl holdTimeControl)
        {
            if (holdTimeControl == null)
                return;

            holdTimeControl.OnFeedbackReceived -= HoldTimeControlOnFeedbackReceived;
        }

        private void HoldTimeControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            HoldTime = (ushort)Math.Round(QsysCoreManager.ScaleUp(args.Position));

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

            infiniteHoldControl.OnFeedbackReceived += InfiniteHoldControlOnFeedbackReceived;
        }

        private void UnsubscribeInfiniteHoldControl(NamedComponentControl infiniteHoldControl)
        {
            if (infiniteHoldControl == null)
                return;

            infiniteHoldControl.OnFeedbackReceived -= InfiniteHoldControlOnFeedbackReceived;
        }

        private void InfiniteHoldControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            InfiniteHoldValue = Convert.ToBoolean(args.Value);

            var callback = newInfiniteHoldChange;
            if (callback != null)
                callback(ComponentName, Convert.ToUInt16(args.Value));
        }

        #endregion

        #region Signal Presence Control Callbacks

        private void SubscribeSignalPresenceControl(NamedComponentControl signalPresenceControl)
        {
            if (signalPresenceControl == null)
                return;

            signalPresenceControl.OnFeedbackReceived += SignalPresenceControlOnFeedbackReceived;
        }

        private void UnsubscribeSignalPresenceControl(NamedComponentControl signalPresenceControl)
        {
            if (signalPresenceControl == null)
                return;

            signalPresenceControl.OnFeedbackReceived -= SignalPresenceControlOnFeedbackReceived;
        }

        private void SignalPresenceControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
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
                callback(ComponentName, (ushort)index, Convert.ToUInt16(args.Value));
        }

        #endregion

        public void ThresholdIncrement()
        {
            if (Component == null)
                return;

            double newThreshold;

            if ((Threshold + 6553.5) <= 65535)
            {
                newThreshold = QsysCoreManager.ScaleDown(Threshold + 6553.5);
            }
            else
            {
                newThreshold = QsysCoreManager.ScaleDown(65535);
            }

            SendComponentChangePosition("threshold", newThreshold);
        }

        public void ThresholdDecrement()
        {
            if (Component == null)
                return;

            double newThreshold;

            if ((Threshold - 6553.5) >= 0)
            {
                newThreshold = QsysCoreManager.ScaleDown(Threshold - 6553.5);
            }
            else
            {
                newThreshold = QsysCoreManager.ScaleDown(0);
            }

            SendComponentChangePosition("threshold", newThreshold);
        }

        public void HoldTimeIncrement()
        {
            if (Component == null)
                return;

            double newHoldtime;

            if ((HoldTime + 6553.5) <= 65535)
            {
                newHoldtime = QsysCoreManager.ScaleDown(HoldTime + 6553.5);
            }
            else
            {
                newHoldtime = QsysCoreManager.ScaleDown(65535);
            }

            SendComponentChangePosition("hold_time", newHoldtime);
        }

        public void HoldTimeDecrement()
        {
            if (Component == null)
                return;

            double newHoldtime;

            if ((HoldTime - 6553.5) >= 0)
            {
                newHoldtime = QsysCoreManager.ScaleDown(HoldTime - 6553.5);
            }
            else
            {
                newHoldtime = QsysCoreManager.ScaleDown(0);
            }

            SendComponentChangePosition("hold_time", newHoldtime);
        }

        public void InfiniteHold(bool value)
        {
            if (Component == null)
                return;

            SendComponentChangeDoubleValue("infinite_hold", Convert.ToDouble(value));
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
    }
}