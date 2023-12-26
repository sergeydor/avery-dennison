using Common.Services.Output;

namespace Server.Device.Communication.RemoteServices.Dtos.Output
{
	public class CheckDevicesInstalledOutput : SvcOutputBase
	{
		public bool Installed { get; set; }

		public override string ToString()
		{
			return base.ToString() + "Installed " + Installed;
		}
	}
}