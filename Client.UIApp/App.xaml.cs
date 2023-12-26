using Client.UIApp;
using Client.UIApp.ViewModels;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Client.Presentation
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //InitWindow wnd = new InitWindow();
            //InitializeScreenViewModel vm = new InitializeScreenViewModel();
            //vm.OpenSettingsCommand = new RelayCommand<IEnumerable<DeviceIdentityViewModel>>(wnd.OpenSettingsCommandHandler, vm.CanExecuteOpenSettingsCommandHandler);
            //wnd.DataContext = vm;
            //wnd.Show();

            MainAppWindow wnd = new UIApp.MainAppWindow();
            wnd.Show();

            base.OnStartup(e);
        }
    }
}
