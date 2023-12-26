using Common.Enums.GSTCommands;

namespace Common.Domain.DeviceResults.GSTCommands
{
	public class RFPowerMeterResult : GeneralDeviceResult
	{
		public PowerMeterRequest Request { get; set; }
		public byte Channel { get; set; }
		public byte Averages { get; set; }
	}
}