using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.AuxInSettingsView
{
    public class AuxInSettingsViewModel : ViewModelBase
    {
        public AuxSettings AuxSettings { get; set; }

        public void UpdateViewModel(AuxSettings auxSettings)
        {
            this.AuxSettings = auxSettings;
            RaisePropertyChanged(() => AuxSettings);
        }
    }
}
