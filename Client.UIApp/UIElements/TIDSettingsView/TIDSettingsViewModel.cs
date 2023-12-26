using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.TIDSettingsView
{
    public class TIDSettingsViewModel : ViewModelBase
    {
        public TIDTestSettings TIDTestSettings { get; private set; } = null;

        public void UpdateTIDTestSettings(TIDTestSettings sett)
        {
            this.TIDTestSettings = sett;
            RaisePropertyChanged(() => TIDTestSettings);
        }
    }
}
