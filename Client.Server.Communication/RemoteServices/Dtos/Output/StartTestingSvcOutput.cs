using Common.Domain.Device;
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
    /// <summary>
    /// Returns details based on mac address
    /// </summary>
    [DataContract]
    public class StartTestingSvcOutput : SvcOutputBase
    {
        [DataMember]
        public Dictionary<string, GeneralDeviceResult> ReadersStartTestDeviceResults { get; set; } = new Dictionary<string, GeneralDeviceResult>();

        [DataMember]
        public Dictionary<string, ErrorDetails> ReadersStartTestSVCErrors { get; set; } = new Dictionary<string, ErrorDetails>();

        [DataMember] 
        public DeviceHighSpeedTestResult GpioHighSpeedDeviceResult { get; set; }

        [DataMember]
        public ErrorDetails GpioHighSpeedSVCError { get; set; }

        public bool AllStarted
        {
            get
            {
                return base.IsOk && ReadersStartTestSVCErrors.Count == 0 && GpioHighSpeedSVCError == null &&
                    !ReadersStartTestDeviceResults.Any(r => r.Value.Status != Common.Enums.StatusCode.OK) // no any with status not ok
                    && GpioHighSpeedDeviceResult != null && 
                    (GpioHighSpeedDeviceResult.Status == Common.Enums.StatusCode.OK || GpioHighSpeedDeviceResult.Status == Common.Enums.StatusCode.HIGH_SPEED_STREAMING_MODE);
            }
        }
    }
}
