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

namespace UserModule_QSYS_GENERIC_BOOLEAN_NAMED_CONTROL
{
    public class UserModuleClass_QSYS_GENERIC_BOOLEAN_NAMED_CONTROL : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        
        Crestron.Logos.SplusObjects.DigitalInput SETBOOLEANSTATEON;
        Crestron.Logos.SplusObjects.DigitalInput SETBOOLEANSTATEOFF;
        Crestron.Logos.SplusObjects.DigitalInput TOGGLEBOOLEANSTATE;
        Crestron.Logos.SplusObjects.DigitalOutput BOOLEANSTATE;
        StringParameter NAMEDCONTROLNAME;
        QscQsys.QsysNamedControlSimpl BOOLEANCONTROL;
        object SETBOOLEANSTATEON_OnPush_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 18;
                BOOLEANCONTROL . SetBoolean ( (ushort)( 1 )) ; 
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    object SETBOOLEANSTATEOFF_OnPush_1 ( Object __EventInfo__ )
    
        { 
        Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
        try
        {
            SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
            
            __context__.SourceCodeLine = 23;
            BOOLEANCONTROL . SetBoolean ( (ushort)( 0 )) ; 
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
object TOGGLEBOOLEANSTATE_OnPush_2 ( Object __EventInfo__ )

    { 
    Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
    try
    {
        SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
        
        __context__.SourceCodeLine = 28;
        if ( Functions.TestForTrue  ( ( BOOLEANSTATE  .Value)  ) ) 
            {
            __context__.SourceCodeLine = 29;
            BOOLEANCONTROL . SetBoolean ( (ushort)( 0 )) ; 
            }
        
        else 
            {
            __context__.SourceCodeLine = 31;
            BOOLEANCONTROL . SetBoolean ( (ushort)( 1 )) ; 
            }
        
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler( __SignalEventArg__ ); }
    return this;
    
}

public void NEWBOOLEANCHANGE ( ushort VALUE , SimplSharpString X ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 36;
        BOOLEANSTATE  .Value = (ushort) ( VALUE ) ; 
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public override object FunctionMain (  object __obj__ ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusFunctionMainStartCode();
        
        __context__.SourceCodeLine = 41;
        // RegisterDelegate( BOOLEANCONTROL , NEWNAMEDCONTROLCHANGE , NEWBOOLEANCHANGE ) 
        BOOLEANCONTROL .newNamedControlChange  = NEWBOOLEANCHANGE; ; 
        __context__.SourceCodeLine = 42;
        BOOLEANCONTROL . Initialize ( NAMEDCONTROLNAME  .ToString(), (short)( 0 ), (short)( 0 )) ; 
        
        
    }
    catch(Exception e) { ObjectCatchHandler(e); }
    finally { ObjectFinallyHandler(); }
    return __obj__;
    }
    

public override void LogosSplusInitialize()
{
    _SplusNVRAM = new SplusNVRAM( this );
    
    SETBOOLEANSTATEON = new Crestron.Logos.SplusObjects.DigitalInput( SETBOOLEANSTATEON__DigitalInput__, this );
    m_DigitalInputList.Add( SETBOOLEANSTATEON__DigitalInput__, SETBOOLEANSTATEON );
    
    SETBOOLEANSTATEOFF = new Crestron.Logos.SplusObjects.DigitalInput( SETBOOLEANSTATEOFF__DigitalInput__, this );
    m_DigitalInputList.Add( SETBOOLEANSTATEOFF__DigitalInput__, SETBOOLEANSTATEOFF );
    
    TOGGLEBOOLEANSTATE = new Crestron.Logos.SplusObjects.DigitalInput( TOGGLEBOOLEANSTATE__DigitalInput__, this );
    m_DigitalInputList.Add( TOGGLEBOOLEANSTATE__DigitalInput__, TOGGLEBOOLEANSTATE );
    
    BOOLEANSTATE = new Crestron.Logos.SplusObjects.DigitalOutput( BOOLEANSTATE__DigitalOutput__, this );
    m_DigitalOutputList.Add( BOOLEANSTATE__DigitalOutput__, BOOLEANSTATE );
    
    NAMEDCONTROLNAME = new StringParameter( NAMEDCONTROLNAME__Parameter__, this );
    m_ParameterList.Add( NAMEDCONTROLNAME__Parameter__, NAMEDCONTROLNAME );
    
    
    SETBOOLEANSTATEON.OnDigitalPush.Add( new InputChangeHandlerWrapper( SETBOOLEANSTATEON_OnPush_0, false ) );
    SETBOOLEANSTATEOFF.OnDigitalPush.Add( new InputChangeHandlerWrapper( SETBOOLEANSTATEOFF_OnPush_1, false ) );
    TOGGLEBOOLEANSTATE.OnDigitalPush.Add( new InputChangeHandlerWrapper( TOGGLEBOOLEANSTATE_OnPush_2, false ) );
    
    _SplusNVRAM.PopulateCustomAttributeList( true );
    
    NVRAM = _SplusNVRAM;
    
}

public override void LogosSimplSharpInitialize()
{
    BOOLEANCONTROL  = new QscQsys.QsysNamedControlSimpl();
    
    
}

public UserModuleClass_QSYS_GENERIC_BOOLEAN_NAMED_CONTROL ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}




const uint SETBOOLEANSTATEON__DigitalInput__ = 0;
const uint SETBOOLEANSTATEOFF__DigitalInput__ = 1;
const uint TOGGLEBOOLEANSTATE__DigitalInput__ = 2;
const uint BOOLEANSTATE__DigitalOutput__ = 0;
const uint NAMEDCONTROLNAME__Parameter__ = 10;

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
