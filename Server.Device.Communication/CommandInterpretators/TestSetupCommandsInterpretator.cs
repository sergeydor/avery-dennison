using System;
using System.Linq;
using Common.Domain.TestSetupCommands;

namespace Server.Device.Communication.CommandInterpretators
{
	public class TestSetupCommandsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Set Test Settings (0x10) command
		/// </summary>
		public CommandData ConvertTestSettingsToCommandData(TestSettings testSettings)
		{
			byte length = 0x24;
			var data = new byte[length];

			var readPowerBytes = BitConverter.GetBytes(testSettings.ReadPower);
			var readTimeoutBytes = BitConverter.GetBytes(testSettings.ReadTimeout);
			var frequency1Bytes = BitConverter.GetBytes(testSettings.Frequency1);
			var frequency2Bytes = BitConverter.GetBytes(testSettings.Frequency2);
			var frequency3Bytes = BitConverter.GetBytes(testSettings.Frequency3);
			var writePowerBytes = BitConverter.GetBytes(testSettings.WritePower);
			var writeTimeoutBytes = BitConverter.GetBytes(testSettings.WriteTimeout);

			Array.Reverse(readPowerBytes);
			Array.Reverse(readTimeoutBytes);
			Array.Reverse(frequency1Bytes);
			Array.Reverse(frequency2Bytes);
			Array.Reverse(frequency3Bytes);
			Array.Reverse(writePowerBytes);
			Array.Reverse(writeTimeoutBytes);

			data[0] = (byte)testSettings.TestType;
			data[1] = (byte)testSettings.TagClass;
			data[2] = (byte)testSettings.AntPort;
			readPowerBytes.CopyTo(data, 3);
			readTimeoutBytes.CopyTo(data, 5);
			frequency1Bytes.CopyTo(data, 7);
			frequency2Bytes.CopyTo(data, 11);
			frequency3Bytes.CopyTo(data, 15);
			writePowerBytes.CopyTo(data, 19);
			writeTimeoutBytes.CopyTo(data, 21);
			data[23] = (byte)testSettings.WriteType;
            Normalize(testSettings.StartTagID, 0, testSettings.StartTagID.Length).CopyTo(data, 24);

			var commandData = new CommandData
			{
				LEN = length,
				CMD = 0x10,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Marker Settings (0x11) command
		/// </summary>
		public CommandData ConvertMarkerSettingsToCommandData(MarkerSettings markerSettings)
		{
			var commandData = new CommandData
			{
				LEN = 0x4,
				CMD = 0x11,
				DATA = new[]
				{
					(byte)markerSettings.Enable,
					markerSettings.Position,
					markerSettings.Duration,
					markerSettings.Offset
				}
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Trigger Input Settings (0x12) command
		/// </summary>
		public CommandData ConvertTriggerInputSettingsToCommandData(TriggerInputSettings settings)
		{
			var commandData = new CommandData
			{
				LEN = 0x5,
				CMD = 0x12,
				DATA = new byte[]
				{
					(byte)settings.Enable,
					(byte)settings.EdgeType,
					settings.Debounce,
					settings.DeafTime,
					settings.TestOffset
				}
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Tester Settings (0x14) command
		/// </summary>
		public CommandData ConvertTesterSettingsToCommandData(TesterSettings testerSettings)
		{
			var commandData = new CommandData
			{
				LEN = 0x2,
				CMD = 0x14,
				DATA = new byte[] { (byte)testerSettings.Enable, testerSettings.Position }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Test Statistics (0x15) command
		/// </summary>
		public CommandData ConvertTestStatisticsToCommandData(TestStatistics testStatistics)
		{
			byte length = 0x0C;
			var data = new byte[length];

			var testPassCountBytes = BitConverter.GetBytes(testStatistics.TestPassCount);
			var testFailCountBytes = BitConverter.GetBytes(testStatistics.TestFailCount);
			var triggerCountBytes = BitConverter.GetBytes(testStatistics.TriggerCount);

			Array.Reverse(testPassCountBytes);
			Array.Reverse(testFailCountBytes);
			Array.Reverse(triggerCountBytes);

			testPassCountBytes.CopyTo(data, 0);
			testFailCountBytes.CopyTo(data, 4);
			triggerCountBytes.CopyTo(data, 8);

			var commandData = new CommandData
			{
				LEN = length,
				CMD = 0x15,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Antenna Settings (0x17) command
		/// </summary>
		public CommandData GetAntennaSettingsCommandData(AntennaSettings antennaSettings)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = 0x17,
				DATA = new byte[] { (byte)antennaSettings.AntPort }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Sensitivity Test Settings (0x18) command
		/// </summary>
		public CommandData ConvertSensitivityTestSettingsToCommandData(SensitivityTestSettings testSettings)
		{
			var frequencyBytes = BitConverter.GetBytes(testSettings.Frequency);
			var minPowerBytes = BitConverter.GetBytes(testSettings.MinPower);
			var maxPowerBytes = BitConverter.GetBytes(testSettings.MaxPower);
			var timeoutBytes = BitConverter.GetBytes(testSettings.Timeout);
			var passThresholdBytes = BitConverter.GetBytes(testSettings.PassThreshold);

			Array.Reverse(frequencyBytes);
			Array.Reverse(minPowerBytes);
			Array.Reverse(maxPowerBytes);
			Array.Reverse(timeoutBytes);
			Array.Reverse(passThresholdBytes);

			byte length = 0x10;
			var data = new byte[length];

			data[0] = (byte)testSettings.ReadWriteMode;
			data[1] = (byte)testSettings.AntPort;
			frequencyBytes.CopyTo(data, 2);
			minPowerBytes.CopyTo(data, 6);
			maxPowerBytes.CopyTo(data, 8);
			data[10] = testSettings.SearchDepth;
			timeoutBytes.CopyTo(data, 11);
			passThresholdBytes.CopyTo(data, 13);
			data[15] = (byte)testSettings.Options;

			var commandData = new CommandData
			{
				LEN = length,
				CMD = 0x18,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set TID Test Settings (0x19) command
		/// </summary>
		public CommandData ConvertTIDTestSettingsToCommandData(TIDTestSettings settings)
		{
			byte length = 0x9;
			var data = new byte[length];

			data[0] = settings.Options;
			byte[] readTimeBytes = BitConverter.GetBytes(settings.ReadTimeout);
			Array.Reverse(readTimeBytes);
			readTimeBytes.CopyTo(data, 1);
			byte[] intervalBytes = BitConverter.GetBytes(settings.Interval);
			Array.Reverse(intervalBytes);
			intervalBytes.CopyTo(data, 3);
			byte[] tidBytes = BitConverter.GetBytes(settings.TID);
			Array.Reverse(tidBytes);
			tidBytes.CopyTo(data, 5);

			var commandData = new CommandData
			{
				LEN = length,
				CMD = 0x19,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Tag ID Filter Settings (0x1A) command
		/// </summary>
		public CommandData ConvertTagIDFilterSettingsToCommandData(TagIDFilterSettings settings)
		{
			byte length = 0x10;
			var data = new byte[length];

			data[0] = settings.Options;
			Normalize(settings.NibbleEnable, 0, settings.NibbleEnable.Length).CopyTo(data, 1);
            Normalize(settings.TagIDFilter, 0, settings.TagIDFilter.Length).CopyTo(data, 4);

			var commandData = new CommandData
			{
				LEN = length,
				CMD = 0x1A,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Aux In Settings (0x1C) command
		/// </summary>
		public CommandData ConvertAuxSettingsToCommandData(AuxSettings settings)
		{
			var data = new byte[0x6];
			data[0] = (byte)settings.Function;
			data[1] = settings.Option1;
			data[2] = settings.Option2;
			data[3] = (byte)settings.EdgeType;
			data[4] = settings.Debounce;
			data[5] = settings.DeafTime;
			var commandData = new CommandData
			{
				LEN = 0x6,
				CMD = 0x1C,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Encoder In Settings (0x1D) command
		/// </summary>
		public CommandData ConvertEncoderSettingsToCommandData(EncoderSettings settings)
		{
			var data = new byte[0xC];

			var trigFilterMinBytes = BitConverter.GetBytes(settings.TriggerFilterMin);
			var testerOffsetBytes = BitConverter.GetBytes(settings.TesterOffset);
			var markerOffsetBytes = BitConverter.GetBytes(settings.MarkerOffset);
			var punchOffsetBytes = BitConverter.GetBytes(settings.PunchOffset);
			var punchFlightBytes = BitConverter.GetBytes(settings.PunchFlight);
			var trigFilterMaxBytes = BitConverter.GetBytes(settings.TriggerFilterMax);

			Array.Reverse(trigFilterMinBytes);
			Array.Reverse(testerOffsetBytes);
			Array.Reverse(markerOffsetBytes);
			Array.Reverse(punchOffsetBytes);
			Array.Reverse(punchFlightBytes);
			Array.Reverse(trigFilterMaxBytes);

			trigFilterMinBytes.CopyTo(data, 0);
			testerOffsetBytes.CopyTo(data, 2);
			markerOffsetBytes.CopyTo(data, 4);
			punchOffsetBytes.CopyTo(data, 6);
			punchFlightBytes.CopyTo(data, 8);
			trigFilterMaxBytes.CopyTo(data, 10);

			var commandData = new CommandData
			{
				LEN = 0x0C,
				CMD = 0x1D,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Auto Marker Settings (1E) command
		/// </summary>
		public CommandData ConvertAutoMarkerSettingsToCommandData(AutoMarkerSettings settings)
		{
			var data = new byte[0x6];
			var pulsesNumberBytes = BitConverter.GetBytes(settings.PulsesNumber);
			var pulseLength = BitConverter.GetBytes(settings.PulseLength);

			pulsesNumberBytes.CopyTo(data, 0);
			pulseLength.CopyTo(data, 4);

			var commandData = new CommandData
			{
				LEN = 0x6,
				CMD = 0x1E,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}


		/// <summary>
		/// Set Auto Test Settings (0x1F) command
		/// </summary>
		public CommandData ConvertAutoTestSettingsToCommandData(AutoTestSettings autoTestSettings)
		{
			var testsNumberBytes = BitConverter.GetBytes(autoTestSettings.TestsNumber);
			var testIntervalBytes = BitConverter.GetBytes(autoTestSettings.TestInterval);
			var triggerIntervalBytes = BitConverter.GetBytes(autoTestSettings.TriggerInterval);

			byte length = 0x06;
			var data = new byte[length];

			testsNumberBytes.CopyTo(data, 0);
			testIntervalBytes.CopyTo(data, 2);
			triggerIntervalBytes.CopyTo(data, 4);

			var commandData = new CommandData
			{
				LEN = length,
				CMD = 0x1F,
				DATA = data
			};
			CalculateFcs(commandData);

			return commandData;
		}
	}
}