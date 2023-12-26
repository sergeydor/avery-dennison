using Common.Enums;

namespace Common.Domain.ExtendedTestSetupCommands
{
	public class PunchSettings
	{
        public PunchEnableMode Enable { get; set; } = PunchEnableMode.PunchBad;
        public byte Position { get; set; } = 0x02;
        public byte Duration { get; set; } = 0x03;
        public byte Offset { get; set; } = 0x04;
	}
}