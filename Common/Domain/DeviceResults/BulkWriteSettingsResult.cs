using Common.Enums;

namespace Common.Domain.DeviceResults
{
	public class BulkWriteSettingsResult : GeneralDeviceResult
	{
		public AntPort AntPort { get; set; }
		public short WritePower { get; set; }
		public ushort WriteTimeout { get; set; }
		public  uint Frequency { get; set; }
		public byte Count { get; set; }
		public WriteType WriteType { get; set; }
		public byte[] TagID { get; set; }
	}
}