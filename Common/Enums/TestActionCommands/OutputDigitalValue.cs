namespace Common.Enums.TestActionCommands
{
	// todo: need to know what byte value should be set here
	public enum OutputDigitalValue : byte
	{
		Reserved = 0x0,
		AuxOut = 0x1,
		Punch = 0x2,
		Marker = 0x3,
		Banner = 0x0 | 0x1,
		Encoder = 0xA | 0xB
	}
}