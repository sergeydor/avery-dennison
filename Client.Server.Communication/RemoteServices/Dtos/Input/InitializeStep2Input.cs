using System.Collections.Generic;
using System.Runtime.Serialization;
using Common.Domain.Device;
using Common.Enums;
using Common.Services.Input;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{
	[DataContract]
	public class InitializeStep2Input : SvcInputBase
	{
		[DataMember]
		public string AppSessionName { get; set; }

		[DataMember]
		public List<DeviceIdentity> Devices { get; set; }

		[DataMember]
		public AppMode AppMode { get; set; }
	}
}