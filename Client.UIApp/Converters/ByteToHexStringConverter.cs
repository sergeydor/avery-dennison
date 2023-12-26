using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class ByteToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv = new BytesToHexStringConverter();
            if (value == null)
                return string.Empty;
            byte[] src = new byte[] { (byte)value };
            var res = conv.Convert(src, null, parameter, null);
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var conv = new BytesToHexStringConverter();
            var res = conv.ConvertBack(value, null, parameter, null);
            byte[] bytes = res as byte[];
            var ret = bytes.Length > 0 ? bytes[0] : byte.MinValue;
            return ret;
        }
    }
}
