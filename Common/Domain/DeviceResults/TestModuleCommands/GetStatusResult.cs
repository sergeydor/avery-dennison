using Common.Enums;

namespace Common.Domain.DeviceResults.TestModuleCommands
{
	public class GetStatusResult : GeneralDeviceResult
	{
		public ushort Timer2 { get; set; }
		public byte Temperature { get; set; }
		public CalibrationStatus CalibrationStatus { get; set; }
		public byte OperatingState { get; set; }
	}
}