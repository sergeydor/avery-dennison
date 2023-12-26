using Common.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{
    [Serializable]
    public class GetTopNSnapshotsStartingFromPlanCreateDtInput : SvcInputBase
    {
        public int N { get; set; }
        public DateTime StartFromPlanDt { get; set; }
    }
}