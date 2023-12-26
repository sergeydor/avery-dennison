using Common.Domain.TestSetupCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.EncoderSettingsView
{
    public class EncoderSettingsViewModel : ViewModelBase
    {
        public EncoderSettings EncoderSettings { get; set; }

        public void UpdateViewModel(EncoderSettings encoderSettings)
        {
            this.EncoderSettings = encoderSettings;
            RaisePropertyChanged(() => EncoderSettings);
        }
    }
}
