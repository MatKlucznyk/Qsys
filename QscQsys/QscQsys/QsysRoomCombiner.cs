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
        public delegate void WallStateChange(SimplSharpString cName, ushort wall, ushort value);
        public delegate void RoomCombinedChange(SimplSharpString cName, ushort room, ushort value);
        public WallStateChange onWallStateChange { get; set; }
        public RoomCombinedChange onRoomCombinedChange { get; set; }

        private string cName;
        private string coreId;
        private bool registered;
        private bool[] wallState;
        private bool[] roomCombined;

        //public event EventHandler<QsysEventsArgs> QsysRoomCombinerEvent;

        public string ComponentName { get { return cName; } }
        public bool IsRegistered { get { return registered; } }
        public bool[] WallState { get { return wallState; } }
        public bool[] RoomCombined { get { return roomCombined; } }


        public void Initialize(string coreId, string name, int rooms, int walls)
        {
            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);

            this.cName = name;
            this.coreId = coreId;

            wallState = new bool[walls];
            roomCombined = new bool[rooms];

            if(!registered)
                RegisterWithCore();
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                RegisterWithCore();
            }
        }

        private void RegisterWithCore()
        {
            if (QsysCoreManager.Cores.ContainsKey(coreId))
            {
                Component component = new Component() { Name = cName, Controls = new List<ControlName>() };

                for (int i = 1; i <= wallState.Length; i++)
                {
                    component.Controls.Add(new ControlName { Name = string.Format("wall_{0}_open", i) });
                }

                for (int i = 1; i <= roomCombined.Length; i++)
                {
                    component.Controls.Add(new ControlName { Name = string.Format("output_{0}_combined", i) });
                }

                if (QsysCoreManager.Cores[coreId].RegisterComponent(component))
                {
                    QsysCoreManager.Cores[coreId].Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Component_OnNewEvent);

                    registered = true;
                }
            }
        }

        void Component_OnNewEvent(object sender, QsysInternalEventsArgs e)
        {
            if (e.Name.Contains("open"))
            {
                var wall = e.Name.Split('_');

                wallState[Convert.ToInt16(wall[1]) - 1] = Convert.ToBoolean(e.Value);

                //QsysRoomCombinerEvent(this, new QsysEventsArgs(eQscEventIds.RoomCombinerWallStateChange, cName, Convert.ToBoolean(e.Value), Convert.ToInt16(wall[1]), e.SValue, null));

                if (onWallStateChange != null)
                    onWallStateChange(cName, Convert.ToUInt16(wall[1]), Convert.ToUInt16(e.Value));
            }
            else if (e.Name.Contains("combined"))
            {
                var room = e.Name.Split('_');

                roomCombined[Convert.ToInt16(room[1]) - 1] = Convert.ToBoolean(e.Value);

                //QsysRoomCombinerEvent(this, new QsysEventsArgs(eQscEventIds.RoomCombinerCombinedStateChange, cName, Convert.ToBoolean(e.Value), Convert.ToInt16(room[1]), e.SValue, null));

                if (onRoomCombinedChange != null)
                    onRoomCombinedChange(cName, Convert.ToUInt16(room[1]), Convert.ToUInt16(e.Value));
            }
        }

        public void SetWall(int wall, bool state)
        {
            if (registered)
            {
                if (wallState.Length >= wall)
                {
                    if (wallState[wall - 1] != state)
                    {
                        ComponentChange newState = new ComponentChange { Params = new ComponentChangeParams { Name = cName, Controls = new List<ComponentSetValue> { new ComponentSetValue { Name = string.Format("wall_{0}_open", wall), Value = Convert.ToDouble(state) } } } };

                        QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(newState, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                    }
                }
            }
        }

        public void SetWall(ushort wall, ushort value)
        {
            switch (value)
            {
                case (1):
                    SetWall(wall, true);
                    break;
                case (0):
                    SetWall(wall, false);
                    break;
            }
        }
    }
}