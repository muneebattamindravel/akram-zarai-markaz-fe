using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Bitsplash.DatePicker
{
    [RequireComponent(typeof(RectTransform))]
    public abstract class DatePickerCell : MonoBehaviour
    {
        public abstract bool CellSelected { get; set; }
        public abstract bool CellEnabled { get; set; }
        public abstract DateTime DayValue { get; set; }
        public abstract void SetText(string text);
        public abstract void SetInitialSettings(bool enabled, bool selected);
    }
}
