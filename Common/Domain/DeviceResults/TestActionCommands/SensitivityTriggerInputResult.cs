namespace Common.Domain.DeviceResults.TestActionCommands
{
	public class SensitivityTriggerInputResult : GeneralDeviceResult
	{
		public uint TriggerCount { get; set; }
		public ushort EncoderCount { get; set; }
	}
}