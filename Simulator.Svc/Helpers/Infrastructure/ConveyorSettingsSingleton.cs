using Common.Domain.Conveyor;

namespace Simulator.Svc.Infrastructure
{
	public class ConveyorSettingsSingleton
	{
		private static ConveyorSettings _instance;

		public static ConveyorSettings GetInstance()
		{
			if (_instance == null)
			{
				_instance = new ConveyorSettings();
			}

			return _instance;
		}
	}
}