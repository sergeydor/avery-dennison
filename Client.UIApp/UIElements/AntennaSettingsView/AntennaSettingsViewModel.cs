using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.AntennaSettingsView
{
    public class AntennaSettingsViewModel : ViewModelBase
    {
        public AntennaSettings AntennaSettings
        {
            get; set;
        }

        public void UpdateViewModel(AntennaSettings antennaSettings)
        {
            this.AntennaSettings = antennaSettings;
            RaisePropertyChanged(() => AntennaSettings);
        }
    }
}
