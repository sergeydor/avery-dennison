using Client.UIApp.UIElements.LogView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class LogItemToIconSourceConverter : IValueConverter
    {
        public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return GetLogIconPath(value as LogItem);
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //not supported
            return null;
        }

        private Uri GetLogIconPath(LogItem item)
        {
            string resourcesPath = "/Client.UIApp;component/Resources/Icons/Log/{0}.ico";

            if (item is CommonLogItem)
            {
                if(item.LogItemType == LogItemType.Warning)
                    return new Uri(String.Format(resourcesPath, "Warning"), UriKind.RelativeOrAbsolute);
                else
                    return new Uri(String.Format(resourcesPath, "Info"), UriKind.RelativeOrAbsolute);
            }
            else if (item is DeviceCommandLogItem)
            {
                return new Uri(String.Format(resourcesPath, "Trace"), UriKind.RelativeOrAbsolute);
            }
            else
            {
                //throw new ArgumentOutOfRangeException();
                return null;
            }
        }
    }
}
