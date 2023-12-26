using Common.Enums;
using Common.Enums.UnsolicitedReplyCommands;

namespace Common.Domain.DeviceResults.UnsolicitedReplyCommands
{
	public class TestE3Result : GeneralDeviceResult
	{
		public TestE3Type TestType { get; set; }
		public TestE3Status TestStatus { get; set; }
		public uint TestCount { get; set; }
		public byte[] EPCTagID { get; set; }
		public ushort Sensitivity { get; set; }
		public byte[] TID { get; set; }
	}
}
