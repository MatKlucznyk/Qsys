using System;
using System.Collections.Generic;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using ExtensionMethods;
using QscQsys.Intermediaries;

namespace QscQsys
{
    public class QsysNamedControl: IDisposable
    {
        public delegate void NamedControlUIntChange(SimplSharpString cName, ushort uIntData);
        public delegate void NamedControlIntChange(SimplSharpString cName, short intData);
        public delegate void NamedControlStringChange(SimplSharpString cNmae, SimplSharpString stringData);
        public delegate void NamdControlListChange(SimplSharpString cName, ushort itemsCount, SimplSharpString xsig);
        //public delegate void NamedControlChange(SimplSharpString cName, ushort uIntData, short intData, SimplSharpString stringData, SimplSharpString choicesData);
        public delegate void NamedControlListSelectedItem(ushort index, SimplSharpString name);
        //public NamedControlChange newNamedControlChange { get; set; }
        public NamedControlUIntChange newNamedControlUIntChange { get; set; }
        public NamedControlIntChange newNamedControlIntChange { get; set; }
        public NamedControlStringChange newNamedControlStringChange { get; set; }
        public NamdControlListChange newNamedControlListChange { get; set; }
        public NamedControlListSelectedItem newNameControlListSelectedItemChange { get; set; }

        private NamedControl _control;
        private string _cName;
        private string _coreId;
        private bool _isInitialized;
        private bool _isInteger;
        private bool _isList;
        private bool _disposed;
        private readonly List<string> _listData;

        //public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;

        public string ComponentName { get { return _cName; } }
        public bool IsRegistered { get { return Control != null; } }

        public QsysNamedControl()
        {
            _listData = new List<string>();
        }

        public NamedControl Control
        {
            get { return _control; }
            private set
            {
                if (_control == value)
                    return;

                Unsubscribe(_control);
                _control = value;
                Subscribe(_control);
            }
        }

        public void Initialize(string coreId, string name, ushort type)
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            
            _cName = name;
            _coreId = coreId;
            if (type == 1)
            {
                _isInteger = true;
            }
            else if (type == 2)
            {
                _isList = true;
            }

            QsysCoreManager.CoreAdded += QsysCoreManager_CoreAdded;
            RegisterWithCore();
        }

        #region NamedControl Callbacks

        private void Subscribe(NamedControl control)
        {
            if (control == null)
                return;

            control.OnFeedbackReceived += ControlOnFeedbackReceived;
        }

        private void Unsubscribe(NamedControl control)
        {
            if (control == null)
                return;

            control.OnFeedbackReceived -= ControlOnFeedbackReceived;
        }

        private void ControlOnFeedbackReceived(object sender, QsysInternalEventsArgs args)
        {
            if (!_isInteger && !_isList)
            {
                //QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(e.Value), Convert.ToUInt16(e.Value), e.SValue, null));

                if (newNamedControlStringChange != null)
                    newNamedControlStringChange(_cName, args.StringValue);
            }
            else if (_isInteger)
            {
                //var uIntValue = (int)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                //QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(intValue), intValue, Convert.ToString(e.Position), null));

                if (args.Type == "position")
                {
                    if (newNamedControlUIntChange != null)
                    {
                        newNamedControlUIntChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(args.Position)));
                    }
                }
                else if (args.Type == "value")
                {
                    if (newNamedControlIntChange != null)
                    {
                        newNamedControlIntChange(_cName, (short)args.Value);
                    }
                }
                else if (args.Type == "change")
                {
                    if (newNamedControlUIntChange != null)
                    {
                        newNamedControlUIntChange(_cName, (ushort)Math.Round(QsysCoreManager.ScaleUp(args.Position)));
                    }

                    if (newNamedControlIntChange != null)
                    {
                        newNamedControlIntChange(_cName, (short)args.Value);
                    }
                }
            }
            else if (_isList)
            {
                _listData.Clear();
                _listData.AddRange(args.Choices);

                if (newNameControlListSelectedItemChange != null)
                {
                    newNameControlListSelectedItemChange(Convert.ToUInt16(_listData.FindIndex(x => x == args.StringValue) + 1), args.StringValue);
                }

                if (newNamedControlListChange != null)
                {
                    for (int i = 0; i < _listData.Count; i++)
                    {
                        var encodedBytes = XSig.GetBytes(i + 1, _listData[i]);
                        newNamedControlListChange(_cName, Convert.ToUInt16(_listData.Count), Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                        //newNamedControlChange(_cName, Convert.ToUInt16(_listData.Count), Convert.ToInt16(_listData.Count), e.SValue, Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                    }
                }
            }
        }

        #endregion

        void QsysCoreManager_CoreAdded(object sender, CoreEventArgs e)
        {
            if (e.CoreId == _coreId)
            {
                RegisterWithCore();
            }
        }

        private void RegisterWithCore()
        {
            if (Control != null)
                return;

            QsysCore core;
            if (!QsysCoreManager.TryGetCore(_coreId, out core))
            {
                Control = null;
                return;
            }

            Control = core.LazyLoadNamedControl(_cName);
        }

        private void SendControlChangePosition(double value)
        {
            if (Control != null)
            {
                var change = new ControlIntegerChange() { ID = JsonConvert.SerializeObject(new CustomResponseId() { ValueType = "position", Caller = _cName, Method = "Control.Set", Position = value }), Params = new ControlIntegerParams() { Name = _cName, Position = value } };

                Control.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        private void SendControlChangeDoubleValue(double value)
        {
            if (Control != null)
            {
                var change = new ControlIntegerChange() { ID = JsonConvert.SerializeObject(new CustomResponseId() { ValueType = "value", Caller = _cName, Method = "Control.Set", Value = value, StringValue = value.ToString() }), Params = new ControlIntegerParams() { Name = _cName, Value = value } };

                Control.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        private void SendControlChangeStringValue(string value)
        {
            if (Control != null)
            {
                var change = new ControlStringChange() { ID = JsonConvert.SerializeObject(new CustomResponseId() { ValueType = "string_value", Caller = _cName, Method = "Control.Set", StringValue = value }), Params = new ControlStringParams() { Name = _cName, Value = value } };

                Control.Core.Enqueue(JsonConvert.SerializeObject(change, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SetUnsignedInteger(ushort value, ushort scaled)
        {
            if (Control != null)
            {
                if (scaled == 1)
                {
                    SendControlChangePosition(QsysCoreManager.ScaleDown(value));
                }
                else
                {
                    SendControlChangeDoubleValue((double)value);
                }
            }
        }

        public void SetSignedInteger(int value, ushort scaled)
        {
            if (scaled == 1)
            {
                SendControlChangePosition(QsysCoreManager.ScaleDown(value));
            }
            else
            {
                SendControlChangeDoubleValue((double)value);
            }
        }

        public void SetString(string value)
        {
            if (Control != null)
            {
                //ControlStringChange str = new ControlStringChange() { Params = new ControlStringParams() { Name = _cName, Value = value } };

                //QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(str));
                SendControlChangeStringValue(value);
            }
        }

        public void SetBoolean(int value)
        {
            if (Control != null)
            {
                //ControlIntegerChange boolean = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = _cName, Value = value } };

                //QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(boolean, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                SendControlChangeDoubleValue((double)value);
            }
        }

        public void SelectListItem(int index)
        {
            if (Control != null)
            {
                if (_listData != null)
                {
                    if (index >= 0 && index < _listData.Count)
                    {
                        //ControlStringChange str = new ControlStringChange() { Params = new ControlStringParams() { Name = _cName, Value = _listData[index] } };

                        //QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(str));
                        SendControlChangeStringValue(_listData[index]);
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            _disposed = true;
            if (disposing)
            {
                QsysCoreManager.CoreAdded -= QsysCoreManager_CoreAdded;
                Control = null;
            }
        }
    }
}