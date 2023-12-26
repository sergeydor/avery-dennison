using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.SensitivitySettingsView
{
    public class SensivitySettingsViewModel : ViewModelBase
    {
        public SensitivityTestSettings Settings { get; set; }

        public void UpdateTIDTestSettings(SensitivityTestSettings sett)
        {
            this.Settings = sett;
            RaisePropertyChanged(() => Settings);
        }
    }
}