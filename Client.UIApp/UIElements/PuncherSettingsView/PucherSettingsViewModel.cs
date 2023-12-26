using Common.Domain.ExtendedTestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.PucherSettingsView
{
    public class PuncherSettingsViewModel : ViewModelBase
    {
        public PunchSettings PunchSettings { get; set; }

        public void UpdateViewModel(PunchSettings punchSettings)
        {
            this.PunchSettings = punchSettings;
            RaisePropertyChanged(() => PunchSettings);
        }
    }
}
