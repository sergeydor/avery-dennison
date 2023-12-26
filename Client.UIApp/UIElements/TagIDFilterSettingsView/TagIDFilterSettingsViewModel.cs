using Client.UIApp.ViewModels;
using Common.Domain.TestSetupCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Client.UIApp.UIElements.LogView;
using GalaSoft.MvvmLight;

namespace Client.UIApp.UIElements.TagIDFilterSettingsView
{
    public class TagIDFilterSettingsViewModel : ViewModelBase
    {
        public TagIDFilterSettingsViewModel()
        {
        }

        public TagIDFilterSettings TagIDFilterSettings { get; set; }
        
        public void UpdateTagIDFilterSettings(TagIDFilterSettings st)
        {
            this.TagIDFilterSettings = st;
            RaisePropertyChanged(() => TagIDFilterSettings);
        }
    }
}
