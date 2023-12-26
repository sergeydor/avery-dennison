using Common.Enums;

namespace Common.Domain.DeviceResults.TestModuleCommands
{
	public class ResetResult : GeneralDeviceResult
	{
		public OutputResetType Type { get; set; }
	}
}