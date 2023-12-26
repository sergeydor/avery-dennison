using Common.Enums;

namespace Common.Domain.DeviceResults.UnsolicitedReplyCommands
{
	public class FaultResult : GeneralDeviceResult
	{
		public FaultCode FaultID { get; set; }
	}
}