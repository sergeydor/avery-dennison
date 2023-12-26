using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.ConfigureReadWriteSettingsView
{
    public class ConfigureReadWriteSettingsViewModel : ViewModelBase
    {
        public TestSettings TestSettings { get; set; }
                
        public bool ReadEnabled { get; set; }

        public bool WriteEnabled { get; set; }

        public bool ReadOrWriteEnabled { get { return ReadEnabled || WriteEnabled; } }

        public void UpdateEnabledState()
        {
            RaisePropertyChanged(() => ReadEnabled);
            RaisePropertyChanged(() => WriteEnabled);
            RaisePropertyChanged(() => ReadOrWriteEnabled);
        }

        public void UpdateViewModel(TestSettings testSettings)
        {
            this.TestSettings = testSettings;
            RaisePropertyChanged(() => TestSettings);
        }
    }
}
