using System.Configuration;

namespace Common.Infrastructure.Devices
{
	public class DeviceConfigSection : ConfigurationSection
	{
		[ConfigurationProperty("vendorId", IsRequired = true)]
		public string VendorId
		{
			get
			{
				return (string)this["vendorId"];
			}
			set
			{
				this["vendorId"] = value;
			}
		}

		[ConfigurationProperty("readerProductId", IsRequired = true)]
		public string ReaderProductId
		{
			get
			{
				return (string)this["readerProductId"];
			}
			set
			{
				this["readerProductId"] = value;
			}
		}

		[ConfigurationProperty("gpioProductId", IsRequired = true)]
		public string GPIOProductId
		{
			get
			{
				return (string)this["gpioProductId"];
			}
			set
			{
				this["gpioProductId"] = value;
			}
		}

		//[ConfigurationProperty("readersCount", IsRequired = true)]
		//public int ReadersCount
		//{
		//	get
		//	{
		//		return (int)this["readersCount"];
		//	}
		//	set
		//	{
		//		this["readersCount"] = value.ToString();
		//	}
		//}
	}
}