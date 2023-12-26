using Server.Device.Communication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Svc.Domain
{
    public class DeviceCommandsLink
    {
        private object _lockObj = new object();

        public DeviceOutgoingCommand _outgoing;
        public DeviceIncommingCommand _incomming;

        public DeviceOutgoingCommand Outgoing
        {
            get
            {
                return this._outgoing;
            }
            set
            {
                this._outgoing = value;
            }
        }

        public DeviceIncommingCommand Incomming
        {
            get
            {
                lock (_lockObj)
                    return this._incomming;
            }
            set
            {
                lock (_lockObj)
                    this._incomming = value;
            }
        }

        public bool IsProcessed
        {
            get
            {
                return this.Incomming != null;
            }
        }
    }
}
