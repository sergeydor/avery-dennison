using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.TriggerInputSettingsView
{
    public class TriggerInputSettingsViewModel : ViewModelBase
    {
        public TriggerInputSettings TriggerInputSettings { get; set; }

        public void UpdateViewModel(TriggerInputSettings triggerInputSettings)
        {
            this.TriggerInputSettings = triggerInputSettings;
            RaisePropertyChanged(() => TriggerInputSettings);
        }
    }
}
