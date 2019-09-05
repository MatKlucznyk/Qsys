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

namespace UserModule_QSYS_GENERIC_STRING_NAMED_CONTROL
{
    public class UserModuleClass_QSYS_GENERIC_STRING_NAMED_CONTROL : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        Crestron.Logos.SplusObjects.StringInput SETSTRINGVALUE;
        Crestron.Logos.SplusObjects.StringOutput STRINGVALUE;
        StringParameter NAMEDCONTROLNAME;
        QscQsys.QsysNamedControlSimpl STRINGCONTROL;
        object SETSTRINGVALUE_OnChange_0 ( Object __EventInfo__ )
        
            { 
            Crestron.Logos.SplusObjects.SignalEventArgs __SignalEventArg__ = (Crestron.Logos.SplusObjects.SignalEventArgs)__EventInfo__;
            try
            {
                SplusExecutionContext __context__ = SplusThreadStartCode(__SignalEventArg__);
                CrestronString X;
                X  = new CrestronString( Crestron.Logos.SplusObjects.CrestronStringEncoding.eEncodingASCII, 255, this );
                
                
                __context__.SourceCodeLine = 18;
                if ( Functions.TestForTrue  ( ( Functions.BoolToInt (SETSTRINGVALUE == ""))  ) ) 
                    { 
                    __context__.SourceCodeLine = 20;
                    STRINGCONTROL . SetString ( "") ; 
                    } 
                
                else 
                    { 
                    __context__.SourceCodeLine = 24;
                    while ( Functions.TestForTrue  ( ( Functions.BoolToInt (X != SETSTRINGVALUE))  ) ) 
                        { 
                        __context__.SourceCodeLine = 26;
                        X  .UpdateValue ( SETSTRINGVALUE  ) ; 
                        __context__.SourceCodeLine = 27;
                        STRINGCONTROL . SetString ( X .ToString()) ; 
                        __context__.SourceCodeLine = 24;
                        } 
                    
                    } 
                
                
                
            }
            catch(Exception e) { ObjectCatchHandler(e); }
            finally { ObjectFinallyHandler( __SignalEventArg__ ); }
            return this;
            
        }
        
    public void NEWSTRINGCHANGE ( ushort X , SimplSharpString VALUE ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
            
            __context__.SourceCodeLine = 34;
            STRINGVALUE  .UpdateValue ( VALUE  .ToString()  ) ; 
            
            
        }
        finally { ObjectFinallyHandler(); }
        }
        
    public override object FunctionMain (  object __obj__ ) 
        { 
        try
        {
            SplusExecutionContext __context__ = SplusFunctionMainStartCode();
            
            __context__.SourceCodeLine = 39;
            // RegisterDelegate( STRINGCONTROL , NEWNAMEDCONTROLCHANGE , NEWSTRINGCHANGE ) 
            STRINGCONTROL .newNamedControlChange  = NEWSTRINGCHANGE; ; 
            __context__.SourceCodeLine = 40;
            STRINGCONTROL . Initialize ( NAMEDCONTROLNAME  .ToString(), (short)( 0 ), (short)( 0 )) ; 
            
            
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
        
        SETSTRINGVALUE = new Crestron.Logos.SplusObjects.StringInput( SETSTRINGVALUE__AnalogSerialInput__, 255, this );
        m_StringInputList.Add( SETSTRINGVALUE__AnalogSerialInput__, SETSTRINGVALUE );
        
        STRINGVALUE = new Crestron.Logos.SplusObjects.StringOutput( STRINGVALUE__AnalogSerialOutput__, this );
        m_StringOutputList.Add( STRINGVALUE__AnalogSerialOutput__, STRINGVALUE );
        
        NAMEDCONTROLNAME = new StringParameter( NAMEDCONTROLNAME__Parameter__, this );
        m_ParameterList.Add( NAMEDCONTROLNAME__Parameter__, NAMEDCONTROLNAME );
        
        
        SETSTRINGVALUE.OnSerialChange.Add( new InputChangeHandlerWrapper( SETSTRINGVALUE_OnChange_0, true ) );
        
        _SplusNVRAM.PopulateCustomAttributeList( true );
        
        NVRAM = _SplusNVRAM;
        
    }
    
    public override void LogosSimplSharpInitialize()
    {
        STRINGCONTROL  = new QscQsys.QsysNamedControlSimpl();
        
        
    }
    
    public UserModuleClass_QSYS_GENERIC_STRING_NAMED_CONTROL ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}
    
    
    
    
    const uint SETSTRINGVALUE__AnalogSerialInput__ = 0;
    const uint STRINGVALUE__AnalogSerialOutput__ = 0;
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
