using Common.Domain.ExtendedTestSetupCommands;

namespace Server.Device.Communication.CommandInterpretators
{
	public class ExtendedTestSetupCommadsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Set Punch Settings (0x20) command
		/// </summary>
		public CommandData ConvertPunchSettingsToCommandData(PunchSettings settings)
		{
			var commandData = new CommandData
			{
				LEN = 0x4,
				CMD = 0x20,
				DATA = new byte[]
				{
					(byte)settings.Enable,
					settings.Position,
					settings.Duration,
					settings.Offset
				}
			};
			CalculateFcs(commandData);

			return commandData;
		}
	}
}