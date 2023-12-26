using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common.Domain.Conveyor;
using Common.Domain.Device;
using Common.Services.Input;
using Common.Services.Output;
using Server.Device.Communication.Domain;
using Server.Device.Communication.RemoteServices.ServiceContracts;

namespace Server.Svc.RemoteServices
{
	public class SimulatorSvcClient : ClientBase<ISimulatorSvc>, ISimulatorSvc
	{
		public SvcOutputGeneric<DeviceConfig> Initialize(SvcInputGeneric<int> readersCountInput)
		{
			return Channel.Initialize(readersCountInput);
		}

		public SvcOutputGeneric<long> GetTimerOffset()
		{
			return Channel.GetTimerOffset();
		}

		public SvcOutputBase ReinstallDsf()
		{
			return Channel.ReinstallDsf();
		}

		public SvcOutputBase ExcludeDevices(List<DeviceIdentity> devices)
		{
			return Channel.ExcludeDevices(devices);
		}

		//public SvcOutputGeneric<ConveyorSettings> GetConveyorSettings()
		//{
		//	return Channel.GetConveyorSettings();
		//}

		public SvcOutputBase Reset()
		{
			return Channel.Reset();
		}

        public SvcOutputBase SetSimulatorSettings(SvcInputGeneric<SimulatorSettings> input)
        {
		return Channel.SetSimulatorSettings(input);
        }

        public SvcOutputBase StopTest()
        {
		return Channel.StopTest();
        }
    }
}