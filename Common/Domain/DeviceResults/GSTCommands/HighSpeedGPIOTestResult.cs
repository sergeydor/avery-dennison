namespace Common.Domain.DeviceResults.GSTCommands
{
	public class HighSpeedGPIOTestResult : GeneralDeviceResult
	{
		public uint Sensor0Count { get; set; }
		public float SensorACount { get; set; }
		public byte[] DeviceID { get; set; }
	}
}