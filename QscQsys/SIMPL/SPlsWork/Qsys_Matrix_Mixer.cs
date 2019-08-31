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

namespace UserModule_QSYS_MATRIX_MIXER
{
    public class UserModuleClass_QSYS_MATRIX_MIXER : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        
        Crestron.Logos.SplusObjects.DigitalInput MUTEON;
        Crestron.Logos.SplusObjects.DigitalInput MUTEOFF;
        Crestron.Logos.SplusObjects.DigitalOutput MUTEISON;
        Crestron.Logos.SplusObjects.DigitalOutput MUTEISOFF;
        StringParameter COMPONENTNAME;
        UShortParameter INPUT;
        UShortParameter OUTPUT;
        QscQsys.QsysMatrixMixerSimpl MIXER;
        object MUTEON_OnPush_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 16;
                MIXER . SetCrossPoint ( (ushort)( 1 )) ; 
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    object MUTEOFF_OnPush_1 ( Object __EventInfo__ )
    
        { 
        Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
        try
        {
            SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
            
            __context__.SourceCodeLine = 21;
            MIXER . SetCrossPoint ( (ushort)( 0 )) ; 
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler( __SignalEventArg__ ); }
        return this;
        
    }
    
public void ONCROSSPOINTVALUECHANGE ( ushort VALUE ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
        
        __context__.SourceCodeLine = 26;
        if ( Functions.TestForTrue  ( ( Functions.BoolToInt (VALUE == 1))  ) ) 
            { 
            __context__.SourceCodeLine = 28;
            MUTEISON  .Value = (ushort) ( 1 ) ; 
            __context__.SourceCodeLine = 29;
            MUTEISOFF  .Value = (ushort) ( 0 ) ; 
            } 
        
        else 
            { 
            __context__.SourceCodeLine = 33;
            MUTEISON  .Value = (ushort) ( 0 ) ; 
            __context__.SourceCodeLine = 34;
            MUTEISOFF  .Value = (ushort) ( 1 ) ; 
            } 
        
        
        
    }
    finally { ObjectFinallyHandler(); }
    }
    
public override object FunctionMain (  object __obj__ ) 
    { 
    try
    {
        SplusExecutionContext __context__ = SplusFunctionMainStartCode();
        
        __context__.SourceCodeLine = 40;
        // RegisterDelegate( MIXER , NEWCROSSPOINTVALUECHANGE , ONCROSSPOINTVALUECHANGE ) 
        MIXER .newCrossPointValueChange  = ONCROSSPOINTVALUECHANGE; ; 
        __context__.SourceCodeLine = 41;
        MIXER . Initialize ( COMPONENTNAME  .ToString(), (ushort)( INPUT  .Value ), (ushort)( OUTPUT  .Value )) ; 
        
        
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
    
    MUTEON = new Crestron.Logos.SplusObjects.DigitalInput( MUTEON__DigitalInput__, this );
    m_DigitalInputList.Add( MUTEON__DigitalInput__, MUTEON );
    
    MUTEOFF = new Crestron.Logos.SplusObjects.DigitalInput( MUTEOFF__DigitalInput__, this );
    m_DigitalInputList.Add( MUTEOFF__DigitalInput__, MUTEOFF );
    
    MUTEISON = new Crestron.Logos.SplusObjects.DigitalOutput( MUTEISON__DigitalOutput__, this );
    m_DigitalOutputList.Add( MUTEISON__DigitalOutput__, MUTEISON );
    
    MUTEISOFF = new Crestron.Logos.SplusObjects.DigitalOutput( MUTEISOFF__DigitalOutput__, this );
    m_DigitalOutputList.Add( MUTEISOFF__DigitalOutput__, MUTEISOFF );
    
    INPUT = new UShortParameter( INPUT__Parameter__, this );
    m_ParameterList.Add( INPUT__Parameter__, INPUT );
    
    OUTPUT = new UShortParameter( OUTPUT__Parameter__, this );
    m_ParameterList.Add( OUTPUT__Parameter__, OUTPUT );
    
    COMPONENTNAME = new StringParameter( COMPONENTNAME__Parameter__, this );
    m_ParameterList.Add( COMPONENTNAME__Parameter__, COMPONENTNAME );
    
    
    MUTEON.OnDigitalPush.Add( new InputChangeHandlerWrapper( MUTEON_OnPush_0, false ) );
    MUTEOFF.OnDigitalPush.Add( new InputChangeHandlerWrapper( MUTEOFF_OnPush_1, false ) );
    
    _SplusNVRAM.PopulateCustomAttributeList( true );
    
    NVRAM = _SplusNVRAM;
    
}

public override void LogosSimplSharpInitialize()
{
    MIXER  = new QscQsys.QsysMatrixMixerSimpl();
    
    
}

public UserModuleClass_QSYS_MATRIX_MIXER ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}




const uint MUTEON__DigitalInput__ = 0;
const uint MUTEOFF__DigitalInput__ = 1;
const uint MUTEISON__DigitalOutput__ = 0;
const uint MUTEISOFF__DigitalOutput__ = 1;
const uint COMPONENTNAME__Parameter__ = 10;
const uint INPUT__Parameter__ = 11;
const uint OUTPUT__Parameter__ = 12;

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
