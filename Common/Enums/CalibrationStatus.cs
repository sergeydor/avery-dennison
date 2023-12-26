using System;

namespace Common.Enums
{
	[Flags]
	public enum CalibrationStatus : byte
	{
		BestGuess = 1,
		ReaderOnly = 2,
		Reserved = 4 | 8 | 16,
		InvalidAntenna = 32,
		OutOfCal = 64,
		NoCalTable = 128
	}
}