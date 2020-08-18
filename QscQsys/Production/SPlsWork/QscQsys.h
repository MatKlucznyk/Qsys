namespace QscQsys;
        // class declarations
         class QsysMatrixMixer;
         class QsysRoomCombiner;
         class QsysCore;
         class QsysPotsController;
         class QsysMeter;
         class QsysSoftphoneController;
         class QsysRouter;
         class QsysMatrixMixerCrosspoint;
         class GetComponents;
         class ComponentResults;
         class ComponentProperties;
         class CreateChangeGroup;
         class CreateChangeGroupParams;
         class AddComoponentToChangeGroup;
         class AddControlToChangeGroup;
         class AddControlToChangeGroupParams;
         class AddComponentToChangeGroupParams;
         class Component;
         class Control;
         class ControlName;
         class Heartbeat;
         class HeartbeatParams;
         class ChangeResult;
         class ComponentChange;
         class ControlIntegerChange;
         class ControlStringChange;
         class ComponentChangeParams;
         class ControlIntegerParams;
         class ControlStringParams;
         class ComponentSetValue;
         class SetCrossPointMute;
         class SetCrossPointMuteParams;
         class ComponentChangeString;
         class ComponentChangeParamsString;
         class ComponentSetValueString;
         class ListBoxChoice;
         class Logon;
         class LogonParams;
         class QsysSnapshot;
         class QsysFader;
         class QsysNv32hDecoder;
         class QsysNamedControl;
         class QsysCamera;
         class PtzTypes;
         class QsysSignalPresence;
     class QsysMatrixMixer 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class QsysRoomCombiner 
    {
        // class delegates
        delegate FUNCTION WallStateChange ( INTEGER wall , INTEGER value );
        delegate FUNCTION RoomCombinedChange ( INTEGER room , INTEGER value );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING name , SIGNED_LONG_INTEGER rooms , SIGNED_LONG_INTEGER walls );
        FUNCTION SetWall ( INTEGER wall , INTEGER value );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty WallStateChange onWallStateChange;
        DelegateProperty RoomCombinedChange onRoomCombinedChange;
        STRING ComponentName[];
    };

     class QsysCore 
    {
        // class delegates
        delegate FUNCTION IsLoggedIn ( INTEGER value );
        delegate FUNCTION IsRegistered ( INTEGER value );
        delegate FUNCTION IsConnectedStatus ( INTEGER value );
        delegate FUNCTION CoreStatus ( SIMPLSHARPSTRING designName , INTEGER isRedundant , INTEGER isEmulator );

        // class events

        // class functions
        FUNCTION Debug ( INTEGER value );
        FUNCTION Initialize ( STRING id , STRING host , INTEGER port , STRING username , STRING password );
        FUNCTION Dispose ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty IsLoggedIn onIsLoggedIn;
        DelegateProperty IsRegistered onIsRegistered;
        DelegateProperty IsConnectedStatus onIsConnected;
        DelegateProperty CoreStatus onNewCoreStatus;
        INTEGER IsDebugMode;
        INTEGER MaxLogonAttemps;
        STRING DesignName[];
        STRING CoreId[];
    };

     class QsysPotsController 
    {
        // class delegates
        delegate FUNCTION OffHookEvent ( INTEGER value );
        delegate FUNCTION RingingEvent ( INTEGER value );
        delegate FUNCTION DialingEvent ( INTEGER value );
        delegate FUNCTION IncomingCallEvent ( INTEGER value );
        delegate FUNCTION AutoAnswerEvent ( INTEGER value );
        delegate FUNCTION DndEvent ( INTEGER value );
        delegate FUNCTION DialStringEvent ( SIMPLSHARPSTRING dialString );
        delegate FUNCTION CurrentlyCallingEvent ( SIMPLSHARPSTRING currentlyCalling );
        delegate FUNCTION CurrentCallStatus ( SIMPLSHARPSTRING callStatus );
        delegate FUNCTION RecentCallsEvent ( SIMPLSHARPSTRING item1 , SIMPLSHARPSTRING item2 , SIMPLSHARPSTRING item3 , SIMPLSHARPSTRING item4 , SIMPLSHARPSTRING item5 );
        delegate FUNCTION RecentCallListEvent ( SIMPLSHARPSTRING xsig );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name );
        FUNCTION NumPad ( STRING number );
        FUNCTION NumString ( STRING number );
        FUNCTION NumPadDelete ();
        FUNCTION NumPadClear ();
        FUNCTION Dial ();
        FUNCTION Connect ();
        FUNCTION Disconnect ();
        FUNCTION Redial ();
        FUNCTION AutoAnswerToggle ();
        FUNCTION DndToggle ();
        FUNCTION SelectRecentCall ( SIGNED_LONG_INTEGER index );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty OffHookEvent onOffHookEvent;
        DelegateProperty RingingEvent onRingingEvent;
        DelegateProperty DialingEvent onDialingEvent;
        DelegateProperty IncomingCallEvent onIncomingCallEvent;
        DelegateProperty AutoAnswerEvent onAutoAnswerEvent;
        DelegateProperty DndEvent onDndEvent;
        DelegateProperty DialStringEvent onDialStringEvent;
        DelegateProperty CurrentlyCallingEvent onCurrentlyCallingEvent;
        DelegateProperty CurrentCallStatus onCurrentCallStatusChange;
        DelegateProperty RecentCallsEvent onRecentCallsEvent;
        DelegateProperty RecentCallListEvent onRecentCallListEvent;
        STRING ComponentName[];
        STRING DialString[];
        STRING CurrentlyCalling[];
        STRING LastNumberCalled[];
        STRING CallStatus[];
    };

     class QsysMeter 
    {
        // class delegates
        delegate FUNCTION MeterChange ( INTEGER meterValue );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING name , SIGNED_LONG_INTEGER index );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty MeterChange onMeterChange;
    };

     class QsysSoftphoneController 
    {
        // class delegates
        delegate FUNCTION OffHookEvent ( INTEGER value );
        delegate FUNCTION RingingEvent ( INTEGER value );
        delegate FUNCTION DialingEvent ( INTEGER value );
        delegate FUNCTION IncomingCallEvent ( INTEGER value );
        delegate FUNCTION AutoAnswerEvent ( INTEGER value );
        delegate FUNCTION DndEvent ( INTEGER value );
        delegate FUNCTION DialStringEvent ( SIMPLSHARPSTRING dialString );
        delegate FUNCTION CurrentlyCallingEvent ( SIMPLSHARPSTRING currentlyCalling );
        delegate FUNCTION CurrentCallStatus ( SIMPLSHARPSTRING callStatus );
        delegate FUNCTION RecentCallsEvent ( SIMPLSHARPSTRING item1 , SIMPLSHARPSTRING item2 , SIMPLSHARPSTRING item3 , SIMPLSHARPSTRING item4 , SIMPLSHARPSTRING item5 );
        delegate FUNCTION RecentCallListEvent ( SIMPLSHARPSTRING xsig );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name );
        FUNCTION NumPad ( STRING number );
        FUNCTION NumString ( STRING number );
        FUNCTION NumPadDelete ();
        FUNCTION NumPadClear ();
        FUNCTION Dial ();
        FUNCTION Connect ();
        FUNCTION Disconnect ();
        FUNCTION Redial ();
        FUNCTION AutoAnswerToggle ();
        FUNCTION DndToggle ();
        FUNCTION SelectRecentCall ( SIGNED_LONG_INTEGER index );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty OffHookEvent onOffHookEvent;
        DelegateProperty RingingEvent onRingingEvent;
        DelegateProperty DialingEvent onDialingEvent;
        DelegateProperty IncomingCallEvent onIncomingCallEvent;
        DelegateProperty AutoAnswerEvent onAutoAnswerEvent;
        DelegateProperty DndEvent onDndEvent;
        DelegateProperty DialStringEvent onDialStringEvent;
        DelegateProperty CurrentlyCallingEvent onCurrentlyCallingEvent;
        DelegateProperty CurrentCallStatus onCurrentCallStatusChange;
        DelegateProperty RecentCallsEvent onRecentCallsEvent;
        DelegateProperty RecentCallListEvent onRecentCallListEvent;
        STRING ComponentName[];
        STRING DialString[];
        STRING CurrentlyCalling[];
        STRING LastNumberCalled[];
        STRING CallStatus[];
    };

     class QsysRouter 
    {
        // class delegates
        delegate FUNCTION RouterInputChange ( INTEGER input );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name , SIGNED_LONG_INTEGER output );
        FUNCTION InputSelect ( SIGNED_LONG_INTEGER input );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty RouterInputChange newRouterInputChange;
        STRING ComponentName[];
        SIGNED_LONG_INTEGER CurrentSelectedInput;
    };

     class QsysMatrixMixerCrosspoint 
    {
        // class delegates
        delegate FUNCTION CrossPointValueChange ( INTEGER value );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING name , INTEGER input , INTEGER output );
        FUNCTION SetCrossPoint ( INTEGER value );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty CrossPointValueChange newCrossPointValueChange;
    };

     class GetComponents 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class ComponentResults 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
        STRING Type[];
    };

     class ComponentProperties 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
        STRING Value[];
    };

     class CreateChangeGroup 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class CreateChangeGroupParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class AddComoponentToChangeGroup 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING method[];
        AddComponentToChangeGroupParams ComponentParams;
    };

     class AddControlToChangeGroup 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING method[];
        AddControlToChangeGroupParams ControlParams;
    };

     class AddControlToChangeGroupParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class AddComponentToChangeGroupParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        Component Component;
    };

     class Component 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
    };

     class Control 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
    };

     class ControlName 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
    };

     class Heartbeat 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        static STRING jsonrpc[];
        static STRING method[];

        // class properties
    };

     class HeartbeatParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class ChangeResult 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Component[];
        STRING Name[];
        STRING String[];
    };

     class ComponentChange 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        ComponentChangeParams Params;
    };

     class ControlIntegerChange 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        ControlIntegerParams Params;
    };

     class ControlStringChange 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        ControlStringParams Params;
    };

     class ComponentChangeParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
    };

     class ControlIntegerParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
    };

     class ControlStringParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
        STRING Value[];
    };

     class ComponentSetValue 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
    };

     class SetCrossPointMute 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        SetCrossPointMuteParams Params;
    };

     class SetCrossPointMuteParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
        STRING Inputs[];
        STRING Outputs[];
    };

     class ComponentChangeString 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        ComponentChangeParamsString Params;
    };

     class ComponentChangeParamsString 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
    };

     class ComponentSetValueString 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Name[];
        STRING Value[];
    };

     class ListBoxChoice 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Text[];
        STRING Color[];
        STRING Icon[];
    };

     class Logon 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        LogonParams Params;
    };

     class LogonParams 
    {
        // class delegates

        // class events

        // class functions
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING User[];
        STRING Password[];
    };

     class QsysSnapshot 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING name );
        FUNCTION LoadSnapshot ( INTEGER number );
        FUNCTION SaveSnapshot ( INTEGER number );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class QsysFader 
    {
        // class delegates
        delegate FUNCTION VolumeChange ( INTEGER value );
        delegate FUNCTION MuteChange ( INTEGER value );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name );
        FUNCTION Volume ( SIGNED_LONG_INTEGER value );
        FUNCTION Mute ( INTEGER value );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty VolumeChange newVolumeChange;
        DelegateProperty MuteChange newMuteChange;
        STRING ComponentName[];
        SIGNED_LONG_INTEGER CurrentVolume;
    };

     class QsysNv32hDecoder 
    {
        // class delegates
        delegate FUNCTION Nv32hDecoderInputChange ( INTEGER input );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name );
        FUNCTION ChangeInput ( SIGNED_LONG_INTEGER source );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty Nv32hDecoderInputChange newNv32hDecoderInputChange;
        STRING ComponentName[];
        SIGNED_LONG_INTEGER CurrentSource;
    };

     class QsysNamedControl 
    {
        // class delegates
        delegate FUNCTION NamedControlChange ( INTEGER intData , SIMPLSHARPSTRING stringData );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name , INTEGER type );
        FUNCTION SetInteger ( SIGNED_LONG_INTEGER value , SIGNED_LONG_INTEGER scaled );
        FUNCTION SetString ( STRING value );
        FUNCTION SetBoolean ( SIGNED_LONG_INTEGER value );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty NamedControlChange newNamedControlChange;
        STRING ComponentName[];
    };

     class QsysCamera 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name );
        FUNCTION StartPTZ ( PtzTypes type );
        FUNCTION StopPTZ ( PtzTypes type );
        FUNCTION RecallHome ();
        FUNCTION TiltUp ();
        FUNCTION StopTiltUp ();
        FUNCTION TiltDown ();
        FUNCTION StopTiltDown ();
        FUNCTION PanLeft ();
        FUNCTION StopPanLeft ();
        FUNCTION PanRight ();
        FUNCTION StopPanRight ();
        FUNCTION ZoomIn ();
        FUNCTION StopZoomIn ();
        FUNCTION ZoomOut ();
        FUNCTION StopZoomOut ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

    static class PtzTypes // enum
    {
        static SIGNED_LONG_INTEGER Up;
        static SIGNED_LONG_INTEGER Down;
        static SIGNED_LONG_INTEGER Left;
        static SIGNED_LONG_INTEGER Right;
        static SIGNED_LONG_INTEGER ZoomIn;
        static SIGNED_LONG_INTEGER ZoomOut;
    };

     class QsysSignalPresence 
    {
        // class delegates
        delegate FUNCTION SignalPresenceChange ( INTEGER index , INTEGER value );
        delegate FUNCTION PeakThresholdChange ( SIMPLSHARPSTRING value );
        delegate FUNCTION HoldTimeChange ( SIMPLSHARPSTRING value );
        delegate FUNCTION InfiniteHoldChange ( INTEGER value );

        // class events

        // class functions
        FUNCTION Initialize ( STRING coreId , STRING Name , INTEGER Count );
        FUNCTION ThresholdIncrement ();
        FUNCTION ThresholdDecrement ();
        FUNCTION HoldTimeIncrement ();
        FUNCTION HoldTimeDecrement ();
        FUNCTION InfiniteHold ( INTEGER value );
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();
        STRING_FUNCTION ToString ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty SignalPresenceChange newSignalPresenceChange;
        DelegateProperty PeakThresholdChange newPeakThresholdChange;
        DelegateProperty HoldTimeChange newHoldTimeChange;
        DelegateProperty InfiniteHoldChange newInfiniteHoldChange;
        STRING ComponentName[];
    };

