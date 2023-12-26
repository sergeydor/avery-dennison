using Common.Domain.TestModuleCommands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class LaneDateTimeToDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            LaneDateTime v = (LaneDateTime)value;
            if(v.Year == 0) // not set at all
            {
                return null;
            }
            return new DateTime(v.Year + 2000, v.Month, v.Day, v.Hour, v.Minute, v.Second);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return new LaneDateTime();
            }

            var v = (DateTime)value;
            return new LaneDateTime() { Year = (byte)(v.Year - 2000), Month = (byte)v.Month, Day = (byte)v.Day, Hour = (byte)v.Hour, Minute = (byte)v.Minute, Second = (byte)v.Second };
        }
    }
}
