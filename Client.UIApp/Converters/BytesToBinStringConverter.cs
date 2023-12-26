using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class BytesToBinStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            byte[] bytes = value is byte[] ? (byte[])value : null;
            if (bytes == null)
                return string.Empty;

            BitArray bitarr = new BitArray(bytes);
            StringBuilder bld = new StringBuilder();

            foreach (bool b in bitarr)
            {
                bld.Append(b ? '1' : '0');
            }
            string s = bld.ToString();
            var resstr = new string(s.Reverse().ToArray());
            return resstr;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = (value ?? "") as string;            
            str = str.Replace(" ", "");
            int shouldBeBits = int.Parse(parameter.ToString()) * 8;

            int add0Preffix = shouldBeBits - str.Length; // normalize
            for (int i = 0; i < add0Preffix; i++)
                str = '0' + str;

            List<bool> boolArr = str.Select(c => c == '1' ? true : false).ToList();
            BitArray bits = new BitArray(boolArr.ToArray());
            byte[] result = ToByteArray(bits);

            var res = result.Reverse().ToArray();            
            return res;
        }

        private static byte[] ToByteArray(BitArray bits)
        {
            int numBytes = bits.Count / 8;
            if (bits.Count % 8 != 0) numBytes++;

            byte[] bytes = new byte[numBytes];
            int byteIndex = 0, bitIndex = 0;

            for (int i = 0; i < bits.Count; i++)
            {
                if (bits[i])
                    bytes[byteIndex] |= (byte)(1 << (7 - bitIndex));

                bitIndex++;
                if (bitIndex == 8)
                {
                    bitIndex = 0;
                    byteIndex++;
                }
            }

            return bytes;
        }       
    }
}
