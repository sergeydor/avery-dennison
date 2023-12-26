using Common.Enums;
using SoftHIDReceiver;

namespace Simulator.Svc.Infrastructure.Devices
{
	public class ReaderDevice : DeviceBase
	{
		public ReaderDevice(HIDDevice deviceInstance) : base(deviceInstance)
		{
			DeviceType = DeviceType.Reader;
		}
	}
}