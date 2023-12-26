using System;
using Common.Domain.DeviceResults.TestActionCommands;
using Common.Enums;
using Common.Enums.TestActionCommands;

namespace Server.Device.Communication.CommandInterpretators
{
	public class TestActionCommandsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Start Testing (0x50) command
		/// </summary>
		public CommandData GetStartTestingCommandData(TestMode testMode)
		{
			var data = new byte[0x01];
			data[0] = (byte)testMode;
			var commandData = new CommandData
			{
				LEN = 0x01,
				CMD = 0x50,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Stop Testing (0x51) command
		/// </summary>
		public CommandData GetStopTestingCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x51
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Test (0x52) command
		/// </summary>
		public CommandData GetTest52CommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x52
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Sensitivity Trigger Input (0x54) command
		/// </summary>
		public CommandData GetSensitivityTriggerInputCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x54
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Sensitivity Trigger Input (0x54) response
		/// </summary>
		public SensitivityTriggerInputResult ConvertResponseDataToSensitivityTriggerInputResult(ResponseData responseData)
		{
			var result = new SensitivityTriggerInputResult();
			CreateGeneralDeviceResult(result, responseData);
			result.TriggerCount = BitConverter.ToUInt32(responseData.DATA, 0);
			result.EncoderCount = BitConverter.ToUInt16(responseData.DATA, 4);

			return result;
		}

		/// <summary>
		/// Punch (0x56) command
		/// </summary>
		public CommandData GetPunchCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x56
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Hardware Version (0x62) command
		/// </summary>
		public CommandData GetHardwareVersion()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x62
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Hardware Version (0x62) response
		/// </summary>
		public HardwareVersionResult ConvertResponseDataToHardwareVersionResult(ResponseData responseData)
		{
			var data = responseData.DATA;
			var result = new HardwareVersionResult
			{
				CheckByte1 = data[0],
				CheckByte2 = data[1],
				BoardType = (BoardType)data[2],
				BoardRevision = data[3],
				ID4 = data[4],
				ID3 = data[5],
				ID2 = data[6],
				ID1 = data[7]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Set Fault (0x63) command
		/// </summary>
		public CommandData GetSetFaultCommandData(FaultCode faultCode)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = 0x63,
				DATA = new[] {(byte)faultCode}
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Digital IO (0x68) command
		/// </summary>
		public CommandData GetSetDigitalIOCommandData(InputDigitalValue ioValue)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = 0x68,
				DATA = new[] {(byte)ioValue}
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Digital IO (0x69) command
		/// </summary>
		public CommandData GetDigitalIOCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x69
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Digital IO (0x69) response
		/// </summary>
		public DigitalIOResult ConvertResponseDataToDigitalIOResult(ResponseData responseData)
		{
			var result = new DigitalIOResult
			{
				IOValue = (OutputDigitalValue)responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}
	}
}