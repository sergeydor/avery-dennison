using System;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Domain.TestModuleCommands;
using Common.Enums;

namespace Server.Device.Communication.CommandInterpretators
{
	public class TestModuleCommandsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Ping (0x00) command
		/// </summary>
		public CommandData GetPingCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x0
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Version (0x01) command
		/// </summary>
		public CommandData GetVersionCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x1
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Version (0x01) response
		/// </summary>
		public VersionResult ConvertResponseDataToVersionResult(ResponseData responseData)
		{
			var data = responseData.DATA;
			var result = new VersionResult
			{
				Major = data[0],
				Minor = data[1],
				Year = data[2],
				Month = data[3],
				Day = data[4]
			};

			return result;
		}

		/// <summary>
		/// Reset (0x02) command
		/// </summary>
		public CommandData ConvertResetSettingsToCommandData(ResetSettings settings)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = 0x2,
				DATA = new []{(byte)settings.Type}
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Reset (0x02) response
		/// </summary>
		public ResetResult ConvertResponseDataToResetResult(ResponseData responseData)
		{
			var result = new ResetResult {Type = (OutputResetType)responseData.DATA[0]};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get Last Fault (0x03) command
		/// </summary>
		public CommandData GetLastFaultCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x3
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Last Fault (0x03) response
		/// </summary>
		public LastFaultResult ConvertResponseDataToLastFaultResult(ResponseData responseData)
		{
			var result = new LastFaultResult();
			result.FaultCode = (FaultCode)responseData.DATA[0];

			return result;
		}

		/// <summary>
		/// Get Status (0x04) command
		/// </summary>
		public CommandData GetStatusCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x04
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Status (0x04) response
		/// </summary>
		public GetStatusResult ConvertResponseDataToGetStatusResult(ResponseData responseData)
		{
			var timer2 = BitConverter.ToUInt16(responseData.DATA, 0);

			var result = new GetStatusResult();
			CreateGeneralDeviceResult(result, responseData);

			result.Timer2 = timer2;
			result.Temperature = responseData.DATA[2];
			result.CalibrationStatus = (CalibrationStatus)responseData.DATA[3];
			result.OperatingState = responseData.DATA[4];

			return result;
		}

		/// <summary>
		/// Clear Last Fault (0x06) command
		/// </summary>
		public CommandData GetClearLastFaultCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x6
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Date Time (0x08) command
		/// </summary>
		public CommandData ConvertLaneDateTimeToCommandData(LaneDateTime laneDateTime)
		{
			var commandData = new CommandData
			{
				LEN = 0x7,
				CMD = 0x8,
				DATA = new byte[0x7]
				{
					laneDateTime.Lane,
					laneDateTime.Year,
					laneDateTime.Month,
					laneDateTime.Day,
					laneDateTime.Hour,
					laneDateTime.Minute,
					laneDateTime.Second
				}
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Date Time (0x09) command
		/// </summary>
		public CommandData GetDateTimeCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x9
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Date Time (0x09) response
		/// </summary>
		public LaneDateTime ConvertResponseDataToLaneDateTime(ResponseData responseData)
		{
			var data = responseData.DATA;
			var laneDateTime = new LaneDateTime
			{
				Lane = data[0],
				Year = data[1],
				Month = data[2],
				Day = data[3],
				Hour = data[4],
				Minute = data[5],
				Second = data[6]
			};

			return laneDateTime;
		}

		/// <summary>
		/// Test RF Settings (0x0B) command
		/// </summary>
		public CommandData ConvertTestRfSettingsToCommandData(TestRFSettings testRfSettings)
		{
			var requestedFrequencyBytes = BitConverter.GetBytes(testRfSettings.RequestedFrequency);
			var requestedPowerBytes = BitConverter.GetBytes(testRfSettings.RequestedPower);

			byte length = 0x6;
			var data = new byte[length];

			requestedFrequencyBytes.CopyTo(data, 0);
			requestedPowerBytes.CopyTo(data, 4);

			var commandData = new CommandData
			{
				LEN = length,
				CMD = 0x0B,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Test RF Settings (0x0B) response
		/// </summary>
		public TestRFSettingsResult ConvertResponseDataToTestRFSettingsResult(ResponseData responseData)
		{
			var data = responseData.DATA;
			var result = new TestRFSettingsResult();
			CreateGeneralDeviceResult(result, responseData);
			result.Frequency = BitConverter.ToUInt32(data, 0);
			result.PowerError = BitConverter.ToInt16(data, 4);
			result.Temp = (RFTemperature)data[6];
			result.CTFAD = BitConverter.ToUInt16(data, 7);
			result.CTRAD = BitConverter.ToUInt16(data, 9);

			return result;
		}

		/// <summary>
		/// Restart Reader To Bootloader (0x0E) command
		/// </summary>
		public CommandData GetRestartReaderCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x0E
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Last Message (0x0F) command
		/// </summary>
		public CommandData GetLastMessageCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x0F
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Last Message (0x0F) response
		/// </summary>
		public LastMessageResult ConvertResponseDataToLastMessageResult(ResponseData responseData)
		{
			var result = new LastMessageResult { MessageId = responseData.DATA[0] };
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}
	}
}