using Common.Enums.GSTCommands;

namespace Common.Domain.DeviceResults.GSTCommands
{
	public class DeviceActivityStateResult : GeneralDeviceResult
	{
		public byte CommandCode { get; set; }
		public ActivityState ActivityState { get; set; }
	}
}