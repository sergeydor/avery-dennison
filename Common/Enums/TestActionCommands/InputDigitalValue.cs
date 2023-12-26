using System;

namespace Common.Enums.TestActionCommands
{
	[Flags]
	public enum InputDigitalValue : byte
	{
		None = 0,
		Reserved = 1 | 16 | 32 | 64 | 128,
		Aux = 2,
		Punch = 4,
		Marker = 8
	}
}