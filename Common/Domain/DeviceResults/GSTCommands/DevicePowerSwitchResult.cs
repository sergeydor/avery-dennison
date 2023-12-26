using Common.Enums.GSTCommands;

namespace Common.Domain.DeviceResults.GSTCommands
{
	public class DevicePowerSwitchResult : GeneralDeviceResult
	{
		public byte CommandCode { get; set; }
		public PowerMode PowerMode { get; set; }
	}
}