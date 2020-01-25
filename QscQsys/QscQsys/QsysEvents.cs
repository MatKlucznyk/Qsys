using System;
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
        public double NumberValue;
        public string StringValue;

        /// <summary>
        /// Default constructor for QsysEventArgs.
        /// </summary>
        /// <param name="eventID">The event ID that is associated with the event.</param>
        /// <param name="controlName">The control that has triggered the event.</param>
        /// <param name="booleanValue">Boolean value of the event.</param>
        /// <param name="integerValue">Double value of the event.</param>
        /// <param name="stringValue">String value of the event.</param>
        public QsysEventsArgs(eQscEventIds eventID, string controlName, bool booleanValue, double numberValue, string stringValue)
        {
            this.EventID = eventID;
            this.ControlName = controlName;
            this.BooleanValue = booleanValue;
            this.NumberValue = numberValue;
            this.StringValue = stringValue;
        }

    }

    /// <summary>
    /// Used only for internal methods.
    /// </summary>
    internal class QsysInternalEventsArgs : EventArgs
    {
        public ChangeResult changeResult;

        public QsysInternalEventsArgs(ChangeResult _changeResult)
        {
            this.changeResult = _changeResult;
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

        PotsControllerCID = 10,

        RouterInputSelected = 11,

        PotsControllerAutoAnswerChange = 12,

        PotsControllerDND_Change = 13,

        Nv32hDecoderInputChange = 14,
        
        MeterUpdate = 15,

        CameraChange = 16,

        NamedControl = 17
    }

    public enum eQscSimplEventIds
    {
        IsRegistered = 1,
        NewCommand = 2,
        IsConnected = 3,
        CoreState = 4,
        Platform = 5,
        DesignName = 6,
        DesignCode = 7,
        IsRedundant = 8,
        IsEmulator = 9,
        StatusCode = 10,
        StatusString = 11
    }
}