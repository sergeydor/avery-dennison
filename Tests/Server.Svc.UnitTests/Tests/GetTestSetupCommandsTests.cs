using Common.Domain.DeviceResults;
using Common.Domain.TestSetupCommands;
using Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class GetTestSetupCommandsTests
	{
		private readonly GetTestSetupCommandsInterpretator _interpretator = new GetTestSetupCommandsInterpretator();

		/// <summary>
		/// Get Test Settings (0x30) command
		/// </summary>
		[TestMethod]
		public void TestGetTestSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x30, new byte[0], 0x30);
			var commandData = _interpretator.GetTestSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Test Settings (0x30) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTestSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x3
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedTestSettings = new TestSettings
			{
				TestType = Test10Type.IDFilter | Test10Type.TIDTest | Test10Type.ReadTest,
				TagClass = TagClass.ImpinjM5,
				AntPort = AntPort.Tx1Rx1,
				ReadPower = -50,
				ReadTimeout = 84,
				Frequency1 = 1200,
				Frequency2 = 1300,
				Frequency3 = 1400,
				WritePower = -1000,
				WriteTimeout = 15,
				WriteType = WriteType.UseDateTimeValues,
				StartTagID = new byte[] {0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C}
			};
			var expectation = new ResponseExpectation(expectedTestSettings);

			var responseData = new ResponseData
			{
				LEN = 0x06,
				CMD = 0x30,
				STATUS = 0x2C,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[0x24]
				{
					0x98, 0x3, 0x1, 0xFF, 0xCE, 0x0, 0x54, 0x0, 0x0, 0x4, 0xB0, 0x0, 0x0, 0x5, 0x14, 0x0, 0x0, 0x05, 0x78, 0xFC, 0x18,
					0x0, 0xF, 0x2, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C
				}
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var testSettings = _interpretator.ConvertResponseDataToTestSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(testSettings));
		}

		/// <summary>
		/// Get Marker Settings (0x31) command
		/// </summary>
		[TestMethod]
		public void TestGetMarkerSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x31, new byte[0], 0x31);
			var commandData = _interpretator.GetMarkerSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Marker Settings (0x31) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToMarkerSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x4
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedMarkerSettings = new MarkerSettings
			{
				Enable = MarkerEnableMode.MarkBadLabels,
				Position = 0x5,
				Duration = 0x6,
				Offset = 0x7
			};
			var expectation = new ResponseExpectation(expectedMarkerSettings);

			var responseData = new ResponseData
			{
				LEN = 0x04,
				CMD = 0x31,
				STATUS = 0x2C,
				TMR_HI = 0x4,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1, 0x5, 0x6, 0x7 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var markerSettings = _interpretator.ConvertResponseDataToMarkerSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(markerSettings));
		}

		/// <summary>
		/// Get Marker Settings (0x32) command
		/// </summary>
		[TestMethod]
		public void TestGetTriggerInputSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x32, new byte[0], 0x32);
			var commandData = _interpretator.GetTriggerInputSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Marker Settings (0x32) response
		/// </summary>
		[TestMethod]
		public void TestConverResponseDataToTriggerInputSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.ANTENNA_1_FAULT,
				Timer = 0x2
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedTriggerInputSettings = new TriggerInputSettings
			{
				Enable = EnableMode.Enable,
				EdgeType = EdgeType.Rising,
				Debounce = 0x5,
				DeafTime = 0x1,
				TestOffset = 0x2
			};
			var expectation = new ResponseExpectation(expectedTriggerInputSettings);

			var responseData = new ResponseData
			{
				LEN = 0x05,
				CMD = 0x32,
				STATUS = 0x37,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1, 0x1, 0x5, 0x1, 0x2 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var triggerInputSettings = _interpretator.ConverResponseDataToTriggerInputSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(triggerInputSettings));
		}

		/// <summary>
		/// Get Tester Settings (0x34) command
		/// </summary>
		[TestMethod]
		public void TestGetTesterSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x34, new byte[0], 0x34);
			var commandData = _interpretator.GetTesterSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Tester Settings (0x34) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTesterSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x3
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedTesterSettings = new TesterSettings
			{
				Enable = EnableMode.Enable,
				Position = 0x6
			};
			var expectation = new ResponseExpectation(expectedTesterSettings);

			var responseData = new ResponseData
			{
				LEN = 0x2,
				CMD = 0x34,
				STATUS = 0x2C,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1, 0x6 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var testerSettings = _interpretator.ConvertResponseDataToTesterSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(testerSettings));
		}

		/// <summary>
		/// Get Test Statistics (0x35) command
		/// </summary>
		[TestMethod]
		public void TestGetTestStatisticsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x35, new byte[0], 0x35);
			var commandData = _interpretator.GetTestStatisticsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Test Statistics (0x35) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTestStatistics()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x3
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedTestStatistics = new TestStatistics
			{
				TestPassCount = 0x8,
				TestFailCount = 16,
				TriggerCount = 96
			};
			var expectation = new ResponseExpectation(expectedTestStatistics);

			var responseData = new ResponseData
			{
				LEN = 0x06,
				CMD = 0x35,
				STATUS = 0x2C,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x0, 0x0, 0x0, 0x8, 0x0, 0x0, 0x0, 0x10, 0x0, 0x0, 0x0, 0x60 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var testStatistics = _interpretator.ConvertResponseDataToTestStatistics(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(testStatistics));
		}

		/// <summary>
		/// Get Bulk Write Settings (0x36) command
		/// </summary>
		[TestMethod]
		public void TestGetBulkWriteSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x36, new byte[0], 0x36);
			var commandData = _interpretator.GetBulkWriteSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Bulk Write Settings (0x36) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToBulkWriteSettingsResult()
		{
			var expectedResult = new BulkWriteSettingsResult
			{
				Status = StatusCode.CMD_UNKNOWN,
				Timer = 0x5,
				AntPort = AntPort.Tx1Rx1,
				WritePower = 0x2,
				WriteTimeout = 0x4,
				Frequency = 0x6,
				Count = 0x2,
				WriteType = WriteType.StrapData,
				TagID = new byte[0xC] {0x1, 0x2, 0x2, 0x4, 0x8, 0x8, 0x8, 0x9, 0xA, 0xC, 0x1, 0x2}
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x17,
				CMD = 0x36,
				STATUS = 0x3,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1, 0x2, 0x0, 0x4, 0x0, 0x6, 0x0, 0x0, 0x0, 0x2, 0x3, 0x1, 0x2, 0x2, 0x4, 0x8, 0x8, 0x8, 0x9, 0xA, 0xC, 0x1, 0x2 }
			};
			var actualtResult = _interpretator.ConvertResponseDataToBulkWriteSettingsResult(responseData);

			Assert.IsTrue(expectation.Equals(actualtResult));
		}

		/// <summary>
		/// Get Antenna Settings (0x37) command
		/// </summary>
		[TestMethod]
		public void TestGetAntennaSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x37, new byte[0], 0x37);
			var commandData = _interpretator.GetAntennaSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Antenna Settings (0x37) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToAntennaSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.ANTENNA_2_FAULT,
				Timer = 0x1
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedAntennaSettings = new AntennaSettings
			{
				AntPort = AntPort.Tx2Rx2
			};
			var expectation = new ResponseExpectation(expectedAntennaSettings);

			var responseData = new ResponseData
			{
				LEN = 0x01,
				CMD = 0x37,
				STATUS = 0x38,
				TMR_HI = 0x1,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x2 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var antennaSettings = _interpretator.ConvertResponseDataToAntennaSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(antennaSettings));
		}

		/// <summary>
		/// Get Sensitivity Test Settings (0x38) command
		/// </summary>
		[TestMethod]
		public void TestGetSensitivityTestSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x38, new byte[0], 0x38);
			var commandData = _interpretator.GetSensitivityTestSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Sensitivity Test Settings (0x38) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToSensitivityTestSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.INVALID_CAL_SETTING,
				Timer = 0x6
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedSensitivityTestSettings = new SensitivityTestSettings
			{
				ReadWriteMode = ReadWriteMode.Write,
				AntPort = AntPort.Tx1Rx1,
				Frequency = 15,
				MinPower = -10,
				MaxPower = -5,
				SearchDepth = 0x5,
				Timeout = 111,
				PassThreshold = -20,
				Options = 0 //SensitivityTestOptions.RFU
			};
			var expectation = new ResponseExpectation(expectedSensitivityTestSettings);

			var responseData = new ResponseData
			{
				LEN = 0x06,
				CMD = 0x38,
				STATUS = 0x3F,
				TMR_HI = 0x6,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1, 0x1, 0x0, 0x0, 0x0, 0xF, 0xFF, 0xF6, 0xFF, 0xFB, 0x5, 0x0, 0x6F, 0xFF, 0xEC, 0x0 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var sensitivityTestSettings = _interpretator.ConvertResponseDataToSensitivityTestSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(sensitivityTestSettings));
		}

		/// <summary>
		/// Get TID Test Settings (0x39) command
		/// </summary>
		[TestMethod]
		public void TestGetTidTestSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x39, new byte[0], 0x39);
			var commandData = _interpretator.GetTidTestSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get TID Test Settings (0x39) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTIDTestSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x3
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedTidTestSettings = new TIDTestSettings
			{
				Options = 0x3,
				ReadTimeout = 0x1,
				Interval = 0x6,
				TID = 18
			};
			var expectation = new ResponseExpectation(expectedTidTestSettings);

			var responseData = new ResponseData
			{
				LEN = 0x9,
				CMD = 0x95,
				STATUS = 0x2C,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x3, 0x0, 0x1, 0x0, 0x6, 0x0, 0x0, 0x0, 0x12 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var tidTestSettings = _interpretator.ConvertResponseDataToTIDTestSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(tidTestSettings));
		}

		/// <summary>
		/// Get Tag ID Filter Settings (0x3A) command
		/// </summary>
		[TestMethod]
		public void TestGetTagIdFilterSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x3A, new byte[0], 0x3A);
			var commandData = _interpretator.GetTagIdFilterSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Tag ID Filter Settings (0x3A) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTagIdFilterSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.BOOTLOADER_MODE,
				Timer = 0x1
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedTagIdFilterSettings = new TagIDFilterSettings
			{
				Options = 0x6,
				NibbleEnable = new byte[] { 0x1, 0x4, 0x6 },
				TagIDFilter = new byte[] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C }
			};
			var expectation = new ResponseExpectation(expectedTagIdFilterSettings);

			var responseData = new ResponseData
			{
				LEN = 0x10,
				CMD = 0x3A,
				STATUS = 0x4,
				TMR_HI = 0x1,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x6, 0x1, 0x4, 0x6, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var tagIdFilterSettings = _interpretator.ConvertResponseDataToTagIdFilterSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(tagIdFilterSettings));
		}

		/// <summary>
		/// Get Aux In Settings (0x3C) command
		/// </summary>
		[TestMethod]
		public void TestGetAuxInSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x3C, new byte[0], 0x3C);
			var commandData = _interpretator.GetAuxSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Aux In Settings (0x3C) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToAuxSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.BOOTLOADER_MODE,
				Timer = 0x1
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedAuxSettings = new AuxSettings
			{
				Function = AuxSettingsFunction.PulseChannel,
				Option1 = 0x7,
				Option2 = 0x1B,
				EdgeType = EdgeType.Falling,
				Debounce = 0x1,
				DeafTime = 0x1
			};
			var expectation = new ResponseExpectation(expectedAuxSettings);

			var responseData = new ResponseData
			{
				LEN = 0x6,
				CMD = 0x3C,
				STATUS = 0x4,
				TMR_HI = 0x1,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x02, 0x7, 0x1B, 0x0, 0x1, 0x1 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var auxSettings = _interpretator.ConvertResponseDataToAuxSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(auxSettings));
		}

		/// <summary>
		/// Get Encoder In Settings (0x3D) command
		/// </summary>
		[TestMethod]
		public void TestGetEncoderSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x3D, new byte[0], 0x3D);
			var commandData = _interpretator.GetEncoderSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Encoder In Settings (0x3D) response
		/// </summary>
		[TestMethod]
		public void TestConvertReponseDataToEncoderSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.M4E_CW_FAULT,
				Timer = 0x1
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedEncoderSettings = new EncoderSettings
			{
				TriggerFilterMin = 0x3,
				TesterOffset = 0x4,
				MarkerOffset = 0x3,
				PunchOffset = 0x3,
				PunchFlight = 10,
				TriggerFilterMax = 0x1
			};
			var expectation = new ResponseExpectation(expectedEncoderSettings);

			var responseData = new ResponseData
			{
				LEN = 0xC,
				CMD = 0x3D,
				STATUS = 0x47,
				TMR_HI = 0x1,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x0, 0x3, 0x0, 0x4, 0x0, 0x3, 0x0, 0x3, 0x0, 0x0A, 0x0, 0x1 }
			};
			var generalDeviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var encoderSettings = _interpretator.ConvertReponseDataToEncoderSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(generalDeviceResult));
			Assert.IsTrue(expectation.Equals(encoderSettings));
		}
	}
}