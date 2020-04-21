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

namespace UserModule_QSYS_PROCESSOR
{
    public class UserModuleClass_QSYS_PROCESSOR : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        Crestron.Logos.SplusObjects.DigitalInput INITIALIZE;
        Crestron.Logos.SplusObjects.DigitalOutput ISINITIALIZED;
        Crestron.Logos.SplusObjects.DigitalOutput ISCONNECTED;
        Crestron.Logos.SplusObjects.DigitalOutput ISREDUNDANT;
        Crestron.Logos.SplusObjects.DigitalOutput ISEMULATOR;
        Crestron.Logos.SplusObjects.StringOutput DESIGNNAME;
        StringParameter ID;
        StringParameter HOST;
        UShortParameter PORT;
        UShortParameter DEBUGMODE;
        ushort WAITTILLSTART = 0;
        QscQsys.QsysProcessorSimplInterface PROCESSOR;
        object INITIALIZE_OnPush_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                
                __context__.SourceCodeLine = 21;
                while ( Functions.TestForTrue  ( ( Functions.Not( WAITTILLSTART ))  ) ) 
                    { 
                    __context__.SourceCodeLine = 21;
                    } 
                
                __context__.SourceCodeLine = 23;
                PROCESSOR . Debug ( (ushort)( DEBUGMODE  .Value )) ; 
                __context__.SourceCodeLine = 24;
                PROCESSOR . Register ( ID  .ToString()) ; 
                __context__.SourceCodeLine = 25;
                 QsysProcessor.Initialize(  HOST  .ToString() , (ushort)( PORT  .Value ) )  ;  
 
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    public void NEWISREGISTERED ( ushort VALUE ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
            
            __context__.SourceCodeLine = 30;
            if ( Functions.TestForTrue  ( ( Functions.BoolToInt (VALUE == 1))  ) ) 
                {
                __context__.SourceCodeLine = 31;
                ISINITIALIZED  .Value = (ushort) ( 1 ) ; 
                }
            
            else 
                {
                __context__.SourceCodeLine = 33;
                ISINITIALIZED  .Value = (ushort) ( 0 ) ; 
                }
            
            
            
        }
        finally { ObjectFinallyHandler(); }
        }
        
    public void NEWISCONNECTED ( ushort VALUE ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
            
            __context__.SourceCodeLine = 38;
            if ( Functions.TestForTrue  ( ( Functions.BoolToInt (VALUE == 1))  ) ) 
                {
                __context__.SourceCodeLine = 39;
                ISCONNECTED  .Value = (ushort) ( 1 ) ; 
                }
            
            else 
                {
                __context__.SourceCodeLine = 41;
                ISCONNECTED  .Value = (ushort) ( 0 ) ; 
                }
            
            
            
        }
        finally { ObjectFinallyHandler(); }
        }
        
    public void NEWCORESTATUS ( SimplSharpString DNAME , ushort REDUNDANT , ushort EMULATOR ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
            
            __context__.SourceCodeLine = 46;
            DESIGNNAME  .UpdateValue ( DNAME  .ToString()  ) ; 
            __context__.SourceCodeLine = 47;
            ISREDUNDANT  .Value = (ushort) ( REDUNDANT ) ; 
            __context__.SourceCodeLine = 48;
            ISEMULATOR  .Value = (ushort) ( EMULATOR ) ; 
            
            
        }
        finally { ObjectFinallyHandler(); }
        }
        
    public override object FunctionMain (  object __obj__ ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusFunctionMainStartCode();
            
            __context__.SourceCodeLine = 53;
            // RegisterDelegate( PROCESSOR , ONISREGISTERED , NEWISREGISTERED ) 
            PROCESSOR .onIsRegistered  = NEWISREGISTERED; ; 
            __context__.SourceCodeLine = 54;
            // RegisterDelegate( PROCESSOR , ONISCONNECTED , NEWISCONNECTED ) 
            PROCESSOR .onIsConnected  = NEWISCONNECTED; ; 
            __context__.SourceCodeLine = 55;
            // RegisterDelegate( PROCESSOR , ONNEWCORESTATUS , NEWCORESTATUS ) 
            PROCESSOR .onNewCoreStatus  = NEWCORESTATUS; ; 
            __context__.SourceCodeLine = 57;
            WaitForInitializationComplete ( ) ; 
            __context__.SourceCodeLine = 59;
            WAITTILLSTART = (ushort) ( 1 ) ; 
            
            
        }
        catch(Exception e) { ObjectCatchHandler(e); }
        finally { ObjectFinallyHandler(); }
        return __obj__;
        }
        
    
    public override void LogosSplusInitialize()
    {
        _SplusNVRAM = new SplusNVRAM( this );
        
        INITIALIZE = new Crestron.Logos.SplusObjects.DigitalInput( INITIALIZE__DigitalInput__, this );
        m_DigitalInputList.Add( INITIALIZE__DigitalInput__, INITIALIZE );
        
        ISINITIALIZED = new Crestron.Logos.SplusObjects.DigitalOutput( ISINITIALIZED__DigitalOutput__, this );
        m_DigitalOutputList.Add( ISINITIALIZED__DigitalOutput__, ISINITIALIZED );
        
        ISCONNECTED = new Crestron.Logos.SplusObjects.DigitalOutput( ISCONNECTED__DigitalOutput__, this );
        m_DigitalOutputList.Add( ISCONNECTED__DigitalOutput__, ISCONNECTED );
        
        ISREDUNDANT = new Crestron.Logos.SplusObjects.DigitalOutput( ISREDUNDANT__DigitalOutput__, this );
        m_DigitalOutputList.Add( ISREDUNDANT__DigitalOutput__, ISREDUNDANT );
        
        ISEMULATOR = new Crestron.Logos.SplusObjects.DigitalOutput( ISEMULATOR__DigitalOutput__, this );
        m_DigitalOutputList.Add( ISEMULATOR__DigitalOutput__, ISEMULATOR );
        
        DESIGNNAME = new Crestron.Logos.SplusObjects.StringOutput( DESIGNNAME__AnalogSerialOutput__, this );
        m_StringOutputList.Add( DESIGNNAME__AnalogSerialOutput__, DESIGNNAME );
        
        PORT = new UShortParameter( PORT__Parameter__, this );
        m_ParameterList.Add( PORT__Parameter__, PORT );
        
        DEBUGMODE = new UShortParameter( DEBUGMODE__Parameter__, this );
        m_ParameterList.Add( DEBUGMODE__Parameter__, DEBUGMODE );
        
        ID = new StringParameter( ID__Parameter__, this );
        m_ParameterList.Add( ID__Parameter__, ID );
        
        HOST = new StringParameter( HOST__Parameter__, this );
        m_ParameterList.Add( HOST__Parameter__, HOST );
        
        
        INITIALIZE.OnDigitalPush.Add( new InputChangeHandlerWrapper( INITIALIZE_OnPush_0, false ) );
        
        _SplusNVRAM.PopulateCustomAttributeList( true );
        
        NVRAM = _SplusNVRAM;
        
    }
    
    public override void LogosSimplSharpInitialize()
    {
        PROCESSOR  = new QscQsys.QsysProcessorSimplInterface();
        
        
    }
    
    public UserModuleClass_QSYS_PROCESSOR ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}
    
    
    
    
    const uint INITIALIZE__DigitalInput__ = 0;
    const uint ISINITIALIZED__DigitalOutput__ = 0;
    const uint ISCONNECTED__DigitalOutput__ = 1;
    const uint ISREDUNDANT__DigitalOutput__ = 2;
    const uint ISEMULATOR__DigitalOutput__ = 3;
    const uint DESIGNNAME__AnalogSerialOutput__ = 0;
    const uint ID__Parameter__ = 10;
    const uint HOST__Parameter__ = 11;
    const uint PORT__Parameter__ = 12;
    const uint DEBUGMODE__Parameter__ = 13;
    
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
