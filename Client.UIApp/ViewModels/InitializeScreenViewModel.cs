using Client.UIApp.RemoteServices.Clients;
using GalaSoft.MvvmLight.Command;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Common.Domain.Device;
using Common.Services.Input;
using Client.Server.Communication.RemoteServices.Dtos.Output;
using Client.UIApp.UIElements.LogView;
using Common.Services.Output;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Client.Presentation;
using Client.Server.Communication.RemoteServices.Dtos.Input;
using Common.Enums;
using Common.Infrastructure.ErrorHandling.Enums;

namespace Client.UIApp.ViewModels
{
    public class InitializeScreenViewModel : ViewModelCommonBase
    {
        public event EventHandler InitializationCompleted;

		public RelayCommand InitializeCommand { get; set; }
        public RelayCommand AcceptCommand { get; set; }
        public RelayCommand<IEnumerable<DeviceIdentityViewModel>> OpenDebugSettingsCommand { get; set; }
		public RelayCommand ReinstallDsfCommand { get; set; }
		//public RelayCommand ResetCommand { get; set; }

		public ObservableCollection<DeviceIdentityViewModel> DevicesViewModels { get; set; } = new ObservableCollection<DeviceIdentityViewModel>();

	    private const int InitializeTimer = 1000;
		private DispatcherTimer _timer = new DispatcherTimer();
		private List<string> addedMacAddresses = new List<string>();

	    public bool InitializeClicked { get; set; } = false;
        public bool AcceptClicked { get; set; } = false;
	    public bool ReinstallDsfClicked { get; set; } = false;
	    public bool DevicesListEmpty { get; set; } = true;

        private int? _numberOfReaders = null;
        public int? NumberOfReaders
        {
            get { return _numberOfReaders; }
            set
            {
                _numberOfReaders = value;
                RaisePropertyChanged(() => NumberOfReaders);
                InitializeCommand.RaiseCanExecuteChanged();
                //RaisePropertyChanged(() => InitializeCommand);
            }
        }

	    private Task _checkMongoStatusTask = null;

        public InitializeScreenViewModel(LogViewModel logViewModel) : base(logViewModel)
        {
			InitializeCommand = new RelayCommand(InitializeCommandHandler, CanExecuteInitializeHandler);
            AcceptCommand = new RelayCommand(AcceptCommandHandler, CanExecuteAcceptCommandHandler);
			ReinstallDsfCommand = new RelayCommand(ReinstallDsfCommandHandler, CanExecuteReinstallDsfCommandHandler);
			//ResetCommand = new RelayCommand(ResetCommandHandler, CanExecuteResetCommandHandler);
            this.LogViewModel = logViewModel;

	        _checkMongoStatusTask = Task.Run(() => CheckMongoStatus());
        }

        private bool? _isDebugMode = null;
        public bool IsDebugMode
        {
            get
            {
                if(!_isDebugMode.HasValue)
                {
                    _isDebugMode = bool.Parse(ConfigurationManager.AppSettings["IsDebugMode"]);
                }
                return _isDebugMode.Value;
            }
        }

        private AppMode _appMode = AppMode.Emulator;
        public AppMode AppMode
        {
            get { return _appMode; }
            set
            {
                _appMode = value;
                RaisePropertyChanged(() => AppMode);
                RaisePropertyChanged(() => IsSimulator);
                DevicesContext.Instance.AppMode = this.AppMode;
            }
        }

	    
		public bool IsSimulator
        {
            get { return AppMode == AppMode.Emulator; }
        }

	    private bool _isMongoDbStateInvalid;

	    public bool IsMongoDbStateInvalid
        {
			get { return _isMongoDbStateInvalid; }
		    set
		    {
			    _isMongoDbStateInvalid = value;
				RaisePropertyChanged(() => IsMongoDbStateInvalid);
		    }
	    }

