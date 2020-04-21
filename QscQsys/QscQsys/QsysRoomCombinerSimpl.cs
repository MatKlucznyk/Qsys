using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;

namespace QscQsys
{
    public class QsysRoomCombinerSimpl
    {
        private QsysRoomCombiner roomCombiner;

        public delegate void WallStateChange(ushort wall, ushort value);
        public delegate void RoomCombinedChange(ushort room, ushort value);
        public WallStateChange onWallStateChange { get; set; }
        public RoomCombinedChange onRoomCombinedChange { get; set; }

        public void Initialize(string name, ushort walls, ushort rooms)
        {
            roomCombiner = new QsysRoomCombiner(name, rooms, walls);

            roomCombiner.QsysRoomCombinerEvent += new EventHandler<QsysEventsArgs>(roomCombiner_QsysRoomCombinerEvent);
        }

        void roomCombiner_QsysRoomCombinerEvent(object sender, QsysEventsArgs e)
        {
            switch (e.EventID)
            {
                case eQscEventIds.NewCommand:
                    break;
                case eQscEventIds.GainChange:
                    break;
                case eQscEventIds.MuteChange:
                    break;
                case eQscEventIds.NewMaxGain:
                    break;
                case eQscEventIds.NewMinGain:
                    break;
                case eQscEventIds.CameraStreamChange:
                    break;
                case eQscEventIds.PotsControllerOffHook:
                    break;
                case eQscEventIds.PotsControllerIsRinging:
                    break;
                case eQscEventIds.PotsControllerDialString:
                    break;
                case eQscEventIds.PotsControllerCurrentlyCalling:
                    break;
                case eQscEventIds.RouterInputSelected:
                    break;
                case eQscEventIds.PotsControllerAutoAnswerChange:
                    break;
                case eQscEventIds.PotsControllerDND_Change:
                    break;
                case eQscEventIds.Nv32hDecoderInputChange:
                    break;
                case eQscEventIds.MeterUpdate:
                    break;
                case eQscEventIds.NamedControlChange:
                    break;
                case eQscEventIds.PotsControllerCallStatusChange:
                    break;
                case eQscEventIds.PotsControllerRecentCallsChange:
                    break;
                case eQscEventIds.PotsControllerDialing:
                    break;
                case eQscEventIds.PotsControllerIncomingCall:
                    break;
                case eQscEventIds.RoomCombinerWallStateChange:
                    if (onWallStateChange != null)
                    {
                        onWallStateChange(Convert.ToUInt16(e.IntegerValue), Convert.ToUInt16(e.BooleanValue));
                    }
                    break;
                case eQscEventIds.RoomCombinerCombinedStateChange:
                    if (onRoomCombinedChange != null)
                    {
                        onRoomCombinedChange(Convert.ToUInt16(e.IntegerValue), Convert.ToUInt16(e.BooleanValue));
                    }
                    break;
                default:
                    break;
            }
        }

        public void SetWall(ushort wall, ushort value)
        {
            switch (value)
            {
                case(1):
                    roomCombiner.SetWall(wall, true);
                    break;
                case(0):
                    roomCombiner.SetWall(wall, false);
                    break;
            }
        }
    }
}