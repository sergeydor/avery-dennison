using Common.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Common.Enums;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{
    [DataContract]
    public class StartTestingSvcInput : SvcInputBase
    {
        [DataMember]
        public TestMode TestMode { get; set; }

        [DataMember]
        public string TestName { get; set; }
    }
}
