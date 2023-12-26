using Common.Enums;

namespace Common.Domain.DeviceResults.TestModuleCommands
{
	public class TestRFSettingsResult : GeneralDeviceResult
	{
		public uint Frequency { get; set; }
		public short PowerError { get; set; }
		public RFTemperature Temp { get; set; }
		public ushort CTFAD { get; set; }
		public ushort CTRAD { get; set; }
	}
}