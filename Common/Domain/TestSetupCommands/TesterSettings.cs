using Common.Enums;

namespace Common.Domain.TestSetupCommands
{
	public class TesterSettings
	{
        public EnableMode Enable { get; set; } = EnableMode.Enable;
        public byte Position { get; set; } = 0x05;
	}
}