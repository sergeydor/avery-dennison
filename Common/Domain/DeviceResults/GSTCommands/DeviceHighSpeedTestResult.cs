using Common.Enums.GSTCommands;

namespace Common.Domain.DeviceResults.GSTCommands
{
	public class DeviceHighSpeedTestResult : GeneralDeviceResult
	{
		public HighSpeedTestDeviceType DeviceType { get; set; }
		public byte Mode { get; set; }

		public bool IsExit { get; set; }
	}
}