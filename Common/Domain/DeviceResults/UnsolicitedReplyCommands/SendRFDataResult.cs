namespace Common.Domain.DeviceResults.UnsolicitedReplyCommands
{
	public class SendRFDataResult : GeneralDeviceResult
	{
		public ushort IFAD { get; set; }
		public ushort IRAD { get; set; }
	}
}
