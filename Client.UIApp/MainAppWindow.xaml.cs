using Client.UIApp.UIElements;
using Client.UIApp.UIElements.LogView;
using Client.UIApp.UIHelper;
using Client.UIApp.ViewModels;
using GalaSoft.MvvmLight.Command;
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
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;
//using Xceed.Wpf.Themes;

namespace Client.UIApp
{
    /// <summary>
    /// Interaction logic for MainAppWindow.xaml
    /// </summary>
    public partial class MainAppWindow : Window
    {
        public MainAppWindow()
        {
            InitializeComponent();

            var logViewModel = new LogViewModel();
            MainAppWindowViewModel mainVm = new MainAppWindowViewModel(logViewModel);
            
            InitializeScreenViewModel initVm = new InitializeScreenViewModel(mainVm.LogViewModel);
            initVm.OpenDebugSettingsCommand = new RelayCommand<IEnumerable<DeviceIdentityViewModel>>(OpenSettingsCommandHandler, initVm.CanExecuteOpenSettingsCommandHandler);
            mainVm.InitializeViewModel = initVm;
            mainVm.InitializeViewModel.InitializationCompleted += (o, a) =>
            {
                mainVm.SettingsViewModel = new SettingsScreenViewModel(mainVm.LogViewModel, DevicesContext.Instance);
                _tabSettings.DataContext = mainVm.SettingsViewModel;
                mainVm.SettingsViewModel.GetAllSettingsEvent += mainVm.OnGetAllSettings;
                mainVm.SettingsViewModel.SetAllSettingsEvent += mainVm.OnSetAllSettings;
            };

            RealTimeViewModel rtVm = new ViewModels.RealTimeViewModel(logViewModel);
            mainVm.RealTimeViewModel = rtVm;

            DataContext = mainVm;
            UserStepContext.Instance.MakeInitStepEnabled();
               /*
            var apprd = Application.Current.Resources;
            Application.Current.Resources.Clear();
            _setStPan.Resources.MergedDictionaries.Clear();

            ChangeTheme(new Xceed.Wpf.Toolkit.Themes.Windows10.Windows10ResourceDictionary(), null);
            ChangeTheme(new Xceed.Wpf.Themes.Windows10.Windows10ResourceDictionary(), null);

            //ChangeTheme(new Xceed.Wpf.Toolkit.Themes.Metro.ToolkitMetroLightThemeResourceDictionary(), null);
            //ChangeTheme(new Xceed.Wpf.Themes.Metro.MetroLightThemeResourceDictionary(), null);

            //ChangeTheme(new Xceed.Wpf.Toolkit.Themes.Office2007.Office2007BlueResourceDictionary(), null);
            //ChangeTheme(new Xceed.Wpf.Themes.Office2007.Office2007BlueResourceDictionary(), null);

            _setStPan.Resources.MergedDictionaries.Add(apprd);*/
        }

        private MainAppWindowViewModel MainVm
        {
            get { return DataContext as MainAppWindowViewModel; }
        }

        public void OpenSettingsCommandHandler(IEnumerable<DeviceIdentityViewModel> items)
        {
            MainAppWindowViewModel mainAppVm = this.DataContext as MainAppWindowViewModel;
            UIHelper.UIHelper.OpenSettingsTestWindow(this, items.ToList(), mainAppVm.LogViewModel);
        }

        private void BasicConfigurationView_ClearLastFaultClick(object sender, RoutedEventArgs e)
        {
            SelectedDeviceRoutedEventArgs selDevArgs = (e as SelectedDeviceRoutedEventArgs);
            MainVm.SettingsViewModel.ClearLastFaultCommand.Execute(selDevArgs.SelectedDevice);
        }

        private void BasicConfigurationView_ResetClick(object sender, RoutedEventArgs e)
        {
            SelectedDeviceRoutedEventArgs selDevArgs = (e as SelectedDeviceRoutedEventArgs);
            MainVm.SettingsViewModel.ResetCommand.Execute(selDevArgs.SelectedDevice);
        }

        private void BasicConfigurationView_PingClick(object sender, RoutedEventArgs e)
        {
            SelectedDeviceRoutedEventArgs selDevArgs = (e as SelectedDeviceRoutedEventArgs);
            MainVm.SettingsViewModel.PingCommand.Execute(selDevArgs.SelectedDevice);
        }
        /*
private void ChangeTheme(ThemeResourceDictionary toolkitTheme, Brush sampleBackgroundBrush)
{
if (toolkitTheme != null)
{
_setStPan.Resources.MergedDictionaries.Add(toolkitTheme);
}
//   _tabCtrl.Background = sampleBackgroundBrush;

//SampleBorder.Background = SystemColors.WindowBrush;
//TextElement.SetForeground(SampleBorder, blackBrush);
}*/
    }
}
