namespace Common.Domain.TestSetupCommands
{
	public class TIDTestSettings
	{
        public byte Options { get; set; } = 0x01;
        public ushort ReadTimeout { get; set; } = 0;
        public ushort Interval { get; set; } = 0x0001;
        public uint TID { get; set; } = 0;
	}
}