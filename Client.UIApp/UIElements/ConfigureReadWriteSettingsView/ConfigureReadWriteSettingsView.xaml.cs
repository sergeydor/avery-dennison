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

namespace Client.UIApp.UIElements.ConfigureReadWriteSettingsView
{
    /// <summary>
    /// Interaction logic for ConfigureReadWriteSettingsView.xaml
    /// </summary>
    public partial class ConfigureReadWriteSettingsView : UserControl
    {
        public static readonly DependencyProperty ReadEnabledProperty;
        public static readonly DependencyProperty WriteEnabledProperty;

        static ConfigureReadWriteSettingsView()
        {
            ReadEnabledProperty = DependencyProperty.Register("ReadEnabled", typeof(bool), typeof(ConfigureReadWriteSettingsView));
            WriteEnabledProperty = DependencyProperty.Register("WriteEnabled", typeof(bool), typeof(ConfigureReadWriteSettingsView));
        }

        public bool ReadEnabled
        {
            set
            {
                SetValue(ReadEnabledProperty, value);
            }
        }

        public bool WriteEnabled
        {
            set
            {
                SetValue(WriteEnabledProperty, value);
            }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            ConfigureReadWriteSettingsViewModel vm = DataContext as ConfigureReadWriteSettingsViewModel;

            if (e.Property.Name == ReadEnabledProperty.Name)
            {
                bool val = (bool)e.NewValue;
                vm.ReadEnabled = val;
                vm.UpdateEnabledState();
            }
            else if(e.Property.Name == WriteEnabledProperty.Name)
            {
                bool val = (bool)e.NewValue;
                vm.WriteEnabled = val;
                vm.UpdateEnabledState();
            }
        }

        public ConfigureReadWriteSettingsView()
        {
            InitializeComponent();
        }
    }
}
