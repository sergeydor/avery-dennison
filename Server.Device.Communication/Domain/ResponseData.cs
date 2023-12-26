namespace Server.Device.Communication
{
	public class ResponseData
	{
		public byte SOF { get; set; } = 0xEE;
		public byte LEN { get; set; }
		public byte CMD { get; set; }
		public byte STATUS { get; set; }
		public byte TMR_HI { get; set; }
		public byte TMR_LO { get; set; }
		public byte[] DATA { get; set; } = new byte[0x0];
		public byte FCS { get; set; }
	}
}