using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{
    [Serializable]
    public class PreInitializeSvcInput : Common.Services.Input.SvcInputBase
    {
        public int NumberOfDeviceSetOnUI { get; set; }

        public Common.Enums.AppMode AppMode { get; set; }
    }
}
