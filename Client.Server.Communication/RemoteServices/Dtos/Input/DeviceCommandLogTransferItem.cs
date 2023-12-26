using Common.Domain.Device;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{    
    public class DeviceCommandLogTransferItem
    {
        public string DbId { get; set; }

        public DeviceIdentity Device { get; set; }
        public string HexData { get; set; }
        public Direction Direction { get; set; }
        public DateTime ReceiveDt { get; set; }

        public TimeSpan ReceiveTs
        {
            get
            {
                return ReceiveDt.TimeOfDay;
            }
        }
    }
}
