using Client.UIApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Client.UIApp.Converters
{
    public class SelectedDeviceVMIsSpecificToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }
            DeviceIdentityViewModel devVm = value as DeviceIdentityViewModel;
            return devVm.Type == DeviceIdentityViewModelType.SpecificDeviceItem;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
