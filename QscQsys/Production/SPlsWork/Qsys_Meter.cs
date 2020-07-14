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

namespace UserModule_QSYS_METER
{
    public class UserModuleClass_QSYS_METER : SplusObject
    {
        static CCriticalSection g_criticalSection = new CCriticalSection();
        
        Crestron.Logos.SplusObjects.AnalogOutput METERVALUE;
        StringParameter COREID;
        StringParameter COMPONENTNAME;
        UShortParameter INDEX;
        QscQsys.QsysMeter METER;
        public void NEWMETERUPDATE ( ushort VALUE ) 
            { 
            try
            {
                SplusExecutionContext __context__ = SplusSimplSharpDelegateThreadStartCode();
                
                __context__.SourceCodeLine = 15;
                METERVALUE  .Value = (ushort) ( VALUE ) ; 
                
                
            }
            finally { ObjectFinallyHandler(); }
            }
            
        public override object FunctionMain (  object __obj__ ) 
            { 
            try
            {
                SplusExecutionContext __context__ = SplusFunctionMainStartCode();
                
                __context__.SourceCodeLine = 20;
                // RegisterDelegate( METER , ONMETERCHANGE , NEWMETERUPDATE ) 
                METER .onMeterChange  = NEWMETERUPDATE; ; 
                __context__.SourceCodeLine = 21;
                METER . Initialize ( COREID  .ToString(), COMPONENTNAME  .ToString(), (int)( INDEX  .Value )) ; 
                
                
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
            
            METERVALUE = new Crestron.Logos.SplusObjects.AnalogOutput( METERVALUE__AnalogSerialOutput__, this );
            m_AnalogOutputList.Add( METERVALUE__AnalogSerialOutput__, METERVALUE );
            
            INDEX = new UShortParameter( INDEX__Parameter__, this );
            m_ParameterList.Add( INDEX__Parameter__, INDEX );
            
            COREID = new StringParameter( COREID__Parameter__, this );
            m_ParameterList.Add( COREID__Parameter__, COREID );
            
            COMPONENTNAME = new StringParameter( COMPONENTNAME__Parameter__, this );
            m_ParameterList.Add( COMPONENTNAME__Parameter__, COMPONENTNAME );
            
            
            
            _SplusNVRAM.PopulateCustomAttributeList( true );
            
            NVRAM = _SplusNVRAM;
            
        }
        
        public override void LogosSimplSharpInitialize()
        {
            METER  = new QscQsys.QsysMeter();
            
            
        }
        
        public UserModuleClass_QSYS_METER ( string InstanceName, string ReferenceID, Crestron.Logos.SplusObjects.CrestronStringEncoding nEncodingType ) : base( InstanceName, ReferenceID, nEncodingType ) {}
        
        
        
        
        const uint METERVALUE__AnalogSerialOutput__ = 0;
        const uint COREID__Parameter__ = 10;
        const uint COMPONENTNAME__Parameter__ = 11;
        const uint INDEX__Parameter__ = 12;
        
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
