using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;

namespace QscQsys
{
    public class QsysSnapshot
    {
        //Core
        private QsysCore myCore;

        //Named Component
        private string componentName;
        public string ComponentName { get { return componentName; } }
        private bool registered;
        public bool IsRegistered { get { return registered; } }
        private bool isComponent;

        //Internal Vars

        //Events

        public QsysSnapshot(int _coreID, string _componentName)
        {
            this.componentName = _componentName;
            this.myCore = QsysMain.AddOrGetCoreObject(_coreID);
        }

        public void LoadSnapshot(int _number)
        {
            ComponentChange loadSnapshot = new ComponentChange();
            loadSnapshot.Params = new ComponentChangeParams();
            loadSnapshot.Params.Name = this.componentName;

            ComponentSetValue load = new ComponentSetValue();
            load.Name = string.Format("load_{0}", _number);
            load.Value = 1;

            loadSnapshot.Params.Controls = new List<ComponentSetValue>();
            loadSnapshot.Params.Controls.Add(load);

            this.myCore.Enqueue(JsonConvert.SerializeObject(loadSnapshot));
        }

        public void SaveSnapshot(int _number)
        {
            ComponentChange saveSnapshot = new ComponentChange();
            saveSnapshot.Params = new ComponentChangeParams();
            saveSnapshot.Params.Name = this.componentName;

            ComponentSetValue save = new ComponentSetValue();
            save.Name = string.Format("save_{0}", _number);
            save.Value = 1;

            saveSnapshot.Params.Controls = new List<ComponentSetValue>();
            saveSnapshot.Params.Controls.Add(save);

            this.myCore.Enqueue(JsonConvert.SerializeObject(saveSnapshot));
        }
    }
}