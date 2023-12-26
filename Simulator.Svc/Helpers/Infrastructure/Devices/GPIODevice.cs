using Common.Enums;
using SoftHIDReceiver;

namespace Simulator.Svc.Infrastructure.Devices
{
	public class GPIODevice : DeviceBase
	{
		public GPIODevice(HIDDevice deviceInstance) : base(deviceInstance)
		{
			DeviceType = DeviceType.GPIO;
		}
	}
}