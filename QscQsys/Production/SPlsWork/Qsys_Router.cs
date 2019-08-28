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

namespace UserModule_QSYS_ROUTER
{
    public class UserModuleClass_QSYS_ROUTER : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        Crestron.Logos.SplusObjects.AnalogInput INPUT;
        Crestron.Logos.SplusObjects.AnalogOutput INPUTVALUE;
        StringParameter COMPONENTNAME;
        UShortParameter OUTPUTNUMBER;
        QscQsys.QsysProcessorSimplInterface PROCESSOR;
        QscQsys.QsysRouterSimpl ROUTER;
        object INPUT_OnChange_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                ushort X = 0;
                
                
                __context__.SourceCodeLine = 19;
                while ( Functions.TestForTrue  ( ( Functions.BoolToInt (X != INPUT  .UshortValue))  ) ) 
                    { 
                    __context__.SourceCodeLine = 21;
                    X = (ushort) ( INPUT  .UshortValue ) ; 
                    __context__.SourceCodeLine = 22;
                    ROUTER . SelectInput ( (ushort)( X )) ; 
                    __context__.SourceCodeLine = 19;
                    } 
                
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    public void ONINITIALIZATIONCOMPLETE ( ushort VALUE ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
            
            
            
        }
        finally { ObjectFinallyHandler(); }
        }
        
    public void ONROUTERINPUTCHANGE ( ushort IN ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
            
            __context__.SourceCodeLine = 32;
            INPUTVALUE  .Value = (ushort) ( IN ) ; 
            
            
        }
        finally { ObjectFinallyHandler(); }
        }
        
    public override object FunctionMain (  object __obj__ ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusFunctionMainStartCode();
            
            __context__.SourceCodeLine = 37;
            // RegisterDelegate( PROCESSOR , ONISREGISTERED , ONINITIALIZATIONCOMPLETE ) 
            PROCESSOR .onIsRegistered  = ONINITIALIZATIONCOMPLETE; ; 
            __context__.SourceCodeLine = 38;
            // RegisterDelegate( ROUTER , NEWROUTERINPUTCHANGE , ONROUTERINPUTCHANGE ) 
            ROUTER .newRouterInputChange  = ONROUTERINPUTCHANGE; ; 
            __context__.SourceCodeLine = 39;
            ROUTER . Initialize ( COMPONENTNAME  .ToString(), (ushort)( OUTPUTNUMBER  .Value )) ; 
            
            
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
        
        INPUT = new Crestron.Logos.SplusObjects.AnalogInput( INPUT__AnalogSerialInput__, this );
        m_AnalogInputList.Add( INPUT__AnalogSerialInput__, INPUT );
        
        INPUTVALUE = new Crestron.Logos.SplusObjects.AnalogOutput( INPUTVALUE__AnalogSerialOutput__, this );
        m_AnalogOutputList.Add( INPUTVALUE__AnalogSerialOutput__, INPUTVALUE );
        
        OUTPUTNUMBER = new UShortParameter( OUTPUTNUMBER__Parameter__, this );
        m_ParameterList.Add( OUTPUTNUMBER__Parameter__, OUTPUTNUMBER );
        
        COMPONENTNAME = new StringParameter( COMPONENTNAME__Parameter__, this );
        m_ParameterList.Add( COMPONENTNAME__Parameter__, COMPONENTNAME );
        
        
        INPUT.OnAnalogChange.Add( new InputChangeHandlerWrapper( INPUT_OnChange_0, true ) );
        
        _SplusNVRAM.PopulateCustomAttributeList( true );
        
        NVRAM = _SplusNVRAM;
        
    }
    
    public override void LogosSimplSharpInitialize()
    {
        PROCESSOR  = new QscQsys.QsysProcessorSimplInterface();
        ROUTER  = new QscQsys.QsysRouterSimpl();
        
        
    }
    
    public UserModuleClass_QSYS_ROUTER ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}
    
    
    
    
    const uint INPUT__AnalogSerialInput__ = 0;
    const uint INPUTVALUE__AnalogSerialOutput__ = 0;
    const uint COMPONENTNAME__Parameter__ = 10;
    const uint OUTPUTNUMBER__Parameter__ = 11;
    
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
