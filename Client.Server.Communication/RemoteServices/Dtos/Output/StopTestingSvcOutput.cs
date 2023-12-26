using Common.Domain.DeviceResults;
using Common.Domain.DeviceResults.GSTCommands;
using Common.Infrastructure.ErrorHandling.Output;
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
    public class StopTestingSvcOutput : SvcOutputBase
    {
        [DataMember]
        public Dictionary<string, GeneralDeviceResult> ReadersStopTestDeviceResults { get; set; } = new Dictionary<string, GeneralDeviceResult>();

        [DataMember]
        public Dictionary<string, ErrorDetails> ReadersStopTestSVCErrors { get; set; } = new Dictionary<string, ErrorDetails>();

        [DataMember]
        public DeviceHighSpeedTestResult GpioHighSpeedDeviceResult { get; set; }

        [DataMember]
        public ErrorDetails GpioHighSpeedSVCError { get; set; }

        public bool AllStopped
        {
            get
            {
                return ReadersStopTestSVCErrors.Count == 0 && GpioHighSpeedSVCError == null &&
                    !ReadersStopTestDeviceResults.Any(r => r.Value.Status != Common.Enums.StatusCode.OK) // no any with status not ok
                    && GpioHighSpeedDeviceResult != null &&
                    (GpioHighSpeedDeviceResult.Status == Common.Enums.StatusCode.OK || GpioHighSpeedDeviceResult.Mode == 0);
            }
        }
    }
}
