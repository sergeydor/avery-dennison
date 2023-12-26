using Common.Enums;

namespace Common.Domain.TestSetupCommands
{
	public class TriggerInputSettings
	{
        public EnableMode Enable { get; set; } = EnableMode.Enable;
        public EdgeType EdgeType { get; set; } = EdgeType.Rising;
        public byte Debounce { get; set; } = 1;
        public byte DeafTime { get; set; } = 1;
        public byte TestOffset { get; set; } = 0;
	}
}