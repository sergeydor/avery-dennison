using Common.Domain.ExtendedTestSetupCommands;
using Common.Enums;

namespace Server.Device.Communication.CommandInterpretators
{
	public class ExtendedGetTestSetupCommandsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Get Punch Settings (0x40) command
		/// </summary>
		public CommandData GetPunchSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x40
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Punch Settings (0x40) response
		/// </summary>
		public PunchSettings ConvertResponseDataToPunchSettings(ResponseData responseData)
		{
			var data = responseData.DATA;
			var punchSettings = new PunchSettings
			{
				Enable = (PunchEnableMode)data[0],
				Position = data[1],
				Duration = data[2],
				Offset = data[3]
			};

			return punchSettings;
		}
	}
}