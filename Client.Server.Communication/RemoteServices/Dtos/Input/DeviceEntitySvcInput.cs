using Common.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{
    [Serializable]
    public class DeviceEntitySvcInput<TEntity> : SvcInputGeneric<TEntity>
    {
        public string DeviceMacAddr { get; set; }

        public bool ForAllDevices { get { return DeviceMacAddr == null; } }
    }
}
