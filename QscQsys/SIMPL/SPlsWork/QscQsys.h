namespace QscQsys;
        // class declarations
         class QsysNamedControlSimpl;
         class QsysPotsControllerSimpl;
         class QsysNv32hDecoder;
         class QsysNamedControl;
         class QsysRouter;
         class QsysPotsController;
         class QsysSoftphoneController;
         class QsysProcessorSimplInterface;
         class QsysFaderSimpl;
         class QsysRouterSimpl;
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
         class QsysSoftphoneControllerSimpl;
         class QsysMeterSimpl;
         class QsysMeter;
         class QsysCamera;
         class PtzTypes;
         class QsysNv32hDecoderSimpl;
         class QsysSnapshotSimpl;
         class QsysSnapshot;
         class QsysFader;
         class QsysEventsArgs;
         class SimplEventArgs;
         class SimplEvents;
         class eQscEventIds;
         class eQscSimplEventIds;
         class QsysMatrixMixerSimpl;
         class QsysProcessor;
         class QsysMatrixMixer;
     class QsysNamedControlSimpl 
    {
        // class delegates
        delegate FUNCTION NamedControlChange ( INTEGER intData , SIMPLSHARPSTRING stringData );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name , INTEGER isInteger );
        FUNCTION SetInteger ( INTEGER value );
        FUNCTION SetString ( STRING value );
        FUNCTION SetBoolean ( INTEGER value );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty NamedControlChange newNamedControlChange;
    };

     class QsysPotsControllerSimpl 
    {
        // class delegates
        delegate FUNCTION OffHookEvent ( INTEGER value );
        delegate FUNCTION RingingEvent ( INTEGER value );
        delegate FUNCTION DialingEvent ( INTEGER value );
        delegate FUNCTION AutoAnswerEvent ( INTEGER value );
        delegate FUNCTION DndEvent ( INTEGER value );
        delegate FUNCTION DialStringEvent ( SIMPLSHARPSTRING dialString );
        delegate FUNCTION CurrentlyCallingEvent ( SIMPLSHARPSTRING currentlyCalling );
        delegate FUNCTION CurrentCallStatus ( SIMPLSHARPSTRING callStatus );
        delegate FUNCTION RecentCallsEvent ( SIMPLSHARPSTRING item1 , SIMPLSHARPSTRING item2 , SIMPLSHARPSTRING item3 , SIMPLSHARPSTRING item4 , SIMPLSHARPSTRING item5 );
        delegate FUNCTION RecentCallListEvent ( SIMPLSHARPSTRING xsig );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name );
        FUNCTION Dial ( STRING number );
        FUNCTION DialWithoutString ();
        FUNCTION NumPad ( STRING number );
        FUNCTION NumPadDelete ();
        FUNCTION NumPadClear ();
        FUNCTION Connect ();
        FUNCTION Disconnect ();
        FUNCTION Redial ();
        FUNCTION AutoAnswerToggle ();
        FUNCTION DndToggle ();
        FUNCTION SelectRecentCall ( INTEGER index );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty OffHookEvent onOffHookEvent;
        DelegateProperty RingingEvent onRingingEvent;
        DelegateProperty DialingEvent onDialingEvent;
        DelegateProperty AutoAnswerEvent onAutoAnswerEvent;
        DelegateProperty DndEvent onDndEvent;
        DelegateProperty DialStringEvent onDialStringEvent;
        DelegateProperty CurrentlyCallingEvent onCurrentlyCallingEvent;
        DelegateProperty CurrentCallStatus onCurrentCallStatusChange;
        DelegateProperty RecentCallsEvent onRecentCallsEvent;
        DelegateProperty RecentCallListEvent onRecentCallListEvent;
    };

     class QsysProcessorSimplInterface 
    {
        // class delegates
        delegate FUNCTION IsRegistered ( INTEGER value );
        delegate FUNCTION IsConnected ( INTEGER value );
        delegate FUNCTION CoreStatus ( SIMPLSHARPSTRING designName , INTEGER isRedundant , INTEGER isEmulator );

        // class events

        // class functions
        FUNCTION Register ( STRING id );
        FUNCTION Debug ( INTEGER value );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty IsRegistered onIsRegistered;
        DelegateProperty IsConnected onIsConnected;
        DelegateProperty CoreStatus onNewCoreStatus;
    };

     class QsysFaderSimpl 
    {
        // class delegates
        delegate FUNCTION VolumeChange ( INTEGER value );
        delegate FUNCTION MuteChange ( INTEGER value );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name );
        FUNCTION Volume ( INTEGER value );
        FUNCTION Mute ( INTEGER value );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty VolumeChange newVolumeChange;
        DelegateProperty MuteChange newMuteChange;
    };

     class QsysRouterSimpl 
    {
        // class delegates
        delegate FUNCTION RouterInputChange ( INTEGER input );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name , INTEGER output );
        FUNCTION SelectInput ( INTEGER input );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty RouterInputChange newRouterInputChange;
    };

     class GetComponents 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class ComponentResults 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class CreateChangeGroupParams 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class AddComoponentToChangeGroup 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class AddComponentToChangeGroupParams 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class ChangeResult 
    {
        // class delegates

        // class events

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

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
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        STRING Text[];
        STRING Color[];
        STRING Icon[];
    };

     class QsysSoftphoneControllerSimpl 
    {
        // class delegates
        delegate FUNCTION OffHookEvent ( INTEGER value );
        delegate FUNCTION RingingEvent ( INTEGER value );
        delegate FUNCTION DialingEvent ( INTEGER value );
        delegate FUNCTION AutoAnswerEvent ( INTEGER value );
        delegate FUNCTION DndEvent ( INTEGER value );
        delegate FUNCTION DialStringEvent ( SIMPLSHARPSTRING dialString );
        delegate FUNCTION CurrentlyCallingEvent ( SIMPLSHARPSTRING currentlyCalling );
        delegate FUNCTION CurrentCallStatus ( SIMPLSHARPSTRING callStatus );
        delegate FUNCTION RecentCallsEvent ( SIMPLSHARPSTRING item1 , SIMPLSHARPSTRING item2 , SIMPLSHARPSTRING item3 , SIMPLSHARPSTRING item4 , SIMPLSHARPSTRING item5 );
        delegate FUNCTION RecentCallListEvent ( SIMPLSHARPSTRING xsig );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name );
        FUNCTION Dial ( STRING number );
        FUNCTION DialWithoutString ();
        FUNCTION NumPad ( STRING number );
        FUNCTION NumPadDelete ();
        FUNCTION NumPadClear ();
        FUNCTION Connect ();
        FUNCTION Disconnect ();
        FUNCTION Redial ();
        FUNCTION AutoAnswerToggle ();
        FUNCTION DndToggle ();
        FUNCTION SelectRecentCall ( INTEGER index );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty OffHookEvent onOffHookEvent;
        DelegateProperty RingingEvent onRingingEvent;
        DelegateProperty DialingEvent onDialingEvent;
        DelegateProperty AutoAnswerEvent onAutoAnswerEvent;
        DelegateProperty DndEvent onDndEvent;
        DelegateProperty DialStringEvent onDialStringEvent;
        DelegateProperty CurrentlyCallingEvent onCurrentlyCallingEvent;
        DelegateProperty CurrentCallStatus onCurrentCallStatusChange;
        DelegateProperty RecentCallsEvent onRecentCallsEvent;
        DelegateProperty RecentCallListEvent onRecentCallListEvent;
    };

     class QsysMeterSimpl 
    {
        // class delegates
        delegate FUNCTION MeterChange ( INTEGER meterValue );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name , INTEGER index );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty MeterChange onMeterChange;
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

     class QsysNv32hDecoderSimpl 
    {
        // class delegates
        delegate FUNCTION Nv32hDecoderInputChange ( INTEGER input );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name );
        FUNCTION ChangeSource ( INTEGER input );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty Nv32hDecoderInputChange newNv32hDecoderInputChange;
    };

     class QsysSnapshotSimpl 
    {
        // class delegates

        // class events

        // class functions
        FUNCTION Initialize ( STRING name );
        FUNCTION LoadSnapshot ( INTEGER number );
        FUNCTION SaveSnapshot ( INTEGER number );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class SimplEvents 
    {
        // class delegates

        // class events
        EventHandler OnNewEvent ( SimplEvents sender, SimplEventArgs e );

        // class functions
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

    static class eQscEventIds // enum
    {
        static SIGNED_LONG_INTEGER NewCommand;
        static SIGNED_LONG_INTEGER GainChange;
        static SIGNED_LONG_INTEGER MuteChange;
        static SIGNED_LONG_INTEGER NewMaxGain;
        static SIGNED_LONG_INTEGER NewMinGain;
        static SIGNED_LONG_INTEGER CameraStreamChange;
        static SIGNED_LONG_INTEGER PotsControllerOffHook;
        static SIGNED_LONG_INTEGER PotsControllerIsRinging;
        static SIGNED_LONG_INTEGER PotsControllerDialString;
        static SIGNED_LONG_INTEGER PotsControllerCurrentlyCalling;
        static SIGNED_LONG_INTEGER RouterInputSelected;
        static SIGNED_LONG_INTEGER PotsControllerAutoAnswerChange;
        static SIGNED_LONG_INTEGER PotsControllerDND_Change;
        static SIGNED_LONG_INTEGER Nv32hDecoderInputChange;
        static SIGNED_LONG_INTEGER MeterUpdate;
        static SIGNED_LONG_INTEGER NamedControlChange;
        static SIGNED_LONG_INTEGER PotsControllerCallStatusChange;
        static SIGNED_LONG_INTEGER PotsControllerRecentCallsChange;
        static SIGNED_LONG_INTEGER PotsControllerDialing;
    };

    static class eQscSimplEventIds // enum
    {
        static SIGNED_LONG_INTEGER IsRegistered;
        static SIGNED_LONG_INTEGER NewCommand;
        static SIGNED_LONG_INTEGER IsConnected;
        static SIGNED_LONG_INTEGER NewCoreStatus;
    };

     class QsysMatrixMixerSimpl 
    {
        // class delegates
        delegate FUNCTION CrossPointValueChange ( INTEGER value );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name , INTEGER input , INTEGER output );
        FUNCTION SetCrossPoint ( INTEGER value );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty CrossPointValueChange newCrossPointValueChange;
    };

    static class QsysProcessor 
    {
        // class delegates

        // class events

        // class functions
        static FUNCTION Initialize ( STRING host , INTEGER port );
        static FUNCTION Debug ( INTEGER value );
        static FUNCTION ParseResponse ( STRING data );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        INTEGER IsDebugMode;
        STRING DesignName[];
    };

