using Common.Services.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.RemoteServices.Dtos.Output
{
    [DataContract]
    public class SendToDeviceCommonResultSvcOutput : SvcOutputBase
    {
        // Probably more information will be added, depends on who makes decision how to process response from device
    }
}
