using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysRoomCombiner : QsysComponent
    {
        public delegate void WallStateChange(SimplSharpString cName, ushort wall, ushort value);
        public delegate void RoomCombinedChange(SimplSharpString cName, ushort room, ushort value);
        public WallStateChange onWallStateChange { get; set; }
        public RoomCombinedChange onRoomCombinedChange { get; set; }

        private bool[] _wallState;
        private bool[] _roomCombined;

        public bool[] WallState { get { return _wallState; } }
        public bool[] RoomCombined { get { return _roomCombined; } }


        public void Initialize(string coreId, string componentName, int rooms, int walls)
        {
            _wallState = new bool[walls];
            _roomCombined = new bool[rooms];

            var component = new Component(true) { Name = componentName, Controls = new List<ControlName>() };

            for (int i = 1; i <= _wallState.Length; i++)
            {
                component.Controls.Add(new ControlName { Name = string.Format("wall_{0}_open", i) });
            }

            for (int i = 1; i <= _roomCombined.Length; i++)
            {
                component.Controls.Add(new ControlName { Name = string.Format("output_{0}_combined", i) });
            }

            base.Initialize(coreId, component);
        }

        protected override void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name.Contains("open"))
            {
                var wall = e.Name.Split('_');

                _wallState[Convert.ToInt16(wall[1]) - 1] = Convert.ToBoolean(e.Value);

                //QsysRoomCombinerEvent(this, new QsysEventsArgs(eQscEventIds.RoomCombinerWallStateChange, cName, Convert.ToBoolean(e.Value), Convert.ToInt16(wall[1]), e.SValue, null));

                if (onWallStateChange != null)
                    onWallStateChange(_cName, Convert.ToUInt16(wall[1]), Convert.ToUInt16(e.Value));
            }
            else if (e.Name.Contains("combined"))
            {
                var room = e.Name.Split('_');

                _roomCombined[Convert.ToInt16(room[1]) - 1] = Convert.ToBoolean(e.Value);

                if (onRoomCombinedChange != null)
                    onRoomCombinedChange(_cName, Convert.ToUInt16(room[1]), Convert.ToUInt16(e.Value));
            }
        }

        public void SetWall(int wall, bool state)
        {
            if (_registered)
            {
                if (_wallState.Length >= wall)
                {
                    if (_wallState[wall - 1] != state)
                    {
                        SendComponentChangeDoubleValue(string.Format("wall_{0}_open", wall), Convert.ToDouble(state));
                    }
                }
            }
        }

        public void SetWall(ushort wall, ushort value)
        {
            SetWall(wall, Convert.ToBoolean(value));
        }
    }
}