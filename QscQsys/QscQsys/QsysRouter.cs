﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Crestron.SimplSharp;
//using Newtonsoft.Json;

//namespace QscQsys
//{
//    public class QsysRouter
//    {
//        private string cName;
//        private bool registered;
//        private int myOutput;

//        public event EventHandler<QsysEventsArgs> QsysRouterEvent;

//        public string ComponentName { get { return cName; } }
//        public bool IsRegistered { get { return registered; } }
//        public int CurrentSelectedInput { set; get; }

//        public QsysRouter(string Name, int output)
//        {
//            cName = Name;
//            myOutput = output;
//            Component component = new Component();
//            component.Name = Name;
//            List<ControlName> names = new List<ControlName>();
//            names.Add(new ControlName());
//            names[0].Name = string.Format("select_{0}", output);

//            component.Controls = names;

//            if (QsysCore.RegisterComponent(component))
//            {
//                QsysCore.Components[component].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(QsysRouter_OnNewEvent);

//                registered = true;
//            }
//        }

//        public void InputSelect(int input)
//        {
//            ComponentChange newInputSelectedChange = new ComponentChange();
//            newInputSelectedChange.Params = new ComponentChangeParams();

//            newInputSelectedChange.Params.Name = cName;

//            ComponentSetValue inputSelected = new ComponentSetValue();
//            inputSelected.Name = string.Format("select_{0}", myOutput);

//            inputSelected.Value = input;

//            newInputSelectedChange.Params.Controls = new List<ComponentSetValue>();
//            newInputSelectedChange.Params.Controls.Add(inputSelected);

//            QsysCore.Enqueue(JsonConvert.SerializeObject(newInputSelectedChange));
//        }

//        private void QsysRouter_OnNewEvent(object sender, QsysInternalEventsArgs e)
//        {
//            if (e.Name.Contains(string.Format("select_{0}", myOutput)))
//            {
//                CurrentSelectedInput = Convert.ToInt16(e.Data);

//                QsysRouterEvent(this, new QsysEventsArgs(eQscEventIds.RouterInputSelected, cName, Convert.ToBoolean(e.Data), Convert.ToInt16(e.Data), e.Data.ToString()));
//            }
//        }
//    }
//}