using Common.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{
    [Serializable]
    public class GetShapshotsForTimeIntervalSvcInput : SvcInputBase
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid TestId { get; set; }
    }
}
