using Common.Domain.Device;
using Common.Enums.GSTCommands;
using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.ViewModels
{
    public enum DeviceIdentityViewModelType
    {
        NotDefined = -1,
        AllDevicesItem = 2,
        AllReadersItem = 3,
        SpecificDeviceItem = 4
    }

    public class DeviceIdentityViewModel : ViewModelBase
    {
        public DeviceIdentity DeviceIdentity { get; set; }

        public DeviceIdentityViewModelType Type { get; set; }

        public override string ToString()
        {
            return Type == DeviceIdentityViewModelType.SpecificDeviceItem ?
                DeviceIdentity.DeviceType + " with MacAddr \"" + DeviceIdentity.MacAddress + (DeviceIdentity.DeviceType == HighSpeedTestDeviceType.GPIO ? "" : "\" On Lane " + DeviceIdentity.Lane)
                : Type == DeviceIdentityViewModelType.AllDevicesItem ? "All Devices" 
                : Type == DeviceIdentityViewModelType.AllReadersItem ? "All Readers"
                : "Not Defined";
        }

        public string MacAddrConsideringAllItem
        {
            get
            {
                if(Type == DeviceIdentityViewModelType.AllDevicesItem || Type == DeviceIdentityViewModelType.AllReadersItem)
                {
                    return null;
                }
                return DeviceIdentity.MacAddress;
            }
        }

        public string DisplayText { get { return this.ToString(); } }
    }
}
