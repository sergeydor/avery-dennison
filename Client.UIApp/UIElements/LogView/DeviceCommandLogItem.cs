using Client.Server.Communication.RemoteServices.Dtos.Input;
using Common.Domain.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.UIApp.UIElements.LogView
{
    public class DeviceCommandLogItem : LogItem
    {
        public DeviceCommandLogTransferItem Item { get; set; }

        public override string Message
        {
            get
            {
                return Item.HexData;
            }           
        }
    }
}
