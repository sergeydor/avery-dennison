using Client.UIApp.UIElements.LogView;
using Client.UIApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements
{
    public class MainAppWindowViewModel : ViewModelCommonBase
    {
        public MainAppWindowViewModel(LogViewModel logViewModel) : base(logViewModel)
        {
            LogViewModel = logViewModel;
			//DevicesContext.Instance.Title = string.Format(TitleFormat, NotDefined, NotDefined);
		}

        public InitializeScreenViewModel InitializeViewModel { get; set; }

        public SettingsScreenViewModel SettingsViewModel { get; set; }

        public RealTimeViewModel RealTimeViewModel { get; set; }

        public void OnSetAllSettings(object s, EventArgs args)
        {
            //this.SettingsViewModel.EncoderSettingsViewModel.
        }

        public void OnGetAllSettings(object s, EventArgs args)
        {

        }

        //public LogViewModel LogViewModel { get; private set; }
    }
}
