using Common.Enums.GSTCommands;

namespace Common.Domain.GSTCommands
{
	public class CalibrateSettings
	{
		public CalibrateMode Mode { get; set; }
		public ushort Pwr { get; set; }
	}
}