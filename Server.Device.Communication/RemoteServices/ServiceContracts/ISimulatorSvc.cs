using System.Collections.Generic;
using System.ServiceModel;
using Common.Domain.Conveyor;
using Common.Domain.Device;
using Common.Services.Output;
using Server.Device.Communication.Domain;
using Common.Services.Input;

namespace Server.Device.Communication.RemoteServices.ServiceContracts
{
	[ServiceContract]
	public interface ISimulatorSvc
	{
		[OperationContract]
		SvcOutputGeneric<DeviceConfig> Initialize(SvcInputGeneric<int> readersCountInput);

		[OperationContract]
		SvcOutputGeneric<long> GetTimerOffset();

		[OperationContract]
		SvcOutputBase ReinstallDsf();

		[OperationContract]
		SvcOutputBase ExcludeDevices(List<DeviceIdentity> devices);

		//[OperationContract]
		//SvcOutputGeneric<ConveyorSettings> GetConveyorSettings();

		[OperationContract]
		SvcOutputBase Reset();

        [OperationContract]
        SvcOutputBase SetSimulatorSettings(SvcInputGeneric<SimulatorSettings> input);

        [OperationContract]
        SvcOutputBase StopTest();
    }
}