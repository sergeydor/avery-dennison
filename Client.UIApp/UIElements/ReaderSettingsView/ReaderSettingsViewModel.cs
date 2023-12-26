using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.ReaderSettingsView
{
    public class ReaderSettingsViewModel : ViewModelBase
    {
        public TesterSettings TesterSettings { get; set; }

        public void UpdateViewModel(TesterSettings testerSettings)
        {
            this.TesterSettings = testerSettings;
            RaisePropertyChanged(() => TesterSettings);
        }
    }
}
