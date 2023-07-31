using System;
using System.Collections.Generic;
using System.Text;
using Crestron.SimplSharp;
using ExtensionMethods;
using QscQsys.Intermediaries;
using QscQsys.Utils;

namespace QscQsys.NamedControls
{
    public sealed class QsysNamedControl: IDisposable
    {
        public delegate void NamedControlUIntChange(SimplSharpString cName, ushort uIntData);
        public delegate void NamedControlIntChange(SimplSharpString cName, short intData);
        public delegate void NamedControlStringChange(SimplSharpString cNmae, SimplSharpString stringData);
        public delegate void NamdControlListChange(SimplSharpString cName, ushort itemsCount, SimplSharpString xsig);
        public delegate void NamedControlListSelectedItem(ushort index, SimplSharpString name);
        public NamedControlUIntChange newNamedControlUIntChange { get; set; }
        public NamedControlIntChange newNamedControlIntChange { get; set; }
        public NamedControlStringChange newNamedControlStringChange { get; set; }
        public NamdControlListChange newNamedControlListChange { get; set; }
        public NamedControlListSelectedItem newNameControlListSelectedItemChange { get; set; }

        private NamedControl _control;
        private bool _disposed;
        private readonly List<string> _listData;

        public string ControlName { get; private set; }
        private string CoreId { get; set; }
        private bool IsInitialized { get; set; }
        private bool IsInteger { get; set; }
        private bool IsList { get; set; }

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

                if (_control != null)
                    UpdateState(_control.State);
            }
        }

        public void Initialize(string coreId, string name, ushort type)
        {
            if (IsInitialized)
                return;
            IsInitialized = true;
            
            ControlName = name;
            CoreId = coreId;
            if (type == 1)
            {
                IsInteger = true;
            }
            else if (type == 2)
            {
                IsList = true;
            }

            QsysCoreManager.CoreAdded += QsysCoreManager_CoreAdded;
            RegisterWithCore();
        }

        #region Set Values

        public void SetUnsignedInteger(ushort value, ushort scaled)
        {
            if (Control == null)
                return;

            if (scaled == 1)
                Control.SendChangePosition(SimplUtils.ScaleToDouble(value));
            else
                Control.SendChangeDoubleValue(value);
        }

        public void SetSignedInteger(int value, ushort scaled)
        {
            if (Control == null)
                return;

            if (scaled == 1)
                Control.SendChangePosition(SimplUtils.ScaleToDouble(value));
            else
                Control.SendChangeDoubleValue(value);
        }

        public void SetBoolean(ushort value)
        {
            if (Control == null)
                return;

            Control.SendChangeBoolValue(value.BoolFromSplus());
        }

        public void SetString(string value)
        {
            if (Control == null)
                return;

            Control.SendChangeStringValue(value);
        }

        #endregion

        #region Update States

        private void UpdateState(QsysStateData state)
        {
            if (!IsInteger && !IsList)
            {
                UpdateStringState(state);
            }
            else if (IsInteger)
            {
                UpdatateIntegerState(state);
            }
            else if (IsList)
            {
                UpdateListState(state);
            }
        }

        private void UpdateListState(QsysStateData state)
        {
            _listData.Clear();
            _listData.AddRange(state.Choices);

            var listSelectedCallback = newNameControlListSelectedItemChange;
            if (listSelectedCallback != null)
            {
                listSelectedCallback(Convert.ToUInt16(_listData.FindIndex(x => x == state.StringValue) + 1),
                                     state.StringValue);
            }

            var listItemCallback = newNamedControlListChange;
            if (listItemCallback != null)
            {
                for (int i = 0; i < _listData.Count; i++)
                {
                    var encodedBytes = XSig.GetBytes(i + 1, _listData[i]);
                    listItemCallback(ControlName, Convert.ToUInt16(_listData.Count),
                                     Encoding.GetEncoding(28591).GetString(encodedBytes, 0, encodedBytes.Length));
                }
            }
        }

        private void UpdatateIntegerState(QsysStateData state)
        {
            var positionCallback = newNamedControlUIntChange;
            var valueCallback = newNamedControlIntChange;
            switch (state.Type)
            {
                case "position":

                    if (positionCallback != null)
                        positionCallback(ControlName, SimplUtils.ScaleToUshort(state.Position));
                    break;
                case "value":

                    if (valueCallback != null)
                        valueCallback(ControlName, (short)state.Value);
                    break;
                case "change":
                    if (positionCallback != null)
                        positionCallback(ControlName, SimplUtils.ScaleToUshort(state.Position));
                    if (valueCallback != null)
                        valueCallback(ControlName, (short)state.Value);
                    break;
            }
        }

        private void UpdateStringState(QsysStateData state)
        {
            var stringCallback = newNamedControlStringChange;
            if (stringCallback != null)
                stringCallback(ControlName, state.StringValue);
        }

        public void SelectListItem(int index)
        {
            if (Control == null)
                return;

            if (index < 0 || index > _listData.Count)
                return;

            Control.SendChangeStringValue(_listData[index]);
        }

        #endregion

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
            UpdateState(args.Data);
        }

        #endregion

        #region CoreManager Callbacks

        void QsysCoreManager_CoreAdded(object sender, CoreEventArgs e)
        {
            if (e.CoreId == CoreId)
                RegisterWithCore();
        }

        private void RegisterWithCore()
        {
            Control = null;

            QsysCore core;
            if (QsysCoreManager.TryGetCore(CoreId, out core))
                Control = core.LazyLoadNamedControl(ControlName);
        }

        #endregion

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