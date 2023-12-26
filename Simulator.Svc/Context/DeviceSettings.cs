using Simulator.Svc.Helpers;

namespace Simulator.Svc.Context
{
	public class DeviceSettings
	{
		public CommandPair Pair { get; set; }
		public byte[] Data { get; set; }
	}
}