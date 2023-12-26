using System;

namespace Common.Enums.UnsolicitedReplyCommands
{
	[Flags]
	public enum TestE3Status : byte
	{
		RFU = 1,
		RdrFailTimeout = 2,
		RdrFailCRC = 4,
		IDFilter = 8,
		TIDTestFail = 16,
		SensitivityFail = 32,
		WriteFail = 64,
		ReadFail = 128
	}
}