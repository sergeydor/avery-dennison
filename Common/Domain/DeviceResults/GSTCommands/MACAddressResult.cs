namespace Common.Domain.DeviceResults.GSTCommands
{
	public class MACAddressResult : GeneralDeviceResult
	{
		public byte MACLength { get; set; } = 0x6;
		public byte Address0 { get; set; }
		public byte Address1 { get; set; }
		public byte Address2 { get; set; }
		public byte Address3 { get; set; }
		public byte Address4 { get; set; }
		public byte Address5 { get; set; }
		public byte CommandCode { get; set; }
	}
}