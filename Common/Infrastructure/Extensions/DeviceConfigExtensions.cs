using System.Globalization;
using Common.Domain;
using Common.Domain.Device;
using Common.Infrastructure.Constants;
using Common.Infrastructure.Devices;

namespace Common.Infrastructure.Extensions
{
	public static class DeviceConfigExtensions
	{
		public static DeviceConfig GetDeviceConfig(this DeviceConfigSection deviceConfigSection)
		{
			short vendorId;
			short readerProductId;
			short gpioProductId;
			if (!short.TryParse(deviceConfigSection.VendorId, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out vendorId))
			{
				vendorId = ConfigConstants.DefaultId;
			}
			if (!short.TryParse(deviceConfigSection.ReaderProductId, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out readerProductId))
			{
				readerProductId = ConfigConstants.DefaultId;
			}
			if (!short.TryParse(deviceConfigSection.GPIOProductId, NumberStyles.HexNumber, CultureInfo.CurrentCulture, out gpioProductId))
			{
				gpioProductId = ConfigConstants.DefaultId;
			}

			var result = new DeviceConfig
			{
				VendorId = vendorId,
				ReaderProductId = readerProductId,
				GPIOProductId = gpioProductId,
			//	ReadersCount = deviceConfigSection.ReadersCount
			};

			return result;
		}
	}
}