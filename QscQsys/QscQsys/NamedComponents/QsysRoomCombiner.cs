using System;
using System.Linq;
using Crestron.SimplSharp;
using QscQsys.Intermediaries;

namespace QscQsys.NamedComponents
{
    public class QsysRoomCombiner : AbstractQsysComponent
    {
        public delegate void WallStateChange(SimplSharpString cName, ushort wall, ushort value);
        public delegate void RoomCombinedChange(SimplSharpString cName, ushort room, ushort value);
        public WallStateChange onWallStateChange { get; set; }
        public RoomCombinedChange onRoomCombinedChange { get; set; }

        private int _walls;
        private int _rooms;
        private bool[] _wallState;
        private bool[] _roomCombined;

        public bool[] WallState { get { return _wallState.ToArray(); } }
        public bool[] RoomCombined { get { return _roomCombined.ToArray(); } }


        public void Initialize(string coreId, string componentName, int rooms, int walls)
        {
            _walls = walls;
            _rooms = rooms;
            _wallState = new bool[walls];
            _roomCombined = new bool[rooms];

            InternalInitialize(coreId, componentName);
        }

        protected override void HandleComponentUpdated(NamedComponent component)
        {
            base.HandleComponentUpdated(component);

            if (component == null)
                return;

            for (int wall = 1; wall <= _walls; wall++)
            {
                component.LazyLoadComponentControl(string.Format("wall_{0}_open", wall));
            }

            for (int room = 1; room <= _rooms; room++)
            {
                component.LazyLoadComponentControl(string.Format("output_{0}_combined", room));
            }
        }

        protected override void ComponentOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            base.ComponentOnFeedbackReceived(sender, args);

            if (args.Name.Contains("open"))
            {
                var wall = args.Name.Split('_');

                _wallState[Convert.ToInt16(wall[1]) - 1] = Convert.ToBoolean(args.Value);

                if (onWallStateChange != null)
                    onWallStateChange(ComponentName, Convert.ToUInt16(wall[1]), Convert.ToUInt16(args.Value));
            }
            else if (args.Name.Contains("combined"))
            {
                var room = args.Name.Split('_');

                _roomCombined[Convert.ToInt16(room[1]) - 1] = Convert.ToBoolean(args.Value);

                if (onRoomCombinedChange != null)
                    onRoomCombinedChange(ComponentName, Convert.ToUInt16(room[1]), Convert.ToUInt16(args.Value));
            }
        }

        public void SetWall(int wall, bool state)
        {
            if (Component == null)
                return;

            if (_walls < wall)
                return;

            if (_wallState[wall - 1] != state)
            {
                SendComponentChangeDoubleValue(string.Format("wall_{0}_open", wall), Convert.ToDouble(state));
            }
        }

        public void SetWall(ushort wall, ushort value)
        {
            SetWall(wall, Convert.ToBoolean(value));
        }
    }
}