using Client.Presentation;
using Client.UIApp.UIElements.LogView;
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
    public static class UIHelper
    {
        public static void OpenSettingsTestWindow(Window initWnd, List<DeviceIdentityViewModel> devices, LogViewModel logVm)
        {
            TempSettingsWindow settingsWnd = new Presentation.TempSettingsWindow(logVm);
            settingsWnd.Initialize(devices);
            settingsWnd.Show();
            initWnd.Close();
        }
    }
}
