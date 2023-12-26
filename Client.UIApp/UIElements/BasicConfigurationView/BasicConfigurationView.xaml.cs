using Client.UIApp.UIHelper;
using Client.UIApp.ViewModels;
using Common.Domain.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Client.UIApp.UIElements.BasicConfigurationView
{
    /// <summary>
    /// Interaction logic for BasicConfigurationView.xaml
    /// </summary>
    public partial class BasicConfigurationView : UserControl
    {
        public static readonly RoutedEvent ClearLastFaultClickEvent = EventManager.RegisterRoutedEvent(
           "ClearLastFaultClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BasicConfigurationView));

        public static readonly RoutedEvent PingClickEvent = EventManager.RegisterRoutedEvent(
           "PingClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BasicConfigurationView));

        public static readonly RoutedEvent ResetClickEvent = EventManager.RegisterRoutedEvent(
           "ResetClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(BasicConfigurationView));

        // Expose this event for this control's container
        public event RoutedEventHandler ClearLastFaultClick
        {
            add { AddHandler(ClearLastFaultClickEvent, value); }
            remove { RemoveHandler(ClearLastFaultClickEvent, value); }
        }

        public event RoutedEventHandler PingClick
        {
            add { AddHandler(PingClickEvent, value); }
            remove { RemoveHandler(PingClickEvent, value); }
        }

        public event RoutedEventHandler ResetClick
        {
            add { AddHandler(ResetClickEvent, value); }
            remove { RemoveHandler(ResetClickEvent, value); }
        }

        public BasicConfigurationView()
        {
            InitializeComponent();
        }

        private void _btClearLastFault_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new SelectedDeviceRoutedEventArgs(ClearLastFaultClickEvent) { SelectedDevice = _cbDevices.SelectedItem as DeviceIdentityViewModel } );
        }

        private void _btPing_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new SelectedDeviceRoutedEventArgs(PingClickEvent) { SelectedDevice = _cbDevices.SelectedItem as DeviceIdentityViewModel });
        }

        private void _btReset_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new SelectedDeviceRoutedEventArgs(ResetClickEvent) { SelectedDevice = _cbDevices.SelectedItem as DeviceIdentityViewModel });
        }
    }
}
