using System.ComponentModel;

namespace Common.Enums
{
	public enum FaultCode : byte
	{
		[Description("No fault condition since get last fault command")]
		NO_FAULT = 0x0,
		[Description("Reset attempt got fault")]
		RESET_FAULT = 0x2,
		[Description("Build In Test - Reader not responding")]
		BIT_READER_FAULT = 0x80,
		[Description("Build In Test - Internal communications fault")]
		BIT_COMMS_FAULT = 0x81,
		[Description("Built In Test - RF subsystem fault")]
		BIT_RF_SUBSYSTEM_FAULT = 0x82,
		[Description("Built In Test - Transmit power detect circuit detected a fault")]
		BIT_TRANSMIT_POWER_FAULT = 0x84
	}
}