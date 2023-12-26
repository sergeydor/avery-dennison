using Client.UIApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.UIApp.UIElements
{
    public class BaseView : UserControl
    {
        public static DependencyProperty TitleKeyProperty = DependencyProperty.Register("Title", typeof(string), typeof(BaseView));

        public BaseView()
        {
            this.Loaded += BaseViewLoaded;
        }

        public string Title
        {
            get { return (string)GetValue(TitleKeyProperty); }
            set { SetValue(TitleKeyProperty, value); }
        }

        public ViewModelCommonBase ViewModel
        {
            get { return this.DataContext as ViewModelCommonBase; }
            set
            {
                DataContext = value;
            }
        }

        private void BaseViewLoaded(object sender, RoutedEventArgs e)
        {
        }
    }
}
