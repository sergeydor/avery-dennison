namespace Common.Domain.DeviceResults.GSTCommands
{
	public class DeviceActionStatusResult : GeneralDeviceResult
	{
		// todo. possibly should be bit flags value
		public byte ActionStatus { get; set; }
	}
}