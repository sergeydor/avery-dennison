using Common.Domain.TestSetupCommands;
using Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class TestSetupCommandsTests
	{
		private readonly TestSetupCommandsInterpretator _interpretator = new TestSetupCommandsInterpretator();

		/// <summary>
		/// Set Test Settings (0x10) command
		/// </summary>
		[TestMethod]
		public void TestConvertTestSettingsToCommandData()
		{
			var test = unchecked ((byte) 0x52C);
			var expectation = new CommandExpectation(0xEE, 0x24, 0x10,
				new byte[0x24]
				{
					0x98, 0x3, 0x1, 0xFF, 0xCE, 0x0, 0x54, 0x0, 0x0, 0x4, 0xB0, 0x0, 0x0, 0x5, 0x14, 0x0, 0x0, 0x05, 0x78, 0xFC, 0x18,
					0x0, 0xF, 0x2, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C
				}, 0xAE);
			
			var testSettings = new TestSettings
			{
				TestType = Test10Type.IDFilter | Test10Type.TIDTest | Test10Type.ReadTest,
				TagClass = TagClass.ImpinjM5,
				AntPort = AntPort.Tx1Rx1,
				ReadPower = -50,
				ReadTimeout = 0x54,
				Frequency1 = 1200,
				Frequency2 = 1300,
				Frequency3 = 1400,
				WritePower = -1000,
				WriteTimeout = 15,
				WriteType = WriteType.UseDateTimeValues,
				StartTagID = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C }
 			};
			var commandData = _interpretator.ConvertTestSettingsToCommandData(testSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Marker Settings (0x11) command
		/// </summary>
		[TestMethod]
		public void TestConvertMarkerSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x4, 0x11, new byte[]{0x2, 0x4, 0x9, 0x12}, 0x36);
			var markerSettings = new MarkerSettings
			{
				Enable = MarkerEnableMode.MarkGoodLabels,
				Position = 0x4,
				Duration = 0x9,
				Offset = 0x12
			};
			var commandData = _interpretator.ConvertMarkerSettingsToCommandData(markerSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Trigger Input Settings (0x12) command
		/// </summary>
		[TestMethod]
		public void TestConvertTriggerInputSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x5, 0x12, new byte[] {0x1, 0x1, 0x4, 0x2, 0x7}, 0x26);

			var settings = new TriggerInputSettings
			{
				Enable = EnableMode.Enable,
				EdgeType = EdgeType.Rising,
				Debounce = 0x4,
				DeafTime = 0x2,
				TestOffset = 0x7
			};
			var commandData = _interpretator.ConvertTriggerInputSettingsToCommandData(settings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Tester Settings (0x14) command
		/// </summary>
		[TestMethod]
		public void TestConvertTesterSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x2, 0x14, new byte[] {0x1, 0x63}, 0x7A);
			
			var testerSettings = new TesterSettings
			{
				Enable = EnableMode.Enable,
				Position = 0x63
			};
			var commandData = _interpretator.ConvertTesterSettingsToCommandData(testerSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Test Statistics (0x15) command
		/// </summary>
		[TestMethod]
		public void TestConvertTestStatisticsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0C, 0x15, new byte[]{0x0, 0x0, 0x0, 0x8, 0x0, 0x0, 0x0, 0x10, 0x0, 0x0, 0x0, 0x60}, 0x99);

			var testStatistics = new TestStatistics
			{
				TestPassCount = 0x8,
				TestFailCount = 0x10,
				TriggerCount = 0x60
			};
			var commandData = _interpretator.ConvertTestStatisticsToCommandData(testStatistics);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Antenna Settings (0x17) command
		/// </summary>
		[TestMethod]
		public void TestGetAntennaSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x1, 0x17, new byte[] {0x3}, 0x1B);
			var commandData = _interpretator.GetAntennaSettingsCommandData(new AntennaSettings { AntPort = AntPort.Tx1Rx2 });

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Sensitivity Test Settings (0x18) command
		/// </summary>
		[TestMethod]
		public void TestConvertSensitivityTestSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x10, 0x18, new byte[] {0x1, 0x1, 0x0, 0x0, 0x0, 0xF, 0xFF, 0xF6, 0xFF, 0xFB, 0x5, 0x0, 0x6F, 0xFF, 0xEC, 0x0}, 0x87);

            var sensitivityTestSettings = new SensitivityTestSettings
            {
                ReadWriteMode = ReadWriteMode.Write,
                AntPort = AntPort.Tx1Rx1,
                Frequency = 15,
                MinPower = -10,
                MaxPower = -5,
                SearchDepth = 0x5,
                Timeout = 111,
                PassThreshold = -20,
                Options = 0// SensitivityTestOptions.RFU
			};
			var commandData = _interpretator.ConvertSensitivityTestSettingsToCommandData(sensitivityTestSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Tag ID Filter Settings (0x1A) command
		/// </summary>
		[TestMethod]
		public void TestConvertTagIDFilterSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x10, 0x1A, new byte[0x10] {0x6, 0x1, 0x4, 0x6, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C }, 0x89);

			var tagIdFilterSettings = new TagIDFilterSettings
			{
				Options = 0x6,
				NibbleEnable = new byte[] { 0x1, 0x4, 0x6 },
				TagIDFilter = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C }
			};
			var commandData = _interpretator.ConvertTagIDFilterSettingsToCommandData(tagIdFilterSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Aux In Settings (0x1C) command
		/// </summary>
		[TestMethod]
		public void TestConvertAuxSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x6, 0x1C, new byte[] { 0x02, 0x7, 0x1B, 0x0, 0x1, 0x1 }, 0x48);

			var auxSettings = new AuxSettings
			{
				Function = AuxSettingsFunction.PulseChannel,
				Option1 = 0x7,
				Option2 = 0x1B,
				EdgeType = EdgeType.Falling,
				Debounce = 0x1,
				DeafTime = 0x1
			};
			var commandData = _interpretator.ConvertAuxSettingsToCommandData(auxSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Encoder In Settings (0x1D) command
		/// </summary>
		[TestMethod]
		public void TestConvertEncoderSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0A, 0x1D, new byte[] { 0x0, 0x3, 0x0, 0x4, 0x0, 0x3, 0x0, 0x3, 0x0, 0x0A, 0x0, 0x1 }, 0x3F);

			var encoderSettings = new EncoderSettings
			{
				TriggerFilterMin = 0x3,
				TesterOffset = 0x4,
				MarkerOffset = 0x3,
				PunchOffset = 0x3,
				PunchFlight = 10,
				TriggerFilterMax = 0x1
			};
			var commandData = _interpretator.ConvertEncoderSettingsToCommandData(encoderSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Auto Marker Settings (1E) command
		/// </summary>
		[TestMethod]
		public void TestConvertAutoMarkerSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x6, 0x1E, new byte[]{0x34, 0x0, 0x0, 0x0, 0x5, 0x0}, 0x5D);
			
			var settings = new AutoMarkerSettings
			{
				PulsesNumber = 0x34,
				PulseLength = 0x5
			};
			var commandData = _interpretator.ConvertAutoMarkerSettingsToCommandData(settings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Auto Test Settings (0x1F) command
		/// </summary>
		[TestMethod]
		public void TestConvertAutoTestSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x6, 0x1F, new byte[] {0x16, 0x0, 0x3A, 0x0, 0x20, 0x0}, 0x95);

			var autoTestSettings = new AutoTestSettings
			{
				TestsNumber = 0x16,
				TestInterval = 0x3A,
				TriggerInterval = 0x20
			};
			var commandData = _interpretator.ConvertAutoTestSettingsToCommandData(autoTestSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}
	}
}