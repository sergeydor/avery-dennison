using Common.Enums.GSTCommands;

namespace Common.Domain.GSTCommands
{
	public class HighSpeedTestModeTimer
	{
        public HighSpeedTestDeviceType DeviceType { get; set; } = HighSpeedTestDeviceType.None;
        public ushort D1D0 { get; set; } = 0;
	}
}