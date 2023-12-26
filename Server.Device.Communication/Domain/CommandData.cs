namespace Server.Device.Communication
{
	public class CommandData
	{
		public byte SOF { get; set; } = 0xEE;
		public byte LEN { get; set; }
		public byte CMD { get; set; }
		public byte[] DATA { get; set; } = new byte[0];
		public byte FCS { get; set; }
	}
}