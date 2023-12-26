using Client.Server.Communication.RemoteServices.Dtos.Input;
using Client.UIApp.UIElements.AntennaSettingsView;
using Client.UIApp.UIElements.AuxInSettingsView;
using Client.UIApp.UIElements.BasicConfigurationView;
using Client.UIApp.UIElements.ConfigureReadWriteSettingsView;
using Client.UIApp.UIElements.EncoderSettingsView;
using Client.UIApp.UIElements.GPIOSettingsView;
using Client.UIApp.UIElements.LogView;
using Client.UIApp.UIElements.MarkerSettingsView;
using Client.UIApp.UIElements.PucherSettingsView;
using Client.UIApp.UIElements.ReaderSettingsView;
using Client.UIApp.UIElements.SensitivitySettingsView;
using Client.UIApp.UIElements.SetStatisticsView;
using Client.UIApp.UIElements.TagIDFilterSettingsView;
using Client.UIApp.UIElements.TestModeFlagsView;
using Client.UIApp.UIElements.TIDSettingsView;
using Client.UIApp.UIElements.TriggerInputSettingsView;
using Client.UIApp.ViewModels;
using Common.Domain.DeviceResults;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Domain.ExtendedTestSetupCommands;
using Common.Domain.GSTCommands;
using Common.Domain.TestModuleCommands;
using Common.Domain.TestSetupCommands;
using Common.Enums;
using Common.Enums.GSTCommands;
using Common.Services.Input;
using Common.Services.Output;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements
{
    public partial class SettingsScreenViewModel : ViewModelCommonBase
    {
        public BasicConfigurationViewModel BasicConfigurationViewModel { get; set; }
        public TagIDFilterSettingsViewModel TagIDFilterSettingsViewModel { get; set; }
        public TIDSettingsViewModel TIDSettingsViewModel { get; set; }
        public SensivitySettingsViewModel SensivitySettingsViewModel { get; set; }
        public SetStatisticsViewModel SetStatisticsViewModel { get; set; }
        public TestModeFlagsViewModel TestModeFlagsViewModel { get; set; }
        public ConfigureReadWriteSettingsViewModel ConfigureReadWriteSettingsViewModel { get; set; }

        private Dictionary<DeviceIdentityViewModel, TagIDFilterSettings> _tagIDFilterSettingsDict = new Dictionary<DeviceIdentityViewModel, TagIDFilterSettings>();
        private Dictionary<DeviceIdentityViewModel, TIDTestSettings> _tidSettingsDict = new Dictionary<DeviceIdentityViewModel, TIDTestSettings>();
        private Dictionary<DeviceIdentityViewModel, SensitivityTestSettings> _sensSettingsDict = new Dictionary<DeviceIdentityViewModel, SensitivityTestSettings>();
        private Dictionary<DeviceIdentityViewModel, TestSettings> _confRWSettingsDict = new Dictionary<DeviceIdentityViewModel, TestSettings>();

        public ReaderSettingsViewModel ReaderSettingsViewModel { get; set; }
        public AntennaSettingsViewModel AntennaSettingsViewModel { get; set; }
        public MarkerSettingsViewModel MarkerSettingsViewModel { get; set; }
        public PuncherSettingsViewModel PuncherSettingsViewModel { get; set; }
        public TriggerInputSettingsViewModel TriggerInputSettingsViewModel { get; set; }
        public AuxInSettingsViewModel AuxInSettingsViewModel { get; set; }
        public EncoderSettingsViewModel EncoderSettingsViewModel { get; set; }

        private Dictionary<DeviceIdentityViewModel, TesterSettings> _readerSettingsDict = new Dictionary<DeviceIdentityViewModel, TesterSettings>();
        private Dictionary<DeviceIdentityViewModel, AntennaSettings> _antennaSettingsDict = new Dictionary<DeviceIdentityViewModel, AntennaSettings>();
        private Dictionary<DeviceIdentityViewModel, MarkerSettings> _markerSettingsDict = new Dictionary<DeviceIdentityViewModel, MarkerSettings>();
        private Dictionary<DeviceIdentityViewModel, PunchSettings> _puncherSettingsDict = new Dictionary<DeviceIdentityViewModel, PunchSettings>();
        private Dictionary<DeviceIdentityViewModel, TriggerInputSettings> _triggerInputSettingsDict = new Dictionary<DeviceIdentityViewModel, TriggerInputSettings>();
        private Dictionary<DeviceIdentityViewModel, AuxSettings> _auxInSettingsDict = new Dictionary<DeviceIdentityViewModel, AuxSettings>();
        private Dictionary<DeviceIdentityViewModel, EncoderSettings> _encoderSettingsDict = new Dictionary<DeviceIdentityViewModel, EncoderSettings>();
        
        public GPIOSettingsViewModel GPIOSettingsViewModel { get; set; }

        public RelayCommand<DeviceIdentityViewModel> ClearLastFaultCommand { get; set; }
        public RelayCommand<DeviceIdentityViewModel> PingCommand { get; set; }
        public RelayCommand<DeviceIdentityViewModel> ResetCommand { get; set; }

        public RelayCommand SaveAllSettingsCommand { get; set; }
        public RelayCommand ExecuteGetCommandsCommand { get; set; }

        private DeviceIdentityViewModel _selectedDeviceInTestModuleCofiguration = null;
        public DeviceIdentityViewModel SelectedDeviceInTestModuleCofiguration
        {
            get
            {
                return _selectedDeviceInTestModuleCofiguration;
            }
            set
            {
                _selectedDeviceInTestModuleCofiguration = value;
                if(value != null)
                {
                    this.TagIDFilterSettingsViewModel.UpdateTagIDFilterSettings(_tagIDFilterSettingsDict[value]);
                    this.TIDSettingsViewModel.UpdateTIDTestSettings(_tidSettingsDict[value]);
                    this.SensivitySettingsViewModel.UpdateTIDTestSettings(_sensSettingsDict[value]);
                    this.ConfigureReadWriteSettingsViewModel.UpdateViewModel(_confRWSettingsDict[value]);
                }
                else
                {
                    this.TagIDFilterSettingsViewModel.UpdateTagIDFilterSettings(null);
                    this.TIDSettingsViewModel.UpdateTIDTestSettings(null);
                    this.SensivitySettingsViewModel.UpdateTIDTestSettings(null);
                    this.ConfigureReadWriteSettingsViewModel.UpdateViewModel(null);
                }
                RaisePropertyChanged(() => TIDSettingsEnabled);
                RaisePropertyChanged(() => TagIDFilterSettingsEnabled);
                RaisePropertyChanged(() => SensitivitySettingsEnabled);
                RaisePropertyChanged(() => ReadEPCTestEnabled);
                RaisePropertyChanged(() => WriteEPCTestEnabled);

                RaisePropertyChanged(() => SelectedDeviceInTestModuleCofiguration);
            }
        }
        
        public bool TIDSettingsEnabled
        {
            get { return this.SelectedDeviceInTestModuleCofiguration != null && this.TestModeFlagsViewModel.TIDTest; }            
        }

        public bool TagIDFilterSettingsEnabled
        {
            get { return this.SelectedDeviceInTestModuleCofiguration != null && this.TestModeFlagsViewModel.IDFilter; }
        }

        public bool SensitivitySettingsEnabled
        {
            get { return this.SelectedDeviceInTestModuleCofiguration != null && this.TestModeFlagsViewModel.SensTest; }
        }

        public bool ReadEPCTestEnabled
        {
            get { return this.SelectedDeviceInTestModuleCofiguration != null && this.TestModeFlagsViewModel.ReadTest; }
        }

        public bool WriteEPCTestEnabled
        {
            get { return this.SelectedDeviceInTestModuleCofiguration != null && this.TestModeFlagsViewModel.WriteTest; }
        }

        private DeviceIdentityViewModel _selectedDeviceInHardwareCofiguration = null;
        public DeviceIdentityViewModel SelectedDeviceInHardwareCofiguration
        {
            get
            {
                return _selectedDeviceInHardwareCofiguration;
            }
            set
            {
                if (value != null)
                {
                    this.ReaderSettingsViewModel.UpdateViewModel(_readerSettingsDict[value]);
                    this.AntennaSettingsViewModel.UpdateViewModel(_antennaSettingsDict[value]);
                    this.MarkerSettingsViewModel.UpdateViewModel(_markerSettingsDict[value]);
                    this.PuncherSettingsViewModel.UpdateViewModel(_puncherSettingsDict[value]);
                    this.TriggerInputSettingsViewModel.UpdateViewModel(_triggerInputSettingsDict[value]);
                    this.AuxInSettingsViewModel.UpdateViewModel(_auxInSettingsDict[value]);
                    this.EncoderSettingsViewModel.UpdateViewModel(_encoderSettingsDict[value]);
                }
                else
                {
                    this.ReaderSettingsViewModel.UpdateViewModel(null);
                    this.AntennaSettingsViewModel.UpdateViewModel(null);
                    this.MarkerSettingsViewModel.UpdateViewModel(null);
                    this.PuncherSettingsViewModel.UpdateViewModel(null);
                    this.TriggerInputSettingsViewModel.UpdateViewModel(null);
                    this.AuxInSettingsViewModel.UpdateViewModel(null);
                    this.EncoderSettingsViewModel.UpdateViewModel(null);
                }

                _selectedDeviceInHardwareCofiguration = value;
                RaisePropertyChanged(() => SelectedDeviceInHardwareCofiguration);
                RaisePropertyChanged(() => ReaderSettingsEnabled);
            }
        }

        public bool ReaderSettingsEnabled
        {
            get
            {
                return this.SelectedDeviceInHardwareCofiguration != null && this.ReaderSettingsViewModel.TesterSettings != null && 
                    this.ReaderSettingsViewModel.TesterSettings.Enable == EnableMode.Enable;
            }
        }

        public SettingsScreenViewModel(LogViewModel logViewModel, DevicesContext devContext) : base(logViewModel)
        {
            ClearLastFaultCommand = new RelayCommand<DeviceIdentityViewModel>(ClearLastFaultCommandHandler);//, ClearLastFaultCommandHandlerCanExecute);
            PingCommand = new RelayCommand<DeviceIdentityViewModel>(PingCommandHandler);//, PingCommandHandlerCanExecute);
            ResetCommand = new RelayCommand<DeviceIdentityViewModel>(ResetCommandHandler);//, ResetCommandHandlerCanExecute);

            SaveAllSettingsCommand = new RelayCommand(SaveAllSettingsCommandHandler);
            ExecuteGetCommandsCommand = new RelayCommand(ExecuteGetCommandsCommandHandler);

            foreach(var devVm in devContext.ReadersInclAll)
            {
                _tagIDFilterSettingsDict.Add(devVm, new TagIDFilterSettings());
                _tidSettingsDict.Add(devVm, new TIDTestSettings());
                _sensSettingsDict.Add(devVm, new SensitivityTestSettings());
                _confRWSettingsDict.Add(devVm, new TestSettings());
            }

            this.BasicConfigurationViewModel = new BasicConfigurationViewModel(devContext);
            this.TagIDFilterSettingsViewModel = new TagIDFilterSettingsViewModel();
            this.TIDSettingsViewModel = new TIDSettingsViewModel();
            this.SensivitySettingsViewModel = new SensivitySettingsViewModel();
            this.ConfigureReadWriteSettingsViewModel = new ConfigureReadWriteSettingsViewModel();
            this.TestModeFlagsViewModel = new TestModeFlagsViewModel();
            this.TestModeFlagsViewModel.PropertyChanged += TestModeFlagsViewModel_PropertyChanged;
            this.SetStatisticsViewModel = new SetStatisticsViewModel() { TestStatistics = new TestStatistics() };

            foreach (var devVm in devContext.ReadersInclAll)
            {
                _readerSettingsDict.Add(devVm, new TesterSettings());
                _antennaSettingsDict.Add(devVm, new AntennaSettings());
                _markerSettingsDict.Add(devVm, new MarkerSettings());
                _puncherSettingsDict.Add(devVm, new PunchSettings());
                _triggerInputSettingsDict.Add(devVm, new TriggerInputSettings());
                _auxInSettingsDict.Add(devVm, new AuxSettings());
                _encoderSettingsDict.Add(devVm, new EncoderSettings());
            }

            this.ReaderSettingsViewModel = new ReaderSettingsViewModel();
            this.ReaderSettingsViewModel.PropertyChanged += ReaderSettingsViewModel_PropertyChanged;
            this.AntennaSettingsViewModel = new AntennaSettingsViewModel();
            this.MarkerSettingsViewModel = new MarkerSettingsViewModel();
            this.PuncherSettingsViewModel = new PuncherSettingsViewModel();
            this.TriggerInputSettingsViewModel = new TriggerInputSettingsViewModel();
            this.AuxInSettingsViewModel = new AuxInSettingsViewModel();
            this.EncoderSettingsViewModel = new EncoderSettingsViewModel();

            this.GPIOSettingsViewModel = new GPIOSettingsViewModel() { HighSpeedTestModeTimer = new HighSpeedTestModeTimer() };
        }

        private void ReaderSettingsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "TesterSettings.Enable")
            {
                RaisePropertyChanged(() => ReaderSettingsEnabled);
            }
        }

        private void TestModeFlagsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(() => TIDSettingsEnabled);
            RaisePropertyChanged(() => TagIDFilterSettingsEnabled);
            RaisePropertyChanged(() => SensitivitySettingsEnabled);
            RaisePropertyChanged(() => ReadEPCTestEnabled);
            RaisePropertyChanged(() => WriteEPCTestEnabled);
        }
        /*
        public int MarkerPosition
        {
            get
            {
                return Convert.ToInt32(_markerSettingsDict.Values.Average(m => m.Position));
            }
        }

        public int PuncherPosition
        {
            get
            {
                return Convert.ToInt32(_markerSettingsDict.Values.Average(m => m.Position));
            }
        }*/
    }
}
