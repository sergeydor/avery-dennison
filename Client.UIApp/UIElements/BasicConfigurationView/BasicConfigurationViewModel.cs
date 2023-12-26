using Client.UIApp.ViewModels;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Domain.TestModuleCommands;
using Common.Enums;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.BasicConfigurationView
{
    public class BasicConfigurationViewModel : ViewModelBase
    {
        public BasicConfigurationViewModel(DevicesContext devicesCont)
        {
            foreach(DeviceIdentityViewModel devVm in devicesCont.DevicesInclAll)
            {
                VersionByDeviceDict.Add(devVm, new VersionResult());
                DateTimeByDeviceDict.Add(devVm, new LaneDateTime());
                LastFaultByDeviceDict.Add(devVm, new LastFaultResult());
            }
        }

        public Dictionary<DeviceIdentityViewModel, VersionResult> VersionByDeviceDict { get; private set; } = new Dictionary<DeviceIdentityViewModel, VersionResult>();
        public Dictionary<DeviceIdentityViewModel, LaneDateTime> DateTimeByDeviceDict { get; private set; } = new Dictionary<DeviceIdentityViewModel, LaneDateTime>();
        public Dictionary<DeviceIdentityViewModel, LastFaultResult> LastFaultByDeviceDict { get; private set; } = new Dictionary<DeviceIdentityViewModel, LastFaultResult>();

        public InputResetType InputResetType { get; set; } = InputResetType.CPUReset;

        private DeviceIdentityViewModel _selectedDevice = null;
        public DeviceIdentityViewModel SelectedDevice
        {
            get
            {
                return _selectedDevice;
            }
            set
            {
                _selectedDevice = value;                
                RaisePropertyChanged(() => CurrentVersion);
                RaisePropertyChanged(() => CurrentLaneDateTime);
                RaisePropertyChanged(() => CurrentLastFaultResult);
                RaisePropertyChanged(() => SelectedDevice);
            }
        }

        public VersionResult CurrentVersion
        {
            get { return SelectedDevice == null ? null : VersionByDeviceDict[SelectedDevice]; }
            set
            {
                if (SelectedDevice == null) return;
                VersionByDeviceDict[SelectedDevice] = value;
                RaisePropertyChanged(() => CurrentVersion);
            }
        }

        public LaneDateTime CurrentLaneDateTime
        {
            get { return SelectedDevice == null ? null : DateTimeByDeviceDict[SelectedDevice]; }
            set
            {
                if (SelectedDevice == null) return;
                DateTimeByDeviceDict[SelectedDevice] = value;
                RaisePropertyChanged(() => CurrentLaneDateTime);
            }
        }

        public LastFaultResult CurrentLastFaultResult
        {
            get { return SelectedDevice == null ? null : LastFaultByDeviceDict[SelectedDevice]; }
            set
            {
                if (SelectedDevice == null) return;
                LastFaultByDeviceDict[SelectedDevice] = value;
                RaisePropertyChanged(() => CurrentLastFaultResult);
            }
        }

        public void UpdateVersionsSource(Dictionary<DeviceIdentityViewModel, VersionResult> versionByDeviceDict)
        {
            this.VersionByDeviceDict.Clear();
            foreach(var kvp in versionByDeviceDict)
            {
                this.VersionByDeviceDict.Add(kvp.Key, kvp.Value);
            }
            this.VersionByDeviceDict.Add(DevicesContext.Instance.AllDevicesItem, new VersionResult());
            SelectedDevice = null;
        }

        public void UpdateLaneDateTimeSource(Dictionary<DeviceIdentityViewModel, LaneDateTime> dateTimeByDeviceDict)
        {
            this.DateTimeByDeviceDict.Clear();
            foreach (var kvp in dateTimeByDeviceDict)
            {
                this.DateTimeByDeviceDict.Add(kvp.Key, kvp.Value);
            }
            this.DateTimeByDeviceDict.Add(DevicesContext.Instance.AllDevicesItem, new LaneDateTime());
            SelectedDevice = null;
        }

        public void UpdateLastFaultsSource(Dictionary<DeviceIdentityViewModel, LastFaultResult> dateTimeByDeviceDict)
        {
            this.LastFaultByDeviceDict.Clear();
            foreach (var kvp in dateTimeByDeviceDict)
            {
                this.LastFaultByDeviceDict.Add(kvp.Key, kvp.Value);
            }
            this.LastFaultByDeviceDict.Add(DevicesContext.Instance.AllDevicesItem, new LastFaultResult());
            SelectedDevice = null;
        }
    }
}
