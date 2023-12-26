using Common.Domain.DeviceResults.TestModuleCommands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class VersionResultToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            VersionResult res = value as VersionResult;
            if (res == null)
                return "";
            return $"{res.Major}-{res.Minor}-{res.Year}-{res.Month}-{res.Day}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