        private void InitializeCommandHandler()
        {
            SafeCall(client =>
            {
                LogViewModel.AppendMessage("Devices initialization...");

                System.Diagnostics.Debug.Assert(NumberOfReaders.HasValue);
                SvcOutputGeneric<DeviceConfig> preInitOutput = client.PreInitialize(new PreInitializeSvcInput() { AppMode = this.AppMode, NumberOfDeviceSetOnUI = NumberOfReaders.Value });
                
                if (!preInitOutput.IsOk)
                {
                    LogViewModel.AppendWarning("Something went wrong while pre-initializing (STEP 1) devices...");
                    LogViewModel.AppendWarning(preInitOutput.ToString());
                    return;
                }
                preInitOutput.Output.NumberOfReadersSetOnUI = this.NumberOfReaders.Value;

                var initOutput = client.Initialize(new SvcInputGeneric<DeviceConfig> { Input = preInitOutput.Output });

                if (!initOutput.IsOk)
                {
                    LogViewModel.AppendWarning("Something went wrong while initializing (STEP 1) devices...");
                    LogViewModel.AppendWarning(initOutput.ToString());
                    return;
                }

                DevicesViewModels.Clear();
                bool installed = GetAndRefreshDevices(client);

                if (!installed)
                {
                    _timer.Interval = TimeSpan.FromMilliseconds(InitializeTimer);
                    _timer.Tick += timer_Elapsed;
                    _timer.Start();
                }
                InitializeClicked = true;
                RaisePropertyChanged(() => InitializeCommand);
                RaisePropertyChanged(() => InitializeClicked);
            });
           
            RaiseCanExecuteChanged();
        }

        private void timer_Elapsed(object sender, EventArgs e)
        {
            SafeCall(client =>
            {
                bool installed = GetAndRefreshDevices(client);
                if (installed)
                {
                    LogViewModel.AppendMessage("Devices initialization...OK");
                    _timer.Stop();
                }
                else
                {
                    LogViewModel.AppendMessage("Devices intialization is in progress...");
                }
            });
		}

        private bool GetAndRefreshDevices(AverySvcClient client)
        {
            CheckDevicesInstalledOutput initStatusResult = client.CheckDevicesInitStatus();
            if (!initStatusResult.IsOk)
            {
                LogViewModel.AppendWarning(initStatusResult.ToString());
                return false;
            }
            var devicesRes = client.GetInitializedDevices();
            if (!devicesRes.IsOk)
            {
                LogViewModel.AppendWarning(devicesRes.ToString());
                return false;
            }
            List<DeviceIdentity> devices = devicesRes.Output;

            RefreshDevicesList(devices);
            return initStatusResult.Installed;
        }

	    private void RefreshDevicesList(List<DeviceIdentity> devices)
	    {
			foreach (var device in devices)
			{
				if (addedMacAddresses.Contains(device.MacAddress)) // TODO - review
				{
					continue;
				}
				addedMacAddresses.Add(device.MacAddress);
				DevicesViewModels.Add(new DeviceIdentityViewModel() { DeviceIdentity = device });
				RaisePropertyChanged(() => DevicesViewModels);
				LogViewModel.AppendMessage($"Device with MAC address {device.MacAddress} installed successfully...");
			}
		}

        private void AcceptCommandHandler()
        {
            SafeCall(client => 
            {
                LogViewModel.AppendMessage("Devices initialization step2 - lanes setup...");

                List<DeviceIdentity> devices = DevicesViewModels.Select(x => x.DeviceIdentity).ToList();

	    //        if (string.IsNullOrEmpty(DevicesContext.Instance.AppSessionName))
	    //        {
					//DevicesContext.Instance.AppSessionName = $"AppSession - {DateTime.Now.ToString("G")}";
	    //        }

				var initInput = new InitializeStep2Input
				{
					AppSessionName = DevicesContext.Instance.AppSessionName ?? $"AppSession - {DateTime.Now.ToString("G")}",
					Devices = devices,
					AppMode = AppMode
				};
                SvcOutputGeneric<List<DeviceIdentity>> callResult = client.InitializeStep2(initInput);

				//DevicesContext.Instance.Title = string.Format(TitleFormat, DevicesContext.Instance.AppSessionName, NotDefined);

				if (!callResult.IsOk)
                {
                    LogViewModel.AppendWarning("[ERROR] Something went wrong while initializing (STEP 2) devices...");
                    LogViewModel.AppendWarning(callResult.ToString());
                    return;
                }
                _timer.Stop();
                LogViewModel.AppendMessage("Devices initialization step2 - lanes setup...OK");
				client.StartDeviceListening();
                LogViewModel.AppendMessage("Devices listening started");
                AcceptClicked = true;
                RaisePropertyChanged(() => AcceptClicked);
                RaiseCanExecuteChanged();

                List<string> acceptedDevices = callResult.Output.Select(d => d.MacAddress).ToList();

                var listOfAcceptedDevices = DevicesViewModels.Where(vm => acceptedDevices.Contains(vm.DeviceIdentity.MacAddress)).OrderBy(vm => vm.DeviceIdentity.Lane).ToList();
                listOfAcceptedDevices.Sort((v1, v2) => v1.DeviceIdentity.Lane - v2.DeviceIdentity.Lane);

                foreach (DeviceIdentityViewModel vm in listOfAcceptedDevices)
                {
                    vm.Type = DeviceIdentityViewModelType.SpecificDeviceItem;
                    DevicesContext.Instance.Devices.Add(vm);
                    DevicesContext.Instance.DevicesInclAll.Add(vm);

                    if (vm.DeviceIdentity.DeviceType == Common.Enums.GSTCommands.HighSpeedTestDeviceType.Reader)
                    {
                        DevicesContext.Instance.Readers.Add(vm);
                        DevicesContext.Instance.ReadersInclAll.Add(vm);
                    }
                }
                DevicesContext.Instance.DevicesInclAll.Insert(0, new DeviceIdentityViewModel() { Type = DeviceIdentityViewModelType.AllDevicesItem });
                DevicesContext.Instance.ReadersInclAll.Insert(0, new DeviceIdentityViewModel() { Type = DeviceIdentityViewModelType.AllReadersItem });

                DevicesContext.Instance.AppMode = this.AppMode;

                if (InitializationCompleted != null)
                {
                    InitializationCompleted(this, null);
                }

                UserStepContext.Instance.CompleteInitStep();
            });
            RaiseCanExecuteChanged();
        }

