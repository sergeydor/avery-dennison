using Common.Enums;
using Common.Enums.TestActionCommands;

namespace Common.Domain.DeviceResults.TestActionCommands
{
	public class HardwareVersionResult : GeneralDeviceResult
	{
		public byte CheckByte1 { get; set; }
		public byte CheckByte2 { get; set; }
		public BoardType BoardType { get; set; }
		public byte BoardRevision { get; set; }
		public byte ID4 { get; set; }
		public byte ID3 { get; set; }
		public byte ID2 { get; set; }
		public byte ID1 { get; set; }
	}
}