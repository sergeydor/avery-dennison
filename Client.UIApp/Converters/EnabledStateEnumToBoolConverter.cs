using Common.Enums;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class EnabledStateEnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            byte res = (byte)value;
            return res != 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) // from bool to enum
        {
            if(value == null)
            {
                if(targetType == typeof(MarkerEnableMode))
                { 
                    return MarkerEnableMode.Disable;
                }
                else if (targetType == typeof(PunchEnableMode))
                {
                    return MarkerEnableMode.Disable;
                }
                else if(targetType == typeof(EnableMode))
                {
                    return EnableMode.Disable;
                }                
            }
            bool v = (bool)value;
            
            if (targetType == typeof(MarkerEnableMode))
            {                
                return v ? MarkerEnableMode.MarkGoodLabels : MarkerEnableMode.Disable;
            }
            else if (targetType == typeof(PunchEnableMode))
            {
                return v ? PunchEnableMode.PunchGood : PunchEnableMode.Disable;
            }
            else if (targetType == typeof(EnableMode))
            {
                return v ? EnableMode.Enable : EnableMode.Disable;
            }
            return null;
        }
    }
}
