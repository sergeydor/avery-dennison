using Common.Enums;
using Common.Enums.UnsolicitedReplyCommands;

namespace Common.Domain.DeviceResults.UnsolicitedReplyCommands
{
	public class SensitivityTestResult : GeneralDeviceResult
	{
		public PassState PassState { get; set; }
		public short Power { get; set; }
	}
}