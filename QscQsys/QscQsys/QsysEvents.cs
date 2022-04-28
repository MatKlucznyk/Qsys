using System;
using System.Collections.Generic;
using System.Linq;
using Crestron.SimplSharp;

namespace QscQsys
{
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

    internal class CoreAddedEventArgs : EventArgs
    {
        public string CoreId;

        public CoreAddedEventArgs(string coreId)
        {
            this.CoreId = coreId;
        }
    }

    internal class CoreRemovedEventArgs : EventArgs
    {
        public string CoreId;

        public CoreRemovedEventArgs(string coreId)
        {
            this.CoreId = coreId;
        }
    }
}