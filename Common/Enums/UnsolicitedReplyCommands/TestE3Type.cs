using System;

namespace Common.Enums.UnsolicitedReplyCommands
{
	[Flags]
	public enum TestE3Type : byte
	{
		F1Read = 1,
		F2Read = 2,
		F3Read = 4,
		IDFilter = 8,
		TIDFilter = 16,
		SensTest = 32,
		WriteTest = 64,
		ReadTest = 128
	}
}