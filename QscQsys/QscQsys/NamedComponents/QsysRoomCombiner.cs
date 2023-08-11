using System.Collections.Generic;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedComponents
{
    public class QsysRoomCombiner : AbstractQsysComponent
    {
        public delegate void WallStateChange(SimplSharpString cName, ushort wall, ushort value);
        public delegate void RoomCombinedChange(SimplSharpString cName, ushort room, ushort value);
        public WallStateChange onWallStateChange { get; set; }
        public RoomCombinedChange onRoomCombinedChange { get; set; }

        // Wall Controls Dictionary
        private readonly Dictionary<int, NamedComponentControl> _wallControls;
        private readonly Dictionary<NamedComponentControl, int> _wallControlsReverse;

        // Room Combined Control Dictionary
        private readonly Dictionary<int, NamedComponentControl> _combineControls;
        private readonly Dictionary<NamedComponentControl, int> _combineControlsReverse;

        public int WallCount { get; private set; }

        public int RoomCount { get; private set; }

        public QsysRoomCombiner()
        {
            _wallControls = new Dictionary<int, NamedComponentControl>();
            _wallControlsReverse = new Dictionary<NamedComponentControl, int>();
            _combineControls = new Dictionary<int, NamedComponentControl>();
            _combineControlsReverse = new Dictionary<NamedComponentControl, int>();
        }

        public void Initialize(string coreId, string componentName, int rooms, int walls)
        {
            WallCount = walls;
            RoomCount = rooms;

            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            lock (_wallControls)
            {
                foreach(var control in _wallControls.Values)
                    UnsubscribeWallControl(control);
                _wallControls.Clear();
                _wallControlsReverse.Clear();
            }

            lock (_combineControls)
            {
                foreach (var control in _combineControls.Values)
                    UnsubscribeRoomCombineControl(control);
                _combineControls.Clear();
                _combineControlsReverse.Clear();
            }

            if (component == null)
                return;

            for (int wallIndex = 1; wallIndex <= WallCount; wallIndex++)
            {
                var control = component.LazyLoadComponentControl(ControlNameUtils.GetRoomCombinerWallOpenName(wallIndex));
                _wallControls.Add(wallIndex, control);
                _wallControlsReverse.Add(control, wallIndex);
                SubscribeWallControl(control);
                UpdateWallState(wallIndex, control.State);
            }

            for (int roomIndex = 1; roomIndex <= RoomCount; roomIndex++)
            {
                var control = component.LazyLoadComponentControl(ControlNameUtils.GetRoomCombinerOutputCombinedName(roomIndex));
                _combineControls.Add(roomIndex, control);
                _combineControlsReverse.Add(control, roomIndex);
                SubscribeRoomCombineControl(control);
                UpdateRoomCombineState(roomIndex, control.State);
            }
        }

        #region Wall Control Callbacks

        private void SubscribeWallControl(NamedComponentControl wallControl)
        {
            if (wallControl == null)
                return;

            wallControl.OnStateChanged += WallControlOnStateChanged;
        }

        private void UnsubscribeWallControl(NamedComponentControl wallControl)
        {
            if (wallControl == null)
                return;

            wallControl.OnStateChanged -= WallControlOnStateChanged;
        }

        private void WallControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            var control = sender as NamedComponentControl;
            
            if (control == null)
                return;

            int index;
            lock (_wallControls)
            {
                if (!_wallControlsReverse.TryGetValue(control, out index))
                    return;
            }

            UpdateWallState(index, args.Data);

        }

        private void UpdateWallState(int wallIndex, QsysStateData state)
        {
            if (state == null)
                return;

            var callback = onWallStateChange;
            if (callback != null)
                callback(ControlNameUtils.GetRoomCombinerWallOpenName(wallIndex), (ushort)wallIndex,
                         state.BoolValue.BoolToSplus());
        }

        #endregion

        #region Room Control Callbacks

        private void SubscribeRoomCombineControl(NamedComponentControl roomCombineControl)
        {
            if (roomCombineControl == null)
                return;

            roomCombineControl.OnStateChanged += RoomCombineControlOnStateChanged;
        }

        private void UnsubscribeRoomCombineControl(NamedComponentControl roomCombineControl)
        {
            if (roomCombineControl == null)
                return;

            roomCombineControl.OnStateChanged -= RoomCombineControlOnStateChanged;
        }

        private void RoomCombineControlOnStateChanged(object sender, QsysInternalEventsArgs args)
        {
            var control = sender as NamedComponentControl;
            if (control == null)
                return;

            int index;
            lock (_combineControls)
            {
                if (!_combineControlsReverse.TryGetValue(control, out index))
                    return;
            }

            UpdateRoomCombineState(index, args.Data);
        }

        private void UpdateRoomCombineState(int roomIndex, QsysStateData state)
        {
            if (state == null)
                return;

            var callback = onRoomCombinedChange;
            if (callback != null)
                callback(ControlNameUtils.GetRoomCombinerOutputCombinedName(roomIndex), (ushort)roomIndex,
                         state.BoolValue.BoolToSplus());
        }

        #endregion

        public void SetWall(int wall, bool state)
        {
            if (Component == null)
                return;

            NamedComponentControl control;

            lock (_wallControls)
            {
                if (!_wallControls.TryGetValue(wall, out control))
                return;
            }

            control.SendChangeBoolValue(state);
        }

        public void SetWall(ushort wall, ushort value)
        {
            SetWall(wall, value.BoolFromSplus());
        }
    }
}