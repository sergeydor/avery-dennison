using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class PowerDeviceToUIValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            double val = System.Convert.ToDouble(value);
            if(val == 0)
            { // assume we entered 0 on UI, so convert it to device value
                val = System.Convert.ToInt16(ConvertBack(0, targetType, parameter, culture));
            }
            double res = (val / 10) - 100;
            return res;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;
            double val = System.Convert.ToDouble(value);
            short res = System.Convert.ToInt16( (val + 100) * 10 );
            return res;
        }
    }
}