        private void ReinstallDsfCommandHandler()
        {
            SafeCall(client => {
                LogViewModel.AppendMessage("Driver Simnulator Framework re-installation...");
	   //         SvcOutputBase resetResult = client.ResetServicesState();
				//if (!resetResult.IsOk)
	   //         {
				//	LogViewModel.AppendWarning(resetResult.ToString());
	   //         }

                SvcOutputBase res = client.ReinstallDsf();
                if(!res.IsOk)
                {
                    LogViewModel.AppendWarning(res.ToString());
                    return;
                }
                LogViewModel.AppendMessage("Driver Simnulator Framework re-installation...OK");
				ReinstallDsfClicked = true;
            });

            RaiseCanExecuteChanged();
	    }

        /*
	    private void ResetCommandHandler()
	    {
		    SafeCall(client =>
		    {
			    LogViewModel.AppendMessage("Reset processing...");

			    SvcOutputBase res = client.ResetServicesState();
			    if (!res.IsOk)
			    {
				    LogViewModel.AppendWarning(res.ToString());
				    return;
			    }

				var devicesRes = client.GetInitializedDevices();
				if (!devicesRes.IsOk)
				{
					LogViewModel.AppendWarning(devicesRes.ToString());
				}
				List<DeviceIdentity> devices = devicesRes.Output;

				RefreshDevicesList(devices);

				LogViewModel.AppendMessage("Reset processing...OK");
			    DevicesListEmpty = devices.Count == 0;
		    });

			RaiseCanExecuteChanged();
		}*/

        private void RaiseCanExecuteChanged()
        {
            AcceptCommand.RaiseCanExecuteChanged();
            InitializeCommand.RaiseCanExecuteChanged();
            OpenDebugSettingsCommand.RaiseCanExecuteChanged();
			ReinstallDsfCommand.RaiseCanExecuteChanged();
			//ResetCommand.RaiseCanExecuteChanged();
        }

        private bool CanExecuteAcceptCommandHandler()
        {
            return this.InitializeClicked && !AcceptClicked;
        }

        private bool CanExecuteInitializeHandler()
        {
            return !InitializeClicked && DevicesListEmpty && NumberOfReaders.HasValue;
        }
        
        public bool CanExecuteOpenSettingsCommandHandler(IEnumerable<DeviceIdentityViewModel> items)
        {
            return true;
        }

	    public bool CanExecuteReinstallDsfCommandHandler()
	    {
			return !InitializeClicked && !AcceptClicked;
		}

	    public bool CanExecuteResetCommandHandler()
	    {
		    return !ReinstallDsfClicked && !InitializeClicked && !AcceptClicked;
	    }

	    private void CheckMongoStatus()
	    {
		    SafeCall(client =>
			{
				while (true)
				{
                    CheckMongoStatusOutput statusOutput = client.CheckMongoServiceStatus();
                    if (statusOutput.IsMongoDbStopped || (!statusOutput.IsOk && statusOutput.ErrorMessage.ErrorCode == ErrorCode.SVC_CHECK_MONGOSVC_ERROR))
                    {
                        IsMongoDbStateInvalid = true;                        
                    }
                    else if(statusOutput.IsOk && !statusOutput.IsMongoDbStopped)
                    {
                        IsMongoDbStateInvalid = false;
                        return;
                    }
					Thread.Sleep(1000);
				}
			});
		    
	    }
    }
}
