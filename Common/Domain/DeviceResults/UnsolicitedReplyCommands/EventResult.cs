using Common.Enums.UnsolicitedReplyCommands;

namespace Common.Domain.DeviceResults.UnsolicitedReplyCommands
{
	public class EventResult : GeneralDeviceResult
	{
		public EventID EventId { get; set; }
		public byte[] TBD { get; set; }
	}
}