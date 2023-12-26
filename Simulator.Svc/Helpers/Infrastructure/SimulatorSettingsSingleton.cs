using Server.Device.Communication.Domain;

namespace Simulator.Svc.Infrastructure
{
	public class SimulatorSettingsSingleton
	{
		private static SimulatorSettings _instance;

		public static SimulatorSettings GetInstance()
		{
			if (_instance == null)
			{
				_instance = new SimulatorSettings();
			}

			return _instance;
		}
	}
}