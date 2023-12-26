using Common.Enums;

namespace Common.Domain.TestSetupCommands
{
	public class MarkerSettings
	{
        public MarkerEnableMode Enable { get; set; } = MarkerEnableMode.MarkBadLabels;
        public byte Position { get; set; } = 0x02;
        public byte Duration { get; set; } = 0x03;
        public byte Offset { get; set; } = 0x04;
	}
}