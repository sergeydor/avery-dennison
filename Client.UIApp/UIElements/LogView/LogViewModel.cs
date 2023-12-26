using System;
using Client.Server.Communication.RemoteServices.Dtos.Input;
using Client.UIApp.ViewModels;
using Common.Services.Output;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Client.UIApp.UIElements.LogView
{
    public class LogViewModel : ViewModelCommonBase
    {
        public const int LogLength = 1000;

        private DateTime LastDeviceCmdLogDate = DateTime.Now;
        public ObservableCollection<CommonLogItem> Messages { get; set; } = new ObservableCollection<CommonLogItem>();
        public ObservableCollection<DeviceCommandLogItem> DeviceData { get; set; } = new ObservableCollection<DeviceCommandLogItem>();
		public ObservableCollection<DeviceCommandLogItem> DeviceUnsolicitedData { get; set; } = new ObservableCollection<DeviceCommandLogItem>();

		public bool ShowMessages { get; set; } = true;
        public bool ShowDeviceData { get; set; } = false;
	    public bool ShowDeviceUnsolicitedData { get; set; } = false;
	    public int DeviceUnsolicitedCmdCount { get; set; } = LogLength;
        public int DeviceUnsolicitedCmdStartFrom { get; set; } = 0;


        public ListCollectionView FilteredDeviceData
        {
            get; set;
        }

        private CommonLogItem _selectedMessageItem;
        public CommonLogItem SelectedMessageItem
        {
            get
            {
                return _selectedMessageItem;
            }
            set
            {
                _selectedMessageItem = value;
                RaisePropertyChanged(() => SelectedMessageItem);
            }
        }

        private DeviceCommandLogItem _selectedCmdDataItem;
        public DeviceCommandLogItem SelectedCmdDataItem
        {
            get
            {
                return _selectedCmdDataItem;
            }
            set
            {
                _selectedCmdDataItem = value;
                RaisePropertyChanged(() => SelectedCmdDataItem);
            }
        }

	    private DeviceCommandLogItem _selectedUnsolicitedCmdDataItem;
		public DeviceCommandLogItem SelectedUnsolicitedCmdDataItem
	    {
		    get
		    {
			    return _selectedUnsolicitedCmdDataItem;
		    }
		    set
		    {
			    _selectedUnsolicitedCmdDataItem = value;
				RaisePropertyChanged(() => SelectedUnsolicitedCmdDataItem);
		    }
	    }

	    private DeviceIdentityViewModel _selectedUnsDevice;
		public DeviceIdentityViewModel SelectedUnsDevice
	    {
		    get
		    {
			    return _selectedUnsDevice;
		    }
		    set
		    {
			    _selectedUnsDevice = value;
				RaisePropertyChanged(() => SelectedUnsDevice);
		    }
	    }

        private DeviceIdentityViewModel _selectedDevice;
        public DeviceIdentityViewModel SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;
                RaisePropertyChanged(() => SelectedDevice);

                var selectedDevice = _selectedDevice;

                FilteredDeviceData.Refresh();

                //if (selectedDevice != null && selectedDevice.Type == DeviceIdentityViewModelType.SpecificDeviceItem)
                //{ // filter
                //    int cnt = DeviceData.Count;
                //    for (int i = cnt - 1; i >= 0; i--)
                //    {
                //        if (DeviceData[i].Item.Device.MacAddress != selectedDevice.DeviceIdentity.MacAddress)
                //        {
                //            DeviceData.RemoveAt(i);
                //        }
                //    }
                //}
            }
        }

        public RelayCommand ShowMessagesCommand { get; set; }
        public RelayCommand ShowDeviceDataCommand { get; set; }
		public RelayCommand ShowDeviceUnsolicitedCommand { get; set; }
		public RelayCommand CleanUpDataCommand { get; set; }
		public RelayCommand ApplyUnsolicitedLogFilterCommand { get; set; }
		public RelayCommand SelectDeviceCommand { get; set; }

        private Task _logTask = null;

        //private Timer _dispatcherTimer = new Timer(500);

        public LogViewModel() : base(null)
        {
            base.LogViewModel = this;
            ShowMessagesCommand = new RelayCommand(ShowMessagesCommandHandler);
            ShowDeviceDataCommand = new RelayCommand(ShowDeviceDataCommandHandler);
			ShowDeviceUnsolicitedCommand = new RelayCommand(ShowDeviceUnsolicitedDataCommandHandler);
            CleanUpDataCommand = new RelayCommand(CleanUpDataCommandHandler);
			ApplyUnsolicitedLogFilterCommand = new RelayCommand(ApplyUnsolicitedLogFilterCommandHandler);
            _logTask = Task.Run(() => LoopDeviceData());

            this.FilteredDeviceData = new ListCollectionView(this.DeviceData);
            this.FilteredDeviceData.Filter = FilterDeviceData;

        }

        private bool FilterDeviceData(object obj)
        {
            if(SelectedDevice == null || SelectedDevice.Type == DeviceIdentityViewModelType.AllDevicesItem)
            {
                return true;
            }
            DeviceCommandLogItem logItem = obj as DeviceCommandLogItem;
            return logItem.Item.Device.MacAddress == SelectedDevice.DeviceIdentity.MacAddress;
        }

        private void LoopDeviceData()
        {
            while (true)
            {
                SafeCall(client =>
                {
                    SvcOutputGeneric<List<DeviceCommandLogTransferItem>> itemsRes = client.GetDeviceDataLogItems(
                        new DeviceEntitySvcInput<DateTime>()
                        {
                            Input = LastDeviceCmdLogDate, DeviceMacAddr = null
                        });

                    List<DeviceCommandLogTransferItem> svcItems = itemsRes.Output;

                    if (svcItems.Count == 0)
                    {
                        return; // continue cycle
                    }
                    DateTime borderDt = LastDeviceCmdLogDate;
                    LastDeviceCmdLogDate = svcItems.Max(i => i.ReceiveDt);
                    
                    List<DeviceCommandLogItem> uiDeviceData = new List<UIElements.LogView.DeviceCommandLogItem>(DeviceData);
                    List<string> borderItemsIds = uiDeviceData.Where(d => d.Item.ReceiveDt == borderDt).Select(d => d.Item.DbId).ToList();
                    svcItems = svcItems.Where(d => !borderItemsIds.Contains(d.DbId)).OrderBy(d => d.ReceiveDt).ToList();

                    if (svcItems.Count == 0)
                    {
                        return; // continue cycle
                    }

                    Application.Current.Dispatcher.InvokeAsync(() =>
                    {
                        List<DeviceCommandLogItem> itemsList = svcItems.Select(i => new DeviceCommandLogItem() { Item = i, LogItemType = LogItemType.Info }).ToList();
                        EnsureActualDeviceDataBatch(itemsList);
                    });                    
               });
               Thread.Sleep(500);
            };
        }

        private void ShowMessagesCommandHandler()
        {
            ShowMessages = true;
            ShowDeviceData = false;
	        ShowDeviceUnsolicitedData = false;

			RaisePropertyChanged(() => ShowMessages);
            RaisePropertyChanged(() => ShowDeviceData);
			RaisePropertyChanged(() => ShowDeviceUnsolicitedData);
        }

        private void ShowDeviceDataCommandHandler()
        {
            ShowMessages = false;
            ShowDeviceData = true;
	        ShowDeviceUnsolicitedData = false;

			RaisePropertyChanged(() => ShowMessages);
            RaisePropertyChanged(() => ShowDeviceData);
			RaisePropertyChanged(() => ShowDeviceUnsolicitedData);
        }

	    private void ShowDeviceUnsolicitedDataCommandHandler()
	    {
		    ShowMessages = false;
		    ShowDeviceData = false;
		    ShowDeviceUnsolicitedData = true;

			RaisePropertyChanged(() => ShowMessages);
			RaisePropertyChanged(() => ShowDeviceData);
			RaisePropertyChanged(() => ShowDeviceUnsolicitedData);
	    }

        private void CleanUpDataCommandHandler()
        {
            this.DeviceData.Clear();
            this.Messages.Clear();
			this.DeviceUnsolicitedData.Clear();
            LastDeviceCmdLogDate = DateTime.Now;
        }

	    private void ApplyUnsolicitedLogFilterCommandHandler()
	    {
            if(SelectedUnsDevice == null || SelectedUnsDevice.Type != DeviceIdentityViewModelType.SpecificDeviceItem)
            {
                this.LogViewModel.AppendWarning("Please, select specific device to get unsolicited logs");
                return;
            }

			SafeCall(client =>
			{
				DeviceUnsolicitedData.Clear();
                SvcOutputGeneric<List<DeviceCommandLogTransferItem>> output = client.GetDeviceUnsolicitedDataLogItems(new DeviceEntitySvcInput<Tuple<int, int>>()
                {
                    Input = new Tuple<int, int>(DeviceUnsolicitedCmdStartFrom, DeviceUnsolicitedCmdCount), DeviceMacAddr = SelectedUnsDevice == null ? null : SelectedUnsDevice.MacAddrConsideringAllItem
                });

                var listOfLogs = output.Output;

                var deviceUnsolicitedData = listOfLogs.Skip(Math.Max(0, listOfLogs.Count - DeviceUnsolicitedCmdCount))
					.Select(x => new DeviceCommandLogItem {Item = x, LogItemType = LogItemType.Info}).ToList();

                for (int i = 0; i < deviceUnsolicitedData.Count; i++)
				{
					//if (SelectedUnsDevice != null && SelectedUnsDevice.Type == DeviceIdentityViewModelType.SpecificDeviceItem)
					//{
					//	var deviceData = deviceUnsolicitedData[i];
					//	var inDeviceIdentity = deviceData.Item.Device;
					//	var selectedDeviceIdentity = SelectedUnsDevice.DeviceIdentity;
					//	if (inDeviceIdentity.MacAddress == selectedDeviceIdentity.MacAddress)
					//	{
					//		DeviceUnsolicitedData.Add(deviceUnsolicitedData[i]);
					//	}
					//}
					//else
					{
						DeviceUnsolicitedData.Add(deviceUnsolicitedData[i]);
					}
				}
                SelectedUnsolicitedCmdDataItem = DeviceUnsolicitedData.LastOrDefault();
			});
		}

		private void EnsureActualMessageBatch(List<CommonLogItem> messages)
        {
            for(int i=0; i<messages.Count; i++)
            {
                Messages.Add(messages[i]);
                if (Messages.Count > LogLength)
                    Messages.RemoveAt(0);
            }
            SelectedMessageItem = Messages.Last();
        }

        private void EnsureActualDeviceDataBatch(List<DeviceCommandLogItem> svcDeviceData)
        {
            for (int i = 0; i < svcDeviceData.Count; i++)
            {
                DeviceData.Add(svcDeviceData[i]);
                if (DeviceData.Count > LogLength)
                    DeviceData.RemoveAt(0);
            }
            FilteredDeviceData.Refresh();

            DeviceCommandLogItem last = null;
            foreach (DeviceCommandLogItem i in FilteredDeviceData)
            {
                last = i;
            }
            SelectedCmdDataItem = last; // DeviceData.Last();
        }

        public void AppendMessages(params CommonLogItem[] messages)
        {
            EnsureActualMessageBatch(messages.ToList());
        }

        public void AppendMessages(params string[] messages)
        {
            AppendMessages(messages.Select(m => new CommonLogItem() { Message = m, LogItemType = LogItemType.Info }).ToArray());
        }

        public void AppendWarning(string message)
        {
            AppendMessages(new CommonLogItem() { Message = message, LogItemType = LogItemType.Warning });
        }

        public void AppendMessage(string message)
        {
            AppendMessages(message);
        }

        
    }
}