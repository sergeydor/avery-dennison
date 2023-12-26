using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class UIntToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            uint i = value == null ? 0 : System.Convert.ToUInt32(value);
            byte[] barr = BitConverter.GetBytes(i);
            string hex = BitConverter.ToString(barr.Reverse().ToArray()).Replace("-", string.Empty);
            return hex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value as string;
            if (string.IsNullOrWhiteSpace(str))
                return 0;
            
            str = str.Replace(" ", "");

            byte[] barr = (byte[])new BytesToHexStringConverter().ConvertBack(value, null, parameter, null);
            List<byte> barrList = barr.ToList();

            var res = BitConverter.ToUInt32(barr, 0);
            return res;
        }
    }
}
