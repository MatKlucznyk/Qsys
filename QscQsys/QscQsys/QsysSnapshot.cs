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
        private string coreId;
        private bool registered;

        public void Intialize(string coreId, string name)
        {
            this.cName = name;
            this.coreId = coreId;

            QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (!registered && e.CoreId == coreId)
            {
                registered = true;
            }
        }

        public void LoadSnapshot(ushort number)
        {
            if (registered)
            {
                ComponentChange loadSnapshot = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("load_{0}", number), Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(loadSnapshot, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SaveSnapshot(ushort number)
        {
            if (registered)
            {
                ComponentChange saveSnapshot = new ComponentChange()
                {
                    Params = new ComponentChangeParams()
                    {
                        Name = cName,
                        Controls = new List<ComponentSetValue>() { new ComponentSetValue() { Name = string.Format("save_{0}", number), Value = 1 } }
                    }
                };

                QsysCoreManager.Cores[coreId].Enqueue(JsonConvert.SerializeObject(saveSnapshot, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }
    }
}