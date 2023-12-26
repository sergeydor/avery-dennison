using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class BytesToHexStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = value is byte[] ? (byte[])value : null;
            if (bytes == null)
            {
                return string.Empty;
            }
            string hex = BitConverter.ToString(bytes.Reverse().ToArray()).Replace("-", string.Empty);
            return hex;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (value ?? "") as string;            
            str = str.Replace(" ", "");
            int shouldBeSymbols = int.Parse(parameter.ToString()) * 2;
            int toAdd = shouldBeSymbols - str.Length;

            for(int i=0; i< toAdd; i++)
            {
                str = '0' + str;
            }

            int numberChars = str.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = System.Convert.ToByte(str.Substring(i, 2), 16);
            var res = bytes.Reverse().ToArray();
                        
            return res;
        }
    }
}
