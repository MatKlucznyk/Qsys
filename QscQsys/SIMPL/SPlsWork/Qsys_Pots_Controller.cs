using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Linq;
using Crestron;
using Crestron.Logos.SplusLibrary;
using Crestron.Logos.SplusObjects;
using Crestron.SimplSharp;
using QscQsys;
using ExtensionMethods;
using TCP_Client;

namespace UserModule_QSYS_POTS_CONTROLLER
{
    public class UserModuleClass_QSYS_POTS_CONTROLLER : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        
        
        
        Crestron.Logos.SplusObjects.DigitalInput DIAL;
        Crestron.Logos.SplusObjects.DigitalInput REDIAL;
        Crestron.Logos.SplusObjects.DigitalInput DISCONNECT;
        Crestron.Logos.SplusObjects.DigitalInput AUTOANSWERTOGGLE;
        Crestron.Logos.SplusObjects.DigitalInput DNDTOGGLE;
        InOutArray<Crestron.Logos.SplusObjects.DigitalInput> KEYPAD;
        Crestron.Logos.SplusObjects.StringInput DIALSTRING;
        Crestron.Logos.SplusObjects.DigitalOutput CONNECTED;
        Crestron.Logos.SplusObjects.DigitalOutput RINGING;
        Crestron.Logos.SplusObjects.DigitalOutput AUTOANSWERSTATUS;
        Crestron.Logos.SplusObjects.DigitalOutput DNDSTATUS;
        Crestron.Logos.SplusObjects.StringOutput DIALSTRINGOUT;
        Crestron.Logos.SplusObjects.StringOutput CURRENTLYCALLING;
        Crestron.Logos.SplusObjects.StringOutput CALLSTATUS;
        QscQsys.QsysPotsControllerSimpl POTS;
        StringParameter COMPONENTNAME;
        object DIAL_OnPush_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 21;
                if ( Functions.TestForTrue  ( ( Functions.Length( DIALSTRING ))  ) ) 
                    { 
                    __context__.SourceCodeLine = 23;
                    POTS . Dial ( DIALSTRING .ToString()) ; 
                    } 
                
                else 
                    { 
                    __context__.SourceCodeLine = 27;
                    POTS . DialWithoutString ( ) ; 
                    } 
                
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    object REDIAL_OnPush_1 ( Object __EventInfo__ )
    
        { 
        Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
        try
        {
            SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
            
            __context__.SourceCodeLine = 33;
            POTS . Redial ( ) ; 
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
object DISCONNECT_OnPush_2 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 38;
        POTS . Disconnect ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object AUTOANSWERTOGGLE_OnPush_3 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 43;
        POTS . AutoAnswerToggle ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object DNDTOGGLE_OnPush_4 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 48;
        POTS . DndToggle ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object KEYPAD_OnPush_5 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        ushort X = 0;
        
        
        __context__.SourceCodeLine = 55;
        X = (ushort) ( Functions.GetLastModifiedArrayIndex( __SignalEventArg__ ) ) ; 
        __context__.SourceCodeLine = 57;
        POTS . NumPad ( Functions.ItoA( (int)( (X - 1) ) ) .ToString()) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

public void NEWOFFHOOKEVENT ( ushort VALUE ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 62;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 63;
            CONNECTED  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 65;
            CONNECTED  .Value = (ushort) ( 0 ) ; 
            }
        
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWRINGINGEVENT ( ushort VALUE ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 70;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 71;
            RINGING  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 73;
            RINGING  .Value = (ushort) ( 0 ) ; 
            }
        
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWAUTOANSWEREVENT ( ushort VALUE ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 78;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 79;
            AUTOANSWERSTATUS  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 81;
            AUTOANSWERSTATUS  .Value = (ushort) ( 0 ) ; 
            }
        
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWDNDEVENT ( ushort VALUE ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 86;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 87;
            DNDSTATUS  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 89;
            DNDSTATUS  .Value = (ushort) ( 0 ) ; 
            }
        
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWDIALSTRINGEVENT ( SimplSharpString NEWDIALSTRING ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 94;
        DIALSTRINGOUT  .UpdateValue ( NEWDIALSTRING  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWCURRENTLYCALLINGEVENT ( SimplSharpString NEWCURRENTLYCALLING ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 99;
        CURRENTLYCALLING  .UpdateValue ( NEWCURRENTLYCALLING  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWCURRENTCALLSTATUSCHANGE ( SimplSharpString STATUS ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 104;
        CALLSTATUS  .UpdateValue ( STATUS  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public override object FunctionMain (  object __obj__ ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusFunctionMainStartCode();
        
        __context__.SourceCodeLine = 109;
        // RegisterDelegate( POTS , ONOFFHOOKEVENT , NEWOFFHOOKEVENT ) 
        POTS .onOffHookEvent  = NEWOFFHOOKEVENT; ; 
        __context__.SourceCodeLine = 110;
        // RegisterDelegate( POTS , ONRINGINGEVENT , NEWRINGINGEVENT ) 
        POTS .onRingingEvent  = NEWRINGINGEVENT; ; 
        __context__.SourceCodeLine = 111;
        // RegisterDelegate( POTS , ONAUTOANSWEREVENT , NEWAUTOANSWEREVENT ) 
        POTS .onAutoAnswerEvent  = NEWAUTOANSWEREVENT; ; 
        __context__.SourceCodeLine = 112;
        // RegisterDelegate( POTS , ONDNDEVENT , NEWDNDEVENT ) 
        POTS .onDndEvent  = NEWDNDEVENT; ; 
        __context__.SourceCodeLine = 113;
        // RegisterDelegate( POTS , ONDIALSTRINGEVENT , NEWDIALSTRINGEVENT ) 
        POTS .onDialStringEvent  = NEWDIALSTRINGEVENT; ; 
        __context__.SourceCodeLine = 114;
        // RegisterDelegate( POTS , ONCURRENTLYCALLINGEVENT , NEWCURRENTLYCALLINGEVENT ) 
        POTS .onCurrentlyCallingEvent  = NEWCURRENTLYCALLINGEVENT; ; 
        __context__.SourceCodeLine = 115;
        // RegisterDelegate( POTS , ONCURRENTCALLSTATUSCHANGE , NEWCURRENTCALLSTATUSCHANGE ) 
        POTS .onCurrentCallStatusChange  = NEWCURRENTCALLSTATUSCHANGE; ; 
        __context__.SourceCodeLine = 116;
        POTS . Initialize ( COMPONENTNAME  .ToString()) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    return __obj__;
    }
    

public override void LogosSplusInitialize()
{
    _SplusNVRAM = new SplusNVRAM( this );
    
    DIAL = new Crestron.Logos.SplusObjects.DigitalInput( DIAL__DigitalInput__, this );
    m_DigitalInputList.Add( DIAL__DigitalInput__, DIAL );
    
    REDIAL = new Crestron.Logos.SplusObjects.DigitalInput( REDIAL__DigitalInput__, this );
    m_DigitalInputList.Add( REDIAL__DigitalInput__, REDIAL );
    
    DISCONNECT = new Crestron.Logos.SplusObjects.DigitalInput( DISCONNECT__DigitalInput__, this );
    m_DigitalInputList.Add( DISCONNECT__DigitalInput__, DISCONNECT );
    
    AUTOANSWERTOGGLE = new Crestron.Logos.SplusObjects.DigitalInput( AUTOANSWERTOGGLE__DigitalInput__, this );
    m_DigitalInputList.Add( AUTOANSWERTOGGLE__DigitalInput__, AUTOANSWERTOGGLE );
    
    DNDTOGGLE = new Crestron.Logos.SplusObjects.DigitalInput( DNDTOGGLE__DigitalInput__, this );
    m_DigitalInputList.Add( DNDTOGGLE__DigitalInput__, DNDTOGGLE );
    
    KEYPAD = new InOutArray<DigitalInput>( 10, this );
    for( uint i = 0; i < 10; i++ )
    {
        KEYPAD[i+1] = new Crestron.Logos.SplusObjects.DigitalInput( KEYPAD__DigitalInput__ + i, KEYPAD__DigitalInput__, this );
        m_DigitalInputList.Add( KEYPAD__DigitalInput__ + i, KEYPAD[i+1] );
    }
    
    CONNECTED = new Crestron.Logos.SplusObjects.DigitalOutput( CONNECTED__DigitalOutput__, this );
    m_DigitalOutputList.Add( CONNECTED__DigitalOutput__, CONNECTED );
    
    RINGING = new Crestron.Logos.SplusObjects.DigitalOutput( RINGING__DigitalOutput__, this );
    m_DigitalOutputList.Add( RINGING__DigitalOutput__, RINGING );
    
    AUTOANSWERSTATUS = new Crestron.Logos.SplusObjects.DigitalOutput( AUTOANSWERSTATUS__DigitalOutput__, this );
    m_DigitalOutputList.Add( AUTOANSWERSTATUS__DigitalOutput__, AUTOANSWERSTATUS );
    
    DNDSTATUS = new Crestron.Logos.SplusObjects.DigitalOutput( DNDSTATUS__DigitalOutput__, this );
    m_DigitalOutputList.Add( DNDSTATUS__DigitalOutput__, DNDSTATUS );
    
    DIALSTRING = new Crestron.Logos.SplusObjects.StringInput( DIALSTRING__AnalogSerialInput__, 1000, this );
    m_StringInputList.Add( DIALSTRING__AnalogSerialInput__, DIALSTRING );
    
    DIALSTRINGOUT = new Crestron.Logos.SplusObjects.StringOutput( DIALSTRINGOUT__AnalogSerialOutput__, this );
    m_StringOutputList.Add( DIALSTRINGOUT__AnalogSerialOutput__, DIALSTRINGOUT );
    
    CURRENTLYCALLING = new Crestron.Logos.SplusObjects.StringOutput( CURRENTLYCALLING__AnalogSerialOutput__, this );
    m_StringOutputList.Add( CURRENTLYCALLING__AnalogSerialOutput__, CURRENTLYCALLING );
    
    CALLSTATUS = new Crestron.Logos.SplusObjects.StringOutput( CALLSTATUS__AnalogSerialOutput__, this );
    m_StringOutputList.Add( CALLSTATUS__AnalogSerialOutput__, CALLSTATUS );
    
    COMPONENTNAME = new StringParameter( COMPONENTNAME__Parameter__, this );
    m_ParameterList.Add( COMPONENTNAME__Parameter__, COMPONENTNAME );
    
    
    DIAL.OnDigitalPush.Add( new InputChangeHandlerWrapper( DIAL_OnPush_0, false ) );
    REDIAL.OnDigitalPush.Add( new InputChangeHandlerWrapper( REDIAL_OnPush_1, false ) );
    DISCONNECT.OnDigitalPush.Add( new InputChangeHandlerWrapper( DISCONNECT_OnPush_2, false ) );
    AUTOANSWERTOGGLE.OnDigitalPush.Add( new InputChangeHandlerWrapper( AUTOANSWERTOGGLE_OnPush_3, false ) );
    DNDTOGGLE.OnDigitalPush.Add( new InputChangeHandlerWrapper( DNDTOGGLE_OnPush_4, false ) );
    for( uint i = 0; i < 10; i++ )
        KEYPAD[i+1].OnDigitalPush.Add( new InputChangeHandlerWrapper( KEYPAD_OnPush_5, true ) );
        
    
    _SplusNVRAM.PopulateCustomAttributeList( true );
    
    NVRAM = _SplusNVRAM;
    
}

public override void LogosSimplSharpInitialize()
{
    POTS  = new QscQsys.QsysPotsControllerSimpl();
    
    
}

public UserModuleClass_QSYS_POTS_CONTROLLER ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}




const uint DIAL__DigitalInput__ = 0;
const uint REDIAL__DigitalInput__ = 1;
const uint DISCONNECT__DigitalInput__ = 2;
const uint AUTOANSWERTOGGLE__DigitalInput__ = 3;
const uint DNDTOGGLE__DigitalInput__ = 4;
const uint KEYPAD__DigitalInput__ = 5;
const uint DIALSTRING__AnalogSerialInput__ = 0;
const uint CONNECTED__DigitalOutput__ = 0;
const uint RINGING__DigitalOutput__ = 1;
const uint AUTOANSWERSTATUS__DigitalOutput__ = 2;
const uint DNDSTATUS__DigitalOutput__ = 3;
const uint DIALSTRINGOUT__AnalogSerialOutput__ = 0;
const uint CURRENTLYCALLING__AnalogSerialOutput__ = 1;
const uint CALLSTATUS__AnalogSerialOutput__ = 2;
const uint COMPONENTNAME__Parameter__ = 10;

[SplusStructAttribute(-1, true, false)]
public class SplusNVRAM : SplusStructureBase
{

    public SplusNVRAM( SplusObject __caller__ ) : base( __caller__ ) {}
    
    
}

SplusNVRAM _SplusNVRAM = null;

public class __CEvent__ : CEvent
{
    public __CEvent__() {}
    public void Close() { base.Close(); }
    public int Reset() { return base.Reset() ? 1 : 0; }
    public int Set() { return base.Set() ? 1 : 0; }
    public int Wait( int timeOutInMs ) { return base.Wait( timeOutInMs ) ? 1 : 0; }
}
public class __CMutex__ : CMutex
{
    public __CMutex__() {}
    public void Close() { base.Close(); }
    public void ReleaseMutex() { base.ReleaseMutex(); }
    public int WaitForMutex() { return base.WaitForMutex() ? 1 : 0; }
}
 public int IsNull( object obj ){ return (obj == null) ? 1 : 0; }
}


}
