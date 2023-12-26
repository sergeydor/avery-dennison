using Common.Enums.TestActionCommands;

namespace Common.Domain.DeviceResults.TestActionCommands
{
	public class DigitalIOResult : GeneralDeviceResult
	{
		public OutputDigitalValue IOValue { get; set; }
	}
}