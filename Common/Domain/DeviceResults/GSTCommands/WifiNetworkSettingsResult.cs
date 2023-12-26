namespace Common.Domain.DeviceResults.GSTCommands
{
	public class WifiNetworkSettingsResult : GeneralDeviceResult
	{
		public byte CommandCode { get; set; }
		public byte Length { get; set; }
		public byte[] WifiSSID { get; set; }
	}
}