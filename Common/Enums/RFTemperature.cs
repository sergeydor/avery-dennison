using System.ComponentModel;

namespace Common.Enums
{
	public enum RFTemperature : byte
	{
		[Description("Device temperature value is less than 25 C")]
		Less25C = 0,
		[Description("Device temperature value is between 25 C and 29.9 C")]
		Between25And29_9C = 1,
		[Description("Device temperature value is between 30 C and 34.9 C")]
		Between30And34_9C = 2,
		[Description("Device temperature value is greater than 35 C")]
		Greater35C = 3
	}
}