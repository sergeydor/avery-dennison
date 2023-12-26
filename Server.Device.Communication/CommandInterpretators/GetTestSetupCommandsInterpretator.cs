using System;
using System.Data;
using Common.Domain.DeviceResults;
using Common.Domain.TestSetupCommands;
using Common.Enums;

namespace Server.Device.Communication.CommandInterpretators
{
	public class GetTestSetupCommandsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Get Test Settings (0x30) command
		/// </summary>
		public CommandData GetTestSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x30
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Test Settings (0x30) response
		/// </summary>
		public TestSettings ConvertResponseDataToTestSettings(ResponseData responseData)
		{
			var data = responseData.DATA;

			var testSettings = new TestSettings
			{
				TestType = (Test10Type)data[0],
				TagClass = (TagClass)data[1],
				AntPort = (AntPort)data[2],
				ReadPower = BitConverter.ToInt16(Normalize(data, 3, 2), 0),
				ReadTimeout = BitConverter.ToUInt16(Normalize(data, 5, 2), 0),
				// todo: await for response from customer what Region parameter is
				Frequency1 = BitConverter.ToUInt32(Normalize(data, 7, 4), 0),
				Frequency2 = BitConverter.ToUInt32(Normalize(data, 11, 4), 0),
				Frequency3 = BitConverter.ToUInt32(Normalize(data, 15, 4), 0),
				WritePower = BitConverter.ToInt16(Normalize(data, 19, 2), 0),
				WriteTimeout = BitConverter.ToUInt16(Normalize(data, 21, 2), 0),
				WriteType = (WriteType)data[23],
				StartTagID = new byte[0xC]
			};
			Array.Copy(data, 24, testSettings.StartTagID, 0, testSettings.StartTagID.Length);
            testSettings.StartTagID = Normalize(testSettings.StartTagID, 0, testSettings.StartTagID.Length);

            return testSettings;
		}

		/// <summary>
		/// Get Marker Settings (0x31) command
		/// </summary>
		public CommandData GetMarkerSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x31
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Marker Settings (0x31) response
		/// </summary>
		public MarkerSettings ConvertResponseDataToMarkerSettings(ResponseData responseData)
		{
			var data = responseData.DATA;
			var markerSettings = new MarkerSettings
			{
				Enable = (MarkerEnableMode)data[0],
				Position = data[1],
				Duration = data[2],
				Offset = data[3]
			};

			return markerSettings;
		}

		/// <summary>
		/// Get Trigger Input Settings (0x32) command
		/// </summary>
		public CommandData GetTriggerInputSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x32
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Trigger Input Settings (0x32) response
		/// </summary>
		public TriggerInputSettings ConverResponseDataToTriggerInputSettings(ResponseData responseData)
		{
			var data = responseData.DATA;
			var triggerInputSettings = new TriggerInputSettings
			{
				Enable = (EnableMode)data[0],
				EdgeType = (EdgeType)data[1],
				Debounce = data[2],
				DeafTime = data[3],
				TestOffset = data[4]
			};

			return triggerInputSettings;
		}

		/// <summary>
		/// Get Tester Settings (0x34) command
		/// </summary>
		public CommandData GetTesterSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x34
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Tester Settings (0x34) response
		/// </summary>
		public TesterSettings ConvertResponseDataToTesterSettings(ResponseData responseData)
		{
			var testerSettings = new TesterSettings
			{
				Enable = (EnableMode)responseData.DATA[0],
				Position = responseData.DATA[1]
			};

			return testerSettings;
		}

		/// <summary>
		/// Get Test Statistics (0x35) command
		/// </summary>
		public CommandData GetTestStatisticsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x35
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Test Statistics (0x35) response
		/// </summary>
		public TestStatistics ConvertResponseDataToTestStatistics(ResponseData responseData)
		{
			byte[] data = responseData.DATA;

			var result = new TestStatistics
			{
				TestPassCount = BitConverter.ToUInt32(Normalize(data, 0, 4), 0),
				TestFailCount = BitConverter.ToUInt32(Normalize(data, 4, 4), 0),
				TriggerCount = BitConverter.ToUInt32(Normalize(data, 8, 4), 0)
			};

			return result;
		}

		/// <summary>
		/// Get Bulk Write Settings (0x36) command
		/// </summary>
		public CommandData GetBulkWriteSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x36
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Bulk Write Settings (0x36) response
		/// </summary>
		public BulkWriteSettingsResult ConvertResponseDataToBulkWriteSettingsResult(ResponseData responseData)
		{
			var data = responseData.DATA;
			var result = new BulkWriteSettingsResult
			{
				AntPort = (AntPort)data[0],
				WritePower = BitConverter.ToInt16(data, 1),
				WriteTimeout = BitConverter.ToUInt16(data, 3),
				Frequency = BitConverter.ToUInt32(data, 5),
				Count = data[9],
				WriteType = (WriteType)data[10],
				TagID = new byte[0xC]
			};
			Array.Copy(data, 11, result.TagID, 0, 0xC);
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get Antenna Settings (0x37) command
		/// </summary>
		public CommandData GetAntennaSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x37
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Antenna Settings (0x37) respose
		/// </summary>
		public AntennaSettings ConvertResponseDataToAntennaSettings(ResponseData responseData)
		{
			var antennaSettings = new AntennaSettings
			{
				AntPort = (AntPort)responseData.DATA[0]
			};

			return antennaSettings;
		}

		/// <summary>
		/// Get Sensitivity Test Settings (0x38) command
		/// </summary>
		public CommandData GetSensitivityTestSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x38
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Sensitivity Test Settings (0x38) response
		/// </summary>
		public SensitivityTestSettings ConvertResponseDataToSensitivityTestSettings(ResponseData responseData)
		{
			var data = responseData.DATA;

			var result = new SensitivityTestSettings
			{
				ReadWriteMode = (ReadWriteMode)data[0],
				AntPort = (AntPort)data[1],
				Frequency = BitConverter.ToUInt32(Normalize(data, 2, 4), 0),
				MinPower = BitConverter.ToInt16(Normalize(data, 6, 2), 0),
				MaxPower = BitConverter.ToInt16(Normalize(data, 8, 2), 0),
				SearchDepth = data[10],
				Timeout = BitConverter.ToUInt16(Normalize(data, 11, 2), 0),
				PassThreshold = BitConverter.ToInt16(Normalize(data, 13, 2), 0),
				Options = data[15]
			};

			return result;
		}

		/// <summary>
		/// Get TID Test Settings (0x39) command
		/// </summary>
		public CommandData GetTidTestSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x39
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get TID Test Settings (0x39) response
		/// </summary>
		public TIDTestSettings ConvertResponseDataToTIDTestSettings(ResponseData responseData)
		{
			var data = responseData.DATA;
			var tidTestSettings = new TIDTestSettings
			{
				Options = data[0],
				ReadTimeout = BitConverter.ToUInt16(Normalize(data, 1, 2), 0),
				Interval = BitConverter.ToUInt16(Normalize(data, 3, 2), 0),
				TID = BitConverter.ToUInt32(Normalize(data, 5, 4), 0)
			};

			return tidTestSettings;
		}

		/// <summary>
		/// Get Tag ID Filter Settings (0x3A) command
		/// </summary>
		public CommandData GetTagIdFilterSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x3A
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Tag ID Filter Settings (0x3A) response
		/// </summary>
		public TagIDFilterSettings ConvertResponseDataToTagIdFilterSettings(ResponseData responseData)
		{
			var data = responseData.DATA;
			var settings = new TagIDFilterSettings
			{
				Options = data[0],
				NibbleEnable = new byte[3],
				TagIDFilter = new byte[12]
			};
			Array.Copy(data, 1, settings.NibbleEnable, 0, 3);
			Array.Copy(data, 4, settings.TagIDFilter, 0, 12);

            settings.NibbleEnable = Normalize(settings.NibbleEnable, 0, settings.NibbleEnable.Length);
            settings.TagIDFilter = Normalize(settings.TagIDFilter, 0, settings.TagIDFilter.Length);

            return settings;
		}

		/// <summary>
		/// Get Aux In Settings (0x3C) command
		/// </summary>
		public CommandData GetAuxSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x3C
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Aux In Settings (0x3C) response
		/// </summary>
		public AuxSettings ConvertResponseDataToAuxSettings(ResponseData responseData)
		{
			var data = responseData.DATA;
			var auxSettings = new AuxSettings
			{
				Function = (AuxSettingsFunction)data[0],
				Option1 = data[1],
				Option2 = data[2],
				EdgeType = (EdgeType)data[3],
				Debounce = data[4],
				DeafTime = data[5]
			};

			return auxSettings;
		}

		/// <summary>
		/// Get Encoder In Settings (0x3D) command
		/// </summary>
		public CommandData GetEncoderSettingsCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x3D
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Encoder In Settings (0x3D) response
		/// </summary>
		public EncoderSettings ConvertReponseDataToEncoderSettings(ResponseData responseData)
		{
			var data = responseData.DATA;
			var encoderSettings = new EncoderSettings
			{
				TriggerFilterMin = BitConverter.ToUInt16(Normalize(data, 0, 2), 0),
				TesterOffset = BitConverter.ToUInt16(Normalize(data, 2, 2), 0),
				MarkerOffset = BitConverter.ToUInt16(Normalize(data, 4, 2), 0),
				PunchOffset = BitConverter.ToUInt16(Normalize(data, 6, 2), 0),
				PunchFlight = BitConverter.ToUInt16(Normalize(data, 8, 2), 0),
				TriggerFilterMax = BitConverter.ToUInt16(Normalize(data, 10, 2), 0)
			};

			return encoderSettings;
		}
	}
}