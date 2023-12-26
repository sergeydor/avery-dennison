namespace Common.Domain.Device
{
	public class DeviceConfig
	{
		public short VendorId { get; set; }
		public short ReaderProductId { get; set; }
		public short GPIOProductId { get; set; }
		//public int ReadersCount { get; set; }
        public int NumberOfReadersSetOnUI { get; set; }
    }
}