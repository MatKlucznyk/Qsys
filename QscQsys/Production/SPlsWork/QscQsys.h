namespace QscQsys;
        // class declarations
         class QsysProcessorSimplInterface;
         class QsysMatrixMixer;
         class QsysPotsControllerSimpl;
         class QsysFader;
         class QsysEventsArgs;
         class SimplEventArgs;
         class SimplEvents;
         class eQscEventIds;
         class eQscSimplEventIds;
         class QsysMatrixMixerSimpl;
         class QsysFaderSimpl;
         class QsysPotsController;
         class QsysSoftphoneController;
         class GetComponents;
         class ComponentResults;
         class ComponentProperties;
         class CreateChangeGroup;
         class CreateChangeGroupParams;
         class AddControlToChangeGroup;
         class AddComponentToChangeGroupParams;
         class Component;
         class ControlName;
         class Heartbeat;
         class HeartbeatParams;
         class ChangeResult;
         class ComponentChange;
         class ComponentChangeParams;
         class ComponentSetValue;
         class SetCrossPointMute;
         class SetCrossPointMuteParams;
         class ComponentChangeString;
         class ComponentChangeParamsString;
         class ComponentSetValueString;
         class QsysRouterSimpl;
         class QsysProcessor;
         class QsysSoftphoneControllerSimpl;
         class QsysSnapshotSimpl;
         class QsysRouter;
         class QsysSnapshot;
         class QsysCamera;
         class PtzTypes;
     class QsysProcessorSimplInterface 
    {
        // class delegates
        delegate FUNCTION IsRegistered ( INTEGER value );
        delegate FUNCTION IsConnected ( INTEGER value );

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
    };

     class QsysPotsControllerSimpl 
    {
        // class delegates
        delegate FUNCTION OffHookEvent ( INTEGER value );
        delegate FUNCTION RingingEvent ( INTEGER value );
        delegate FUNCTION AutoAnswerEvent ( INTEGER value );
        delegate FUNCTION DndEvent ( INTEGER value );
        delegate FUNCTION DialStringEvent ( SIMPLSHARPSTRING dialString );
        delegate FUNCTION CurrentlyCallingEvent ( SIMPLSHARPSTRING currentlyCalling );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name );
        FUNCTION Dial ( STRING number );
        FUNCTION DialWithoutString ();
        FUNCTION NumPad ( STRING number );
        FUNCTION Disconnect ();
        FUNCTION Redial ();
        FUNCTION AutoAnswerToggle ();
        FUNCTION DndToggle ();
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty OffHookEvent onOffHookEvent;
        DelegateProperty RingingEvent onRingingEvent;
        DelegateProperty AutoAnswerEvent onAutoAnswerEvent;
        DelegateProperty DndEvent onDndEvent;
        DelegateProperty DialStringEvent onDialStringEvent;
        DelegateProperty CurrentlyCallingEvent onCurrentlyCallingEvent;
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
    };

    static class eQscSimplEventIds // enum
    {
        static SIGNED_LONG_INTEGER IsRegistered;
        static SIGNED_LONG_INTEGER NewCommand;
        static SIGNED_LONG_INTEGER IsConnected;
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
        AddComponentToChangeGroupParams ComponentParams;
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

     class QsysRouterSimpl 
    {
        // class delegates
        delegate FUNCTION RouterInputChange ( INTEGER input );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name , INTEGER size );
        FUNCTION SelectInput ( INTEGER input );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty RouterInputChange newRouterInputChange;
    };

    static class QsysProcessor 
    {
        // class delegates

        // class events

        // class functions
        static FUNCTION Initialize ( STRING host , INTEGER port );
        static FUNCTION Debug ( INTEGER value );
        static FUNCTION Dispose ();
        static FUNCTION ParseResponse ( STRING data );
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
    };

     class QsysSoftphoneControllerSimpl 
    {
        // class delegates
        delegate FUNCTION OffHookEvent ( INTEGER value );
        delegate FUNCTION RingingEvent ( INTEGER value );
        delegate FUNCTION AutoAnswerEvent ( INTEGER value );
        delegate FUNCTION DndEvent ( INTEGER value );
        delegate FUNCTION DialStringEvent ( SIMPLSHARPSTRING dialString );
        delegate FUNCTION CurrentlyCallingEvent ( SIMPLSHARPSTRING currentlyCalling );

        // class events

        // class functions
        FUNCTION Initialize ( STRING name );
        FUNCTION Dial ( STRING number );
        FUNCTION DialWithoutString ();
        FUNCTION NumPad ( STRING number );
        FUNCTION Disconnect ();
        FUNCTION Redial ();
        FUNCTION AutoAnswerToggle ();
        FUNCTION DndToggle ();
        STRING_FUNCTION ToString ();
        SIGNED_LONG_INTEGER_FUNCTION GetHashCode ();

        // class variables
        INTEGER __class_id__;

        // class properties
        DelegateProperty OffHookEvent onOffHookEvent;
        DelegateProperty RingingEvent onRingingEvent;
        DelegateProperty AutoAnswerEvent onAutoAnswerEvent;
        DelegateProperty DndEvent onDndEvent;
        DelegateProperty DialStringEvent onDialStringEvent;
        DelegateProperty CurrentlyCallingEvent onCurrentlyCallingEvent;
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

    static class PtzTypes // enum
    {
        static SIGNED_LONG_INTEGER Up;
        static SIGNED_LONG_INTEGER Down;
        static SIGNED_LONG_INTEGER Left;
        static SIGNED_LONG_INTEGER Right;
        static SIGNED_LONG_INTEGER ZoomIn;
        static SIGNED_LONG_INTEGER ZoomOut;
    };

