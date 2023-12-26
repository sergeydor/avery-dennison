using SoftHIDReceiver;

namespace Simulator.Svc.Infrastructure.Devices
{
	public class DeviceContainer
	{
		private static HIDDeviceContainer _container;

		public static HIDDeviceContainer Container
		{
			get
			{
				if (_container == null)
				{
					_container = new HIDDeviceContainerClass();
				}

				return _container;
			}
		}
	}
}