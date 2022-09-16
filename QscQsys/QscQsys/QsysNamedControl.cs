using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Crestron.SimplSharp;
using Newtonsoft.Json;
using ExtensionMethods;

namespace QscQsys
{
    public class QsysNamedControl: IDisposable
    {
        public delegate void NamedControlChange(SimplSharpString cName, ushort intData, SimplSharpString stringData, SimplSharpString choicesData);
        public delegate void NamedControlListSelectedItem(ushort index, SimplSharpString name);
        public NamedControlChange newNamedControlChange { get; set; }
        public NamedControlListSelectedItem newNameControldListSelectedItemChange { get; set; }

        private Control _control;
        private string _cName;
        private string _coreId;
        private bool _registered;
        private bool _isInteger;
        private bool _isList;
        private bool _disposed;
        private List<string> _listData;

        //public event EventHandler<QsysEventsArgs> QsysNamedControlEvent;

        public string ComponentName { get { return _cName; } }
        public bool IsRegistered { get { return _registered; } }

        public void Initialize(string coreId, string Name, ushort type)
        {
            if (!_registered)
            {  
                _cName = Name;
                this._coreId = coreId;
                if (type == 1)
                {
                    _isInteger = true;
                }
                else if (type == 2)
                {
                    _isList = true;
                }

                QsysCoreManager.CoreAdded += new EventHandler<CoreAddedEventArgs>(QsysCoreManager_CoreAdded);
                RegisterWithCore();
            }
        }

        void QsysCoreManager_CoreAdded(object sender, CoreAddedEventArgs e)
        {
            if (e.CoreId == _coreId)
            {
                RegisterWithCore();
            }
        }

        private void RegisterWithCore()
        {
            if (!_registered)
            {
                if (QsysCoreManager.Cores.ContainsKey(_coreId))
                {
                    _control = new Control(true) { Name = _cName };

                    if (QsysCoreManager.Cores[_coreId].RegisterControl(_control))
                    {
                        QsysCoreManager.Cores[_coreId].Controls[_control].OnNewEvent += new EventHandler<QsysInternalEventsArgs>(Control_OnNewEvent);

                        _registered = true;
                    }
                }
            }
        }

        //add event handling
        private void Control_OnNewEvent(object o, QsysInternalEventsArgs e)
        {

            int intValue;

            if (!_isInteger && !_isList)
            {
                //QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(e.Value), Convert.ToUInt16(e.Value), e.SValue, null));

                if (newNamedControlChange != null)
                    newNamedControlChange(_cName, Convert.ToUInt16(e.Value), e.SValue, string.Empty);
            }
            else if(_isInteger)
            {
                intValue = (int)Math.Round(QsysCoreManager.ScaleUp(e.Position));

                //QsysNamedControlEvent(this, new QsysEventsArgs(eQscEventIds.NamedControlChange, e.Name, Convert.ToBoolean(intValue), intValue, Convert.ToString(e.Position), null));

                if (newNamedControlChange != null)
                    newNamedControlChange(_cName, Convert.ToUInt16(intValue), intValue.ToString(), string.Empty);
            }
            else if (_isList)
            {
                _listData = e.Choices;

                if (newNameControldListSelectedItemChange != null)
                {
                    newNameControldListSelectedItemChange(Convert.ToUInt16(_listData.FindIndex(x => x == e.SValue) + 1), e.SValue);
                }

                if (newNamedControlChange != null)
                {
                    for (int i = 0; i < _listData.Count; i++)
                    {
                        var encodedBytes = XSig.GetBytes(i + 1, _listData[i]);
                        newNamedControlChange(_cName, Convert.ToUInt16(_listData.Count), e.SValue, Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                    }
                }
            }
        }

        private void SendControlChangePosition(double value)
        {
            if (_registered)
            {
                var change = new ControlIntegerChange() {ID = JsonConvert.SerializeObject(new CustomResponseId(){Caller = _cName, Method = "Control.Set", Position = value}), Params = new ControlIntegerParams() { Name = _cName, Position = value } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(change, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        private void SendControlChangeStringValue(string value)
        {
            if (_registered)
            {
                var change = new ControlStringChange() { ID = JsonConvert.SerializeObject(new CustomResponseId() { Caller = _cName, Method = "Control.Set", StringValue = value }), Params = new ControlStringParams() { Name = _cName, Value = value } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(change, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SetInteger(int value, int scaled)
        {
            if (_registered)
            {
                double newValue;
                ControlIntegerChange integer;

                if (scaled == 1)
                {
                    newValue = QsysCoreManager.ScaleDown(value);
                    integer = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = _cName, Position = newValue } };
                }
                else
                {
                    newValue = value;
                    integer = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = _cName, Value = newValue } };
                }

                 

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(integer, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SetString(string value)
        {
            if (_registered)
            {
                ControlStringChange str = new ControlStringChange() { Params = new ControlStringParams() { Name = _cName, Value = value } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(str));
            }
        }

        public void SetBoolean(int value)
        {
            if (_registered)
            {
                ControlIntegerChange boolean = new ControlIntegerChange() { Params = new ControlIntegerParams() { Name = _cName, Value = value } };

                QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(boolean, Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
            }
        }

        public void SelectListItem(int index)
        {
            if (_registered)
            {
                if (_listData != null)
                {
                    if (index >= 0 && index < _listData.Count)
                    {
                        ControlStringChange str = new ControlStringChange() { Params = new ControlStringParams() { Name = _cName, Value = _listData[index] } };

                        QsysCoreManager.Cores[_coreId].Enqueue(JsonConvert.SerializeObject(str));
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
                if (_registered)
                {
                    if (QsysCoreManager.Cores.ContainsKey(_coreId))
                    {
                        if (QsysCoreManager.Cores[_coreId].Controls.ContainsKey(_control))
                        {
                            QsysCoreManager.Cores[_coreId].Controls[_control].OnNewEvent -= Control_OnNewEvent;
                        }
                    }
                    _registered = false;
                }
            }
        }
    }
}