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

namespace Client.UIApp.UIElements.LogView
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class LogView : BaseView
    {
        public LogView()
        {
            InitializeComponent();
            _grLogMessages.SelectionChanged += _grLogMessages_SelectionChanged;
            _grLogDeviceData.SelectionChanged += _grLogDeviceData_SelectionChanged;
        }

        private void _grLogDeviceData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(_grLogDeviceData.SelectedItem != null)
                _grLogDeviceData.ScrollIntoView(_grLogDeviceData.SelectedItem, null);
        }

        private void _grLogMessages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_grLogMessages.SelectedItem != null)
                _grLogMessages.ScrollIntoView(_grLogMessages.SelectedItem, null);
        }
        /*
        private void ScrollToEnd()
        {
            if (_grLogMessages.Items.Count > 0)
            {
                var border = VisualTreeHelper.GetChild(_grLogMessages, 0) as Decorator;
                if (border != null)
                {
                    var scroll = border.Child as ScrollViewer;
                    if (scroll != null) scroll.ScrollToEnd();
                }
            }
        }*/
    }
}
