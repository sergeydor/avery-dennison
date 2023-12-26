using Common.Domain.DeviceResults.TestModuleCommands;
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
    public class LastFaultToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;
            LastFaultResult v = (LastFaultResult)value;
            string name = Enum.GetName(typeof(FaultCode), v.FaultCode);
            string val = ((byte)v.FaultCode).ToString("X8");
            return name + " " + val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
