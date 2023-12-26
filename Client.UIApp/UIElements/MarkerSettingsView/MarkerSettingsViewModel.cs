using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.MarkerSettingsView
{
    public class MarkerSettingsViewModel : ViewModelBase
    {
        public MarkerSettings MarkerSettings { get; set; }

        public void UpdateViewModel(MarkerSettings markerSettings)
        {
            this.MarkerSettings = markerSettings;
            RaisePropertyChanged(() => MarkerSettings);
        }
    }
}
