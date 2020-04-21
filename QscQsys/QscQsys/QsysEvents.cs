using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;

namespace QscQsys
{
    /// <summary>
    /// Event args for Q-Sys events.
    /// </summary>
    public class QsysEventsArgs : EventArgs
    {
        public eQscEventIds EventID;
        public string ControlName;
        public bool BooleanValue;
        public int IntegerValue;
        public string StringValue;
        public List<ListBoxChoice> ListValue;

        /// <summary>
        /// Default constructor for QsysEventArgs.
        /// </summary>
        /// <param name="eventID">The event ID that is associated with the event.</param>
        /// <param name="controlName">The control that has triggered the event.</param>
        /// <param name="booleanValue">Boolean value of the event.</param>
        /// <param name="integerValue">Integer value of the event.</param>
        /// <param name="stringValue">String value of the event.</param>
        public QsysEventsArgs(eQscEventIds eventID, string controlName, bool booleanValue, int integerValue, string stringValue, List<ListBoxChoice> listValue)
        {
            this.EventID = eventID;
            this.ControlName = controlName;
            this.BooleanValue = booleanValue;
            this.IntegerValue = integerValue;
            this.StringValue = stringValue;
            this.ListValue = listValue;
        }

    }

    /// <summary>
    /// Used only for internal methods.
    /// </summary>
    internal class QsysInternalEventsArgs : EventArgs
    {
        public string Name;
        public double Value;
        public double Position;
        public string SValue;
        public List<string> Choices;

        public QsysInternalEventsArgs(string name, double data, double position, string sData, List<string> choices)
        {
            this.Name = name;
            this.Value = data;
            this.Position = position;
            this.SValue = sData;
            this.Choices = choices;

        }
    }

    /// <summary>
    /// Used only for internal methods.
    /// </summary>
    internal class InternalEvents
    {
        private event EventHandler<QsysInternalEventsArgs> onNewEvent = delegate { };

        public event EventHandler<QsysInternalEventsArgs> OnNewEvent
        {
            add
            {
                if (!onNewEvent.GetInvocationList().Contains(value))
                {
                    onNewEvent += value;
                }
            }
            remove
            {
                onNewEvent -= value;
            }
        }

        internal void Fire(QsysInternalEventsArgs e)
        {
            onNewEvent(null, e);
        }
    }

    public class SimplEventArgs : EventArgs
    {
        public SimplSharpString StringData;
        public ushort IntData;
        public eQscSimplEventIds ID;

        public SimplEventArgs(eQscSimplEventIds id, SimplSharpString stringData, ushort intData)
        {
            this.StringData = stringData;
            this.IntData = intData;
            this.ID = id;
        }
    }

    public class SimplEvents
    {
        private event EventHandler<SimplEventArgs> onNewEvent = delegate { };

        public event EventHandler<SimplEventArgs> OnNewEvent
        {
            add
            {
                if (!onNewEvent.GetInvocationList().Contains(value))
                {
                    onNewEvent += value;
                }
            }
            remove
            {
                onNewEvent -= value;
            }
        }

        internal void Fire(SimplEventArgs e)
        {
            onNewEvent(null, e);
        }
    }

    /// <summary>
    /// Event IDs.
    /// </summary>
    public enum eQscEventIds
    {
        /// <summary>
        /// New command event ID.
        /// </summary>
        NewCommand = 1,

        /// <summary>
        /// New gain change event ID.
        /// </summary>
        GainChange = 2,

        /// <summary>
        /// New mute change event ID.
        /// </summary>
        MuteChange = 3,

        NewMaxGain = 4,

        NewMinGain = 5,

        CameraStreamChange = 6,

        PotsControllerOffHook = 7,

        PotsControllerIsRinging = 8,

        PotsControllerDialString = 9,

        PotsControllerCurrentlyCalling = 10,

        RouterInputSelected = 11,

        PotsControllerAutoAnswerChange = 12,

        PotsControllerDND_Change = 13,

        Nv32hDecoderInputChange = 14,
        
        MeterUpdate = 15,

        NamedControlChange = 16,

        PotsControllerCallStatusChange = 17,

        PotsControllerRecentCallsChange = 18,
        
        PotsControllerDialing = 19,

        PotsControllerIncomingCall = 20,

        RoomCombinerWallStateChange = 21,
        
        RoomCombinerCombinedStateChange = 22
    }

    public enum eQscSimplEventIds
    {
        IsRegistered = 1,
        NewCommand = 2,
        IsConnected = 3,
        NewCoreStatus = 4
    }
}