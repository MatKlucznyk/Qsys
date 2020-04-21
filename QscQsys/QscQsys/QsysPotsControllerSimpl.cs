using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.SimplSharpExtensions;

namespace QscQsys
{
    public class QsysPotsControllerSimpl
    {
        public delegate void OffHookEvent(ushort value);
        public delegate void RingingEvent(ushort value);
        public delegate void DialingEvent(ushort value);
        public delegate void IncomingCallEvent(ushort value);
        public delegate void AutoAnswerEvent(ushort value);
        public delegate void DndEvent(ushort value);
        public delegate void DialStringEvent(SimplSharpString dialString);
        public delegate void CurrentlyCallingEvent(SimplSharpString currentlyCalling);
        public delegate void CurrentCallStatus(SimplSharpString callStatus);
        public delegate void RecentCallsEvent(SimplSharpString item1, SimplSharpString item2, SimplSharpString item3, SimplSharpString item4, SimplSharpString item5);
        public delegate void RecentCallListEvent(SimplSharpString xsig);
        public OffHookEvent onOffHookEvent { get; set; }
        public RingingEvent onRingingEvent { get; set; }
        public DialingEvent onDialingEvent { get; set; }
        public IncomingCallEvent onIncomingCallEvent { get; set; }
        public AutoAnswerEvent onAutoAnswerEvent { get; set; }
        public DndEvent onDndEvent { get; set; }
        public DialStringEvent onDialStringEvent { get; set; }
        public CurrentlyCallingEvent onCurrentlyCallingEvent { get; set; }
        public CurrentCallStatus onCurrentCallStatusChange { get; set; }
        public RecentCallsEvent onRecentCallsEvent { get; set; }
        public RecentCallListEvent onRecentCallListEvent { get; set; }

        private QsysPotsController pots;

        private List<string> fullRecentCallList;

        public void Initialize(string name)
        {
            pots = new QsysPotsController(name);
            fullRecentCallList = new List<string>();
            pots.QsysPotsControllerEvent += new EventHandler<QsysEventsArgs>(softphone_QsysPotsControllerEvent);
        }

        void softphone_QsysPotsControllerEvent(object sender, QsysEventsArgs e)
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
                    if (onOffHookEvent != null)
                        onOffHookEvent(Convert.ToUInt16(e.IntegerValue));
                    break;
                case eQscEventIds.PotsControllerIsRinging:
                    if (onRingingEvent != null)
                        onRingingEvent(Convert.ToUInt16(e.IntegerValue));
                    break;
                case eQscEventIds.PotsControllerDialString:
                    if (onDialStringEvent != null)
                        onDialStringEvent(e.StringValue);
                    break;
                case eQscEventIds.PotsControllerCurrentlyCalling:
                    if (onCurrentlyCallingEvent != null)
                        onCurrentlyCallingEvent(e.StringValue);
                    break;
                case eQscEventIds.RouterInputSelected:
                    break;
                case eQscEventIds.PotsControllerAutoAnswerChange:
                    if (onAutoAnswerEvent != null)
                        onAutoAnswerEvent(Convert.ToUInt16(e.BooleanValue));
                    break;
                case eQscEventIds.PotsControllerDND_Change:
                    if(onDndEvent != null)
                        onDndEvent(Convert.ToUInt16(e.BooleanValue));
                    break;
                case eQscEventIds.PotsControllerCallStatusChange:
                    if (onCurrentCallStatusChange != null)
                        onCurrentCallStatusChange(e.StringValue);
                    break;
                case eQscEventIds.PotsControllerRecentCallsChange:
                    if (onRecentCallsEvent != null)
                    {
                        List<string> calls = new List<string>();

                        foreach (var call in e.ListValue)
                        {
                            fullRecentCallList.Add(call.Text);
                        }

                        if(e.ListValue.Count > 0)
                            calls.Add(e.ListValue[0].Text);
                        else
                            calls.Add(string.Empty);

                        if (e.ListValue.Count > 1)
                            calls.Add(e.ListValue[1].Text);
                        else
                            calls.Add(string.Empty);

                        if (e.ListValue.Count > 2)
                            calls.Add(e.ListValue[2].Text);
                        else
                            calls.Add(string.Empty);

                        if (e.ListValue.Count > 3)
                            calls.Add(e.ListValue[3].Text);
                        else
                            calls.Add(string.Empty);

                        if (e.ListValue.Count > 4)
                            calls.Add(e.ListValue[4].Text);
                        else
                            calls.Add(string.Empty);

                        onRecentCallsEvent(calls[0], calls[1], calls[2], calls[3], calls[4]);
                    }
                    if (onRecentCallListEvent != null)
                    {
                        List<string> calls = new List<string>();

                        foreach (var call in calls)
                        {
                            var encodedBytes = XSig.GetBytes(calls.IndexOf(call), call);
                            onRecentCallListEvent(Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                        }
                    }
                    break;
                case eQscEventIds.PotsControllerDialing:
                    if (e.BooleanValue && onDialingEvent != null)
                        onDialingEvent(1);
                    else if (onDialingEvent != null)
                        onDialingEvent(0);
                    break;
                case eQscEventIds.PotsControllerIncomingCall:
                    if (e.BooleanValue && onIncomingCallEvent != null)
                        onIncomingCallEvent(1);
                    else if (onIncomingCallEvent != null)
                        onIncomingCallEvent(0);
                    break;
                default:
                    break;
            }
        }

        public void Dial(string number)
        {
            pots.Dial(number);
        }

        public void DialWithoutString()
        {
            pots.Dial();
        }

        public void NumPad(string number)
        {
            pots.NumPad(number);
        }

        public void NumString(string number)
        {
            pots.NumString(number);
        }

        public void NumPadDelete()
        {
            pots.NumPadDelete();
        }

        public void NumPadClear()
        {
            pots.NumPadClear();
        }

        public void Connect()
        {
            pots.Connect();
        }

        public void Disconnect()
        {
            pots.Disconnect();
        }

        public void Redial()
        {
            pots.Redial();
        }

        public void AutoAnswerToggle()
        {
            pots.AutoAnswerToggle();
        }

        public void DndToggle()
        {
            pots.DndToggle();
        }

        public void SelectRecentCall(ushort index)
        {
            pots.SelectRecentCall(index);
        }

    }
}