using Common.Domain.Device;
using Common.Enums;
using Common.Enums.GSTCommands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Client.UIApp.ViewModels
{
    public class DevicesContext : ViewModelBase // todo  - rename to appsessioncontext
    {
        protected const string TitleFormat = "Avery Dennison - App Session Name = {0} - Current Test Name = {1}";
        protected const string NotDefined = "Not Defined";
        //private string _title;

        private string _testName = null;
        public string TestName
        {
            get { return _testName; }
            set
            {
                _testName = value;
                RaisePropertyChanged(() => TestName);
                RaisePropertyChanged(() => Title);
            }
        }

        public string Title
	    {
			get
            {
                return string.Format(TitleFormat, AppSessionName ?? NotDefined, TestName ?? NotDefined);
            }		  
	    }

        private string _appSessionName = null; 
		public string AppSessionName
	    {
			get { return _appSessionName; }
		    set
		    {
			    _appSessionName = value;
			    RaisePropertyChanged(() => AppSessionName);
                RaisePropertyChanged(() => Title);
            }
	    }

        public AppMode _appMode = AppMode.Emulator;
        public AppMode AppMode
        {
            get { return _appMode; }
            set
            {
                _appMode = value;
                RaisePropertyChanged(() => AppMode);
                RaisePropertyChanged(() => IsSimulator);
            }
        }

        public bool IsSimulator
        {
            get
            {
                return AppMode == AppMode.Emulator;                
            }
        }

        private CommandsProcessingMode _testMode = CommandsProcessingMode.NotRunning;
        public CommandsProcessingMode TestMode
        {
            get { return _testMode; }
            set
            {
                _testMode = value;
                RaisePropertyChanged(() => TestMode);
                RaisePropertyChanged(() => IsRunning);
            }
        } 

        public bool IsRunning
        {
            get
            {
                return _testMode == CommandsProcessingMode.Running;
            }
        }

        private static Lazy<DevicesContext> _instance = new Lazy<DevicesContext>();

        public static DevicesContext Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        public DevicesContext()
        {
        }

        public DeviceIdentityViewModel AllReadersItem
        {
            get { return this.ReadersInclAll.FirstOrDefault(r => r.Type == DeviceIdentityViewModelType.AllReadersItem); } 
        }

        public DeviceIdentityViewModel AllDevicesItem
        {
            get { return this.DevicesInclAll.FirstOrDefault(r => r.Type == DeviceIdentityViewModelType.AllDevicesItem); }
        }

        public DeviceIdentityViewModel GPIODeviceVm
        {
            get
            {
                return DevicesContext.Instance.Devices.FirstOrDefault(d => d.DeviceIdentity.DeviceType == HighSpeedTestDeviceType.GPIO);
            }
        }

        public ObservableCollection<DeviceIdentityViewModel> Devices { get; set; } = new ObservableCollection<DeviceIdentityViewModel>();
        public ObservableCollection<DeviceIdentityViewModel> DevicesInclAll { get; set; } = new ObservableCollection<DeviceIdentityViewModel>();
        public ObservableCollection<DeviceIdentityViewModel> Readers { get; set; } = new ObservableCollection<DeviceIdentityViewModel>();
        public ObservableCollection<DeviceIdentityViewModel> ReadersInclAll { get; set; } = new ObservableCollection<DeviceIdentityViewModel>();
    }
}
