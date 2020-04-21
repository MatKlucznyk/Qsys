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
        private string cName;

        public QsysSnapshot(string name)
        {
            this.cName = name;
        }

        public void LoadSnapshot(int number)
        {
            ComponentChange loadSnapshot = new ComponentChange();
            loadSnapshot.Params = new ComponentChangeParams();
            loadSnapshot.Params.Name = cName;

            ComponentSetValue load = new ComponentSetValue();
            load.Name = string.Format("load_{0}", number);
            load.Value = 1;

            loadSnapshot.Params.Controls = new List<ComponentSetValue>();
            loadSnapshot.Params.Controls.Add(load);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(loadSnapshot, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }

        public void SaveSnapshot(int number)
        {
            ComponentChange saveSnapshot = new ComponentChange();
            saveSnapshot.Params = new ComponentChangeParams();
            saveSnapshot.Params.Name = cName;

            ComponentSetValue save = new ComponentSetValue();
            save.Name = string.Format("save_{0}", number);
            save.Value = 1;

            saveSnapshot.Params.Controls = new List<ComponentSetValue>();
            saveSnapshot.Params.Controls.Add(save);

            QsysProcessor.Enqueue(JsonConvert.SerializeObject(saveSnapshot, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
        }
    }
}