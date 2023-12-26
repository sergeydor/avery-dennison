using Common.Domain.GSTCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.GPIOSettingsView
{
    public class GPIOSettingsViewModel : ViewModelBase
    {
        public HighSpeedTestModeTimer HighSpeedTestModeTimer { get; set; }

        public void UpdateViewModel(HighSpeedTestModeTimer highSpeedTestModeTimer)
        {
            this.HighSpeedTestModeTimer = highSpeedTestModeTimer;
            RaisePropertyChanged(() => HighSpeedTestModeTimer);
        }
    }
}
