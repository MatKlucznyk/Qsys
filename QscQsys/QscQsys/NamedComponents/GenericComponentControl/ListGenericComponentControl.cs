using System;
using System.Collections.Generic;
using System.Text;
using Crestron.SimplSharp;
using ExtensionMethods;
using JetBrains.Annotations;

namespace QscQsys.NamedComponents.GenericComponentControl
{
    public class ListGenericComponentControl : AbstractGenericComponentControl
    {

        public delegate void NamdControlListChange(SimplSharpString cName, ushort itemsCount, SimplSharpString xsig);
        public delegate void NamedControlListSelectedItem(ushort index, SimplSharpString name);

        private readonly List<string> _listData;

        [PublicAPI("S+")]
        public NamdControlListChange newNamedControlListChange { get; set; }
        [PublicAPI("S+")]
        public NamedControlListSelectedItem newNameControlListSelectedItemChange { get; set; }

        [PublicAPI("S+")]
        public ListGenericComponentControl()
        {
            _listData = new List<string>();
        }

        [PublicAPI("S+")]
        public void SelectListItem(int index)
        {
            if (Control == null)
                return;

            if (index < 0 || index >= _listData.Count)
                return;

            Control.SendChangeStringValue(_listData[index]);
        }

        private void UpdateList(IEnumerable<string> choices)
        {
            _listData.Clear();
            _listData.AddRange(choices);
        }

        protected override void UpdateState(QsysStateData state)
        {
            base.UpdateState(state);

            if (state == null)
                return;

            UpdateList(state.Choices);

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
    }
}