using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysRoomCombiner
    {
        private string name;
        private bool registered;
        private bool[] wallState;
        private bool[] roomCombined;

        public event EventHandler<QsysEventsArgs> QsysRoomCombinerEvent;

        public string ComponentName { get { return name; } }
        public bool IsRegistered { get { return registered; } }
        public bool[] WallState { get { return wallState; } }
        public bool[] RoomCombined { get { return roomCombined; } }


        public QsysRoomCombiner(string name, int rooms, int walls)
        {
            this.name = name;

            wallState = new bool[walls];
            roomCombined = new bool[rooms];

            Component component = new Component() { Name = name, Controls = new List<ControlName>()};

            for (int i = 1; i <= walls; i++)
            {
                component.Controls.Add(new ControlName { Name = string.Format("wall_{0}_open", i) });
            }

            for (int i = 1; i <= rooms; i++)
            {
                component.Controls.Add(new ControlName { Name = string.Format("output_{0}_combined", i) });   
            }

            if (QsysProcessor.RegisterComponent(component))
            {
                QsysProcessor.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(QsysRoomCombiner_OnNewEvent);

                registered = true;
            }
        }

        void QsysRoomCombiner_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name.Contains("open"))
            {
                var wall = e.Name.Split('_');

                wallState[Convert.ToInt16(wall[1]) - 1] = Convert.ToBoolean(e.Value);

                QsysRoomCombinerEvent(this, new QsysEventsArgs(eQscEventIds.RoomCombinerWallStateChange, name, Convert.ToBoolean(e.Value), Convert.ToInt16(wall[1]), e.SValue, null));
            }
            else if (e.Name.Contains("combined"))
            {
                var room = e.Name.Split('_');

                roomCombined[Convert.ToInt16(room[1]) - 1] = Convert.ToBoolean(e.Value);

                QsysRoomCombinerEvent(this, new QsysEventsArgs(eQscEventIds.RoomCombinerCombinedStateChange, name, Convert.ToBoolean(e.Value), Convert.ToInt16(room[1]), e.SValue, null));
            }
        }

        public void SetWall(int wall, bool state)
        {
            if (wallState.Length >= wall)
            {
                if (wallState[wall - 1] != state)
                {
                    ComponentChange newState = new ComponentChange { Params = new ComponentChangeParams { Name = name, Controls = new List<ComponentSetValue> { new ComponentSetValue { Name = string.Format("wall_{0}_open", wall), Value = Convert.ToDouble(state) } } } };

                    QsysProcessor.Enqueue(JsonConvert.SerializeObject(newState, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
            }
        }
    }
}