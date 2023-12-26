namespace Common.Domain.DeviceResults.GSTCommands
{
	public class HighSpeedReaderTestResult : GeneralDeviceResult
	{
		public uint Sensor0Count { get; set; }
		public uint SensorACount { get; set; }
		public byte[] DeviceID { get; set; }
		public byte[] TID { get; set; }
		public byte[] EPC { get; set; }
		public byte Result { get; set; }
	}
}