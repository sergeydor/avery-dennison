using System;
using System.ComponentModel;

namespace Common.Enums
{
	[Serializable]
	public enum StatusCode : byte
	{
		[Description("Command completed successfully")]
		OK = 0x0,
		[Description("Command did not complete successfully, but not due to an error")]
		FAILED = 0x1,
		[Description("Packet error")]
		FCS_ERROR = 0x2,
		[Description("Command is unknown to the Test Module")]
		CMD_UNKNOWN = 0x3,
		[Description("Command in not supported by the bootloader")]
		BOOTLOADER_MODE = 0x4,
		[Description("One of the parameters was invalid")]
		INVALID_PARAMETER = 0x5,
		[Description("Hardware cannot support command")]
		HARDWARE_UNAVAILABLE = 0x2C,
		[Description("Invalid frequency parameter")]
		INVALID_FREQUENCY = 0x30,
		[Description("Invalid antenna parameter")]
		INVALID_ANTENNA = 0x31,
		[Description("Invalid tag protocol parameter")]
		INVALID_TAG_PROTOCOL = 0x32,
		[Description("Invalid read power")]
		INVALID_READ_POWER = 0x33,
		[Description("Invalid write power")]
		INVALID_WRITE_POWER = 0x34,
		[Description("Invalid attenuation setting")]
		INVALID_ATTENUATION = 0x35,
		[Description("Invalid RF channel")]
		INVALID_RF_CHANNEL = 0x36,
		[Description("Antenna fault detected on port 1")]
		ANTENNA_1_FAULT = 0x37,
		[Description("Antenna fault detected on port 2")]
		ANTENNA_2_FAULT = 0x38,
		[Description("Monitor table is not valid")]
		INVALID_MONITOR_TABLE = 0x39,
		[Description("Forward power is not within specified limits")]
		TRANSMIT_POWER_FAULT = 0x3A,
		[Description("Invalid setting for calibration/power control")]
		INVALID_CAL_SETTING = 0x3F,
		[Description("M4e failed to set frequency")]
		M4E_FREQUENCY_FAULT = 0x40,
		[Description("M4e failed ti set antenna")]
		M4E_ANTENNA_FAULT = 0x41,
		[Description("M4e failed to set tag protocol")]
		M4E_TAG_PROTOCOL_FAULT = 0x42,
		[Description("M4e failed to set read power")]
		M4E_READ_POWER_FAULT = 0x43,
		[Description("M4e failed to set write power")]
		M4E_WRITE_POWER_FAULT = 0x44,
		[Description("M4e failed to set baud rate")]
		M4E_BAUD_RATE_FAULT = 0x45,
		[Description("M4e failed to boot firmware")]
		M4E_BOOT_FIRMWARE_FAULT = 0x46,
		[Description("M4e failed to set carrier wave on or off")]
		M4E_CW_FAULT = 0x47,
		[Description("Test did not perform properly due to an error")]
		TEST_ERROR = 0x52,
		[Description("Cannot trigger in since testing has not been started")]
		TRIGGER_ERROR_TESTING_NOT_STARTED = 0x54,
        [Description("Status sent when in high speed streaming mode")]
        HIGH_SPEED_STREAMING_MODE = 0x55,
        [Description("Hardware does not support Mark good labels and punch bad labels.")]
        Illegal_mark_punch_combination = 0x56,
        [Description("Both static and moving tests were requested.")]
        Illegal_test_combination = 0x57,
        [Description("use command 0x14 to enable the tester")]
        Tester_disabled = 0x58
    }
}