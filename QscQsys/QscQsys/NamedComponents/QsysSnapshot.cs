using System;
using System.Collections.Generic;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public class QsysSnapshot : AbstractQsysComponent
    {
        public delegate void SnapshotUpdate(SimplSharpString cName, ushort snapshot);

        public SnapshotUpdate onRecalledSnapshot { get; set; }
        public SnapshotUpdate onUnrecalledSnapshot { get; set; }

        private readonly Dictionary<NamedComponentControl, int> _snapshotControls;

        public int SnapshotCount { get; private set; }

        public QsysSnapshot()
        {
            _snapshotControls = new Dictionary<NamedComponentControl, int>();
        }

        public void Initialize(string coreId, string componentName, ushort snapshotCount)
        {
            SnapshotCount = snapshotCount;
            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            lock (_snapshotControls)
            {
                foreach (var control in _snapshotControls.Keys)
                    UnsubscribeSnapshotControl(control);
                _snapshotControls.Clear();

                if (component == null)
                    return;

                for (int i = 1; i <= SnapshotCount; i++)
                {
                    var control = component.LazyLoadComponentControl(ControlNameUtils.GetSnapshotLoadControlName(i));
                    _snapshotControls.Add(control, i);
                    SubscribeSnapshotControl(control);
                }
            }
        }

        #region Snapshot Control Callbacks

        private void SubscribeSnapshotControl(NamedComponentControl snapshotControl)
        {
            if (snapshotControl == null)
                return;

            snapshotControl.OnFeedbackReceived += SnapshotControlOnFeedbackReceived;
        }

        private void UnsubscribeSnapshotControl(NamedComponentControl snapshotControl)
        {
            if (snapshotControl == null)
                return;

            snapshotControl.OnFeedbackReceived -= SnapshotControlOnFeedbackReceived;
        }

        private void SnapshotControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            var control = sender as NamedComponentControl;
            if (control == null)
                return;

            int index;

            lock (_snapshotControls)
            {
                if (!_snapshotControls.TryGetValue(control, out index))
                    return;
            }

            bool state = Math.Abs(args.Position - 1.0) < QsysCore.TOLERANCE;

            var callback = state ? onRecalledSnapshot : onUnrecalledSnapshot;
            if (callback != null)
                callback(ComponentName, (ushort)index);

        }

        #endregion

        public void LoadSnapshot(ushort number)
        {
            SendComponentChangeDoubleValue(ControlNameUtils.GetSnapshotLoadControlName(number), 1);
        }

        public void SaveSnapshot(ushort number)
        {
            SendComponentChangeDoubleValue(ControlNameUtils.GetSnapshotSaveControlName(number), 1);
        }
    }
}