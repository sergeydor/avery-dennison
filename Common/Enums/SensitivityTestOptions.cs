using System;

namespace Common.Enums
{
	[Flags]
	public enum SensitivityTestOptions : byte
	{
		RFU = 1 | 2 | 4 | 8 | 16 | 32 | 64,
		EffectsResult = 128
	}
}