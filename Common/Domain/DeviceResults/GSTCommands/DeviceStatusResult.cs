namespace Common.Domain.DeviceResults.GSTCommands
{
	public class DeviceStatusResult : GeneralDeviceResult
	{
		public byte CommandCode { get; set; }
		public byte DeviceStatus { get; set; }
	}
}