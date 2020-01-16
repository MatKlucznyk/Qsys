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
using Crestron.SimplSharp.SimplSharpExtensions;
using TCP_Client;

namespace UserModule_QSYS_POTS_CONTROLLER
{
    public class UserModuleClass_QSYS_POTS_CONTROLLER : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        
        
        Crestron.Logos.SplusObjects.DigitalInput DIAL;
        Crestron.Logos.SplusObjects.DigitalInput REDIAL;
        Crestron.Logos.SplusObjects.DigitalInput CONNECT;
        Crestron.Logos.SplusObjects.DigitalInput DISCONNECT;
        Crestron.Logos.SplusObjects.DigitalInput AUTOANSWERTOGGLE;
        Crestron.Logos.SplusObjects.DigitalInput DNDTOGGLE;
        Crestron.Logos.SplusObjects.DigitalInput KEYPADDELETE;
        Crestron.Logos.SplusObjects.DigitalInput KEYPADCLEAR;
        Crestron.Logos.SplusObjects.DigitalInput KEYPADSTAR;
        Crestron.Logos.SplusObjects.DigitalInput KEYPADPOUND;
        InOutArray<Crestron.Logos.SplusObjects.DigitalInput> KEYPAD;
        InOutArray<Crestron.Logos.SplusObjects.DigitalInput> SELECTRECENTCALL;
        Crestron.Logos.SplusObjects.AnalogInput SELECTRECENTCALLINDEX;
        Crestron.Logos.SplusObjects.DigitalOutput CONNECTED;
        Crestron.Logos.SplusObjects.DigitalOutput RINGING;
        Crestron.Logos.SplusObjects.DigitalOutput DIALING;
        Crestron.Logos.SplusObjects.DigitalOutput AUTOANSWERSTATUS;
        Crestron.Logos.SplusObjects.DigitalOutput DNDSTATUS;
        Crestron.Logos.SplusObjects.StringOutput CURRENTLYCALLING;
        Crestron.Logos.SplusObjects.StringOutput CALLSTATUS;
        Crestron.Logos.SplusObjects.StringOutput DIALSTRINGOUT;
        Crestron.Logos.SplusObjects.StringOutput RECENTCALLXSIG;
        InOutArray<Crestron.Logos.SplusObjects.StringOutput> RECENTCALLS;
        QscQsys.QsysPotsControllerSimpl POTS;
        StringParameter COMPONENTNAME;
        object DIAL_OnPush_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 21;
                POTS . DialWithoutString ( ) ; 
                
                
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
            
            __context__.SourceCodeLine = 26;
            POTS . Redial ( ) ; 
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
object CONNECT_OnPush_2 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 31;
        POTS . Connect ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object DISCONNECT_OnPush_3 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 36;
        POTS . Disconnect ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object AUTOANSWERTOGGLE_OnPush_4 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 41;
        POTS . AutoAnswerToggle ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object DNDTOGGLE_OnPush_5 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 46;
        POTS . DndToggle ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object KEYPAD_OnPush_6 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        ushort X = 0;
        
        
        __context__.SourceCodeLine = 53;
        X = (ushort) ( Functions.GetLastModifiedArrayIndex( __SignalEventArg__ ) ) ; 
        __context__.SourceCodeLine = 55;
        POTS . NumPad ( Functions.ItoA( (int)( (X - 1) ) ) .ToString()) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object KEYPADDELETE_OnPush_7 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 60;
        POTS . NumPadDelete ( ) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object KEYPADSTAR_OnPush_8 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 65;
        POTS . NumPad ( "*") ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object KEYPADPOUND_OnPush_9 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 70;
        POTS . NumPad ( "#") ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SELECTRECENTCALL_OnPush_10 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        ushort X = 0;
        
        
        __context__.SourceCodeLine = 77;
        X = (ushort) ( Functions.GetLastModifiedArrayIndex( __SignalEventArg__ ) ) ; 
        __context__.SourceCodeLine = 79;
        POTS . SelectRecentCall ( (ushort)( X )) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

object SELECTRECENTCALLINDEX_OnChange_11 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        ushort X = 0;
        
        
        __context__.SourceCodeLine = 86;
        while ( Functions.TestForTrue  ( ( Functions.BoolToInt (X != SELECTRECENTCALLINDEX  .UshortValue))  ) ) 
            { 
            __context__.SourceCodeLine = 88;
            X = (ushort) ( SELECTRECENTCALLINDEX  .UshortValue ) ; 
            __context__.SourceCodeLine = 90;
            POTS . SelectRecentCall ( (ushort)( X )) ; 
            __context__.SourceCodeLine = 86;
            } 
        
        
        
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
        
        __context__.SourceCodeLine = 96;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 97;
            CONNECTED  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 99;
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
        
        __context__.SourceCodeLine = 104;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 105;
            RINGING  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 107;
            RINGING  .Value = (ushort) ( 0 ) ; 
            }
        
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWDIALINGEVENT ( ushort VALUE ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 112;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 113;
            DIALING  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 115;
            DIALING  .Value = (ushort) ( 0 ) ; 
            }
        
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWAUTOANSWEREVENT ( ushort VALUE ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 120;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 121;
            AUTOANSWERSTATUS  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 123;
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
        
        __context__.SourceCodeLine = 128;
        if ( Functions.TestForTrue  ( ( VALUE)  ) ) 
            {
            __context__.SourceCodeLine = 129;
            DNDSTATUS  .Value = (ushort) ( 1 ) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 131;
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
        
        __context__.SourceCodeLine = 136;
        DIALSTRINGOUT  .UpdateValue ( NEWDIALSTRING  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWCURRENTLYCALLINGEVENT ( SimplSharpString NEWCURRENTLYCALLING ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 141;
        CURRENTLYCALLING  .UpdateValue ( NEWCURRENTLYCALLING  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWCURRENTCALLSTATUSCHANGE ( SimplSharpString STATUS ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 146;
        CALLSTATUS  .UpdateValue ( STATUS  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWRECENTCALLSEVENT ( SimplSharpString CALL1 , SimplSharpString CALL2 , SimplSharpString CALL3 , SimplSharpString CALL4 , SimplSharpString CALL5 ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 151;
        RECENTCALLS [ 1]  .UpdateValue ( CALL1  .ToString()  ) ; 
        __context__.SourceCodeLine = 152;
        RECENTCALLS [ 2]  .UpdateValue ( CALL2  .ToString()  ) ; 
        __context__.SourceCodeLine = 153;
        RECENTCALLS [ 3]  .UpdateValue ( CALL3  .ToString()  ) ; 
        __context__.SourceCodeLine = 154;
        RECENTCALLS [ 4]  .UpdateValue ( CALL4  .ToString()  ) ; 
        __context__.SourceCodeLine = 155;
        RECENTCALLS [ 5]  .UpdateValue ( CALL5  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public void NEWRECENTCALLLISTEVENT ( SimplSharpString XSIG ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 160;
        RECENTCALLXSIG  .UpdateValue ( XSIG  .ToString()  ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public override object FunctionMain (  object __obj__ ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusFunctionMainStartCode();
        
        __context__.SourceCodeLine = 165;
        // RegisterDelegate( POTS , ONOFFHOOKEVENT , NEWOFFHOOKEVENT ) 
        POTS .onOffHookEvent  = NEWOFFHOOKEVENT; ; 
        __context__.SourceCodeLine = 166;
        // RegisterDelegate( POTS , ONRINGINGEVENT , NEWRINGINGEVENT ) 
        POTS .onRingingEvent  = NEWRINGINGEVENT; ; 
        __context__.SourceCodeLine = 167;
        // RegisterDelegate( POTS , ONDIALINGEVENT , NEWDIALINGEVENT ) 
        POTS .onDialingEvent  = NEWDIALINGEVENT; ; 
        __context__.SourceCodeLine = 168;
        // RegisterDelegate( POTS , ONAUTOANSWEREVENT , NEWAUTOANSWEREVENT ) 
        POTS .onAutoAnswerEvent  = NEWAUTOANSWEREVENT; ; 
        __context__.SourceCodeLine = 169;
        // RegisterDelegate( POTS , ONDNDEVENT , NEWDNDEVENT ) 
        POTS .onDndEvent  = NEWDNDEVENT; ; 
        __context__.SourceCodeLine = 170;
        // RegisterDelegate( POTS , ONDIALSTRINGEVENT , NEWDIALSTRINGEVENT ) 
        POTS .onDialStringEvent  = NEWDIALSTRINGEVENT; ; 
        __context__.SourceCodeLine = 171;
        // RegisterDelegate( POTS , ONCURRENTLYCALLINGEVENT , NEWCURRENTLYCALLINGEVENT ) 
        POTS .onCurrentlyCallingEvent  = NEWCURRENTLYCALLINGEVENT; ; 
        __context__.SourceCodeLine = 172;
        // RegisterDelegate( POTS , ONCURRENTCALLSTATUSCHANGE , NEWCURRENTCALLSTATUSCHANGE ) 
        POTS .onCurrentCallStatusChange  = NEWCURRENTCALLSTATUSCHANGE; ; 
        __context__.SourceCodeLine = 173;
        // RegisterDelegate( POTS , ONRECENTCALLSEVENT , NEWRECENTCALLSEVENT ) 
        POTS .onRecentCallsEvent  = NEWRECENTCALLSEVENT; ; 
        __context__.SourceCodeLine = 174;
        // RegisterDelegate( POTS , ONRECENTCALLLISTEVENT , NEWRECENTCALLLISTEVENT ) 
        POTS .onRecentCallListEvent  = NEWRECENTCALLLISTEVENT; ; 
        __context__.SourceCodeLine = 175;
        POTS . Initialize ( COMPONENTNAME  .ToString()) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    return __obj__;
    }
    

public override void LogosSplusInitialize()
{
    SocketInfo __socketinfo__ = new SocketInfo( 1, this );
    InitialParametersClass.ResolveHostName = __socketinfo__.ResolveHostName;
    _SplusNVRAM = new SplusNVRAM( this );
    
    DIAL = new Crestron.Logos.SplusObjects.DigitalInput( DIAL__DigitalInput__, this );
    m_DigitalInputList.Add( DIAL__DigitalInput__, DIAL );
    
    REDIAL = new Crestron.Logos.SplusObjects.DigitalInput( REDIAL__DigitalInput__, this );
    m_DigitalInputList.Add( REDIAL__DigitalInput__, REDIAL );
    
    CONNECT = new Crestron.Logos.SplusObjects.DigitalInput( CONNECT__DigitalInput__, this );
    m_DigitalInputList.Add( CONNECT__DigitalInput__, CONNECT );
    
    DISCONNECT = new Crestron.Logos.SplusObjects.DigitalInput( DISCONNECT__DigitalInput__, this );
    m_DigitalInputList.Add( DISCONNECT__DigitalInput__, DISCONNECT );
    
    AUTOANSWERTOGGLE = new Crestron.Logos.SplusObjects.DigitalInput( AUTOANSWERTOGGLE__DigitalInput__, this );
    m_DigitalInputList.Add( AUTOANSWERTOGGLE__DigitalInput__, AUTOANSWERTOGGLE );
    
    DNDTOGGLE = new Crestron.Logos.SplusObjects.DigitalInput( DNDTOGGLE__DigitalInput__, this );
    m_DigitalInputList.Add( DNDTOGGLE__DigitalInput__, DNDTOGGLE );
    
    KEYPADDELETE = new Crestron.Logos.SplusObjects.DigitalInput( KEYPADDELETE__DigitalInput__, this );
    m_DigitalInputList.Add( KEYPADDELETE__DigitalInput__, KEYPADDELETE );
    
    KEYPADCLEAR = new Crestron.Logos.SplusObjects.DigitalInput( KEYPADCLEAR__DigitalInput__, this );
    m_DigitalInputList.Add( KEYPADCLEAR__DigitalInput__, KEYPADCLEAR );
    
    KEYPADSTAR = new Crestron.Logos.SplusObjects.DigitalInput( KEYPADSTAR__DigitalInput__, this );
    m_DigitalInputList.Add( KEYPADSTAR__DigitalInput__, KEYPADSTAR );
    
    KEYPADPOUND = new Crestron.Logos.SplusObjects.DigitalInput( KEYPADPOUND__DigitalInput__, this );
    m_DigitalInputList.Add( KEYPADPOUND__DigitalInput__, KEYPADPOUND );
    
    KEYPAD = new InOutArray<DigitalInput>( 10, this );
    for( uint i = 0; i < 10; i++ )
    {
        KEYPAD[i+1] = new Crestron.Logos.SplusObjects.DigitalInput( KEYPAD__DigitalInput__ + i, KEYPAD__DigitalInput__, this );
        m_DigitalInputList.Add( KEYPAD__DigitalInput__ + i, KEYPAD[i+1] );
    }
    
    SELECTRECENTCALL = new InOutArray<DigitalInput>( 5, this );
    for( uint i = 0; i < 5; i++ )
    {
        SELECTRECENTCALL[i+1] = new Crestron.Logos.SplusObjects.DigitalInput( SELECTRECENTCALL__DigitalInput__ + i, SELECTRECENTCALL__DigitalInput__, this );
        m_DigitalInputList.Add( SELECTRECENTCALL__DigitalInput__ + i, SELECTRECENTCALL[i+1] );
    }
    
    CONNECTED = new Crestron.Logos.SplusObjects.DigitalOutput( CONNECTED__DigitalOutput__, this );
    m_DigitalOutputList.Add( CONNECTED__DigitalOutput__, CONNECTED );
    
    RINGING = new Crestron.Logos.SplusObjects.DigitalOutput( RINGING__DigitalOutput__, this );
    m_DigitalOutputList.Add( RINGING__DigitalOutput__, RINGING );
    
    DIALING = new Crestron.Logos.SplusObjects.DigitalOutput( DIALING__DigitalOutput__, this );
    m_DigitalOutputList.Add( DIALING__DigitalOutput__, DIALING );
    
    AUTOANSWERSTATUS = new Crestron.Logos.SplusObjects.DigitalOutput( AUTOANSWERSTATUS__DigitalOutput__, this );
    m_DigitalOutputList.Add( AUTOANSWERSTATUS__DigitalOutput__, AUTOANSWERSTATUS );
    
    DNDSTATUS = new Crestron.Logos.SplusObjects.DigitalOutput( DNDSTATUS__DigitalOutput__, this );
    m_DigitalOutputList.Add( DNDSTATUS__DigitalOutput__, DNDSTATUS );
    
    SELECTRECENTCALLINDEX = new Crestron.Logos.SplusObjects.AnalogInput( SELECTRECENTCALLINDEX__AnalogSerialInput__, this );
    m_AnalogInputList.Add( SELECTRECENTCALLINDEX__AnalogSerialInput__, SELECTRECENTCALLINDEX );
    
    CURRENTLYCALLING = new Crestron.Logos.SplusObjects.StringOutput( CURRENTLYCALLING__AnalogSerialOutput__, this );
    m_StringOutputList.Add( CURRENTLYCALLING__AnalogSerialOutput__, CURRENTLYCALLING );
    
    CALLSTATUS = new Crestron.Logos.SplusObjects.StringOutput( CALLSTATUS__AnalogSerialOutput__, this );
    m_StringOutputList.Add( CALLSTATUS__AnalogSerialOutput__, CALLSTATUS );
    
    DIALSTRINGOUT = new Crestron.Logos.SplusObjects.StringOutput( DIALSTRINGOUT__AnalogSerialOutput__, this );
    m_StringOutputList.Add( DIALSTRINGOUT__AnalogSerialOutput__, DIALSTRINGOUT );
    
    RECENTCALLXSIG = new Crestron.Logos.SplusObjects.StringOutput( RECENTCALLXSIG__AnalogSerialOutput__, this );
    m_StringOutputList.Add( RECENTCALLXSIG__AnalogSerialOutput__, RECENTCALLXSIG );
    
    RECENTCALLS = new InOutArray<StringOutput>( 5, this );
    for( uint i = 0; i < 5; i++ )
    {
        RECENTCALLS[i+1] = new Crestron.Logos.SplusObjects.StringOutput( RECENTCALLS__AnalogSerialOutput__ + i, this );
        m_StringOutputList.Add( RECENTCALLS__AnalogSerialOutput__ + i, RECENTCALLS[i+1] );
    }
    
    COMPONENTNAME = new StringParameter( COMPONENTNAME__Parameter__, this );
    m_ParameterList.Add( COMPONENTNAME__Parameter__, COMPONENTNAME );
    
    
    DIAL.OnDigitalPush.Add( new InputChangeHandlerWrapper( DIAL_OnPush_0, false ) );
    REDIAL.OnDigitalPush.Add( new InputChangeHandlerWrapper( REDIAL_OnPush_1, false ) );
    CONNECT.OnDigitalPush.Add( new InputChangeHandlerWrapper( CONNECT_OnPush_2, false ) );
    DISCONNECT.OnDigitalPush.Add( new InputChangeHandlerWrapper( DISCONNECT_OnPush_3, false ) );
    AUTOANSWERTOGGLE.OnDigitalPush.Add( new InputChangeHandlerWrapper( AUTOANSWERTOGGLE_OnPush_4, false ) );
    DNDTOGGLE.OnDigitalPush.Add( new InputChangeHandlerWrapper( DNDTOGGLE_OnPush_5, false ) );
    for( uint i = 0; i < 10; i++ )
        KEYPAD[i+1].OnDigitalPush.Add( new InputChangeHandlerWrapper( KEYPAD_OnPush_6, true ) );
        
    KEYPADDELETE.OnDigitalPush.Add( new InputChangeHandlerWrapper( KEYPADDELETE_OnPush_7, false ) );
    KEYPADSTAR.OnDigitalPush.Add( new InputChangeHandlerWrapper( KEYPADSTAR_OnPush_8, false ) );
    KEYPADPOUND.OnDigitalPush.Add( new InputChangeHandlerWrapper( KEYPADPOUND_OnPush_9, false ) );
    for( uint i = 0; i < 5; i++ )
        SELECTRECENTCALL[i+1].OnDigitalPush.Add( new InputChangeHandlerWrapper( SELECTRECENTCALL_OnPush_10, true ) );
        
    SELECTRECENTCALLINDEX.OnAnalogChange.Add( new InputChangeHandlerWrapper( SELECTRECENTCALLINDEX_OnChange_11, true ) );
    
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
const uint CONNECT__DigitalInput__ = 2;
const uint DISCONNECT__DigitalInput__ = 3;
const uint AUTOANSWERTOGGLE__DigitalInput__ = 4;
const uint DNDTOGGLE__DigitalInput__ = 5;
const uint KEYPADDELETE__DigitalInput__ = 6;
const uint KEYPADCLEAR__DigitalInput__ = 7;
const uint KEYPADSTAR__DigitalInput__ = 8;
const uint KEYPADPOUND__DigitalInput__ = 9;
const uint KEYPAD__DigitalInput__ = 10;
const uint SELECTRECENTCALL__DigitalInput__ = 20;
const uint SELECTRECENTCALLINDEX__AnalogSerialInput__ = 0;
const uint CONNECTED__DigitalOutput__ = 0;
const uint RINGING__DigitalOutput__ = 1;
const uint DIALING__DigitalOutput__ = 2;
const uint AUTOANSWERSTATUS__DigitalOutput__ = 3;
const uint DNDSTATUS__DigitalOutput__ = 4;
const uint CURRENTLYCALLING__AnalogSerialOutput__ = 0;
const uint CALLSTATUS__AnalogSerialOutput__ = 1;
const uint DIALSTRINGOUT__AnalogSerialOutput__ = 2;
const uint RECENTCALLXSIG__AnalogSerialOutput__ = 3;
const uint RECENTCALLS__AnalogSerialOutput__ = 4;
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
