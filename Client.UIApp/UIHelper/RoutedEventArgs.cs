using Client.UIApp.ViewModels;
using Common.Domain.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.UIApp.UIHelper
{
    public class SelectedDeviceRoutedEventArgs : RoutedEventArgs
    {
        public SelectedDeviceRoutedEventArgs(RoutedEvent routedEvent) : base(routedEvent) { }

        public DeviceIdentityViewModel SelectedDevice { get; set; }
    }
}
