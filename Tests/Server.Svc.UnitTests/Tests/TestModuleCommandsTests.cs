using Common.Domain.DeviceResults;
using Common.Domain.DeviceResults.TestModuleCommands;
using Common.Domain.TestModuleCommands;
using Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class TestModuleCommandsTests
	{
		private readonly TestModuleCommandsInterpretator _interpretator = new TestModuleCommandsInterpretator();

		/// <summary>
		/// Ping (0x00) command
		/// </summary>
		[TestMethod]
		public void TestGetPingCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x0, new byte[0x0], 0x0);
			var commandData = _interpretator.GetPingCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Version (0x01) command
		/// </summary>
		[TestMethod]
		public void TestGetVersionCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x1, new byte[0x0], 0x1);
			var commandData = _interpretator.GetVersionCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Version (0x01) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToVersionResult()
		{
			var expectedResult = new VersionResult
			{
				Major = 0x2,
				Minor = 0x1,
				Year = 0x10,
				Month = 0xA,
				Day = 0x5
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x5,
				CMD = 0x1,
				STATUS = 0x4,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] {0x2, 0x1, 0x10, 0xA, 0x5}
			};
			var actualResult = _interpretator.ConvertResponseDataToVersionResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Reset (0x02) command
		/// </summary>
		[TestMethod]
		public void TestConvertResetSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x1, 0x2, new byte[] {0x3}, 0x6);

			var resetSettings = new ResetSettings {Type = InputResetType.TimerReset};
			var commandData = _interpretator.ConvertResetSettingsToCommandData(resetSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Reset (0x02) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToResetResult()
		{
			var expectedResult = new ResetResult
			{
				Status = StatusCode.BOOTLOADER_MODE,
				Timer = 0x5,
				Type = OutputResetType.BootloaderReset
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x1,
				STATUS = 0x4,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1 }
			};
			var actualResult = _interpretator.ConvertResponseDataToResetResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get Last Fault (0x03) command
		/// </summary>
		[TestMethod]
		public void TestGetLastFaultCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x3, new byte[0x0], 0x3);
			var commandData = _interpretator.GetLastFaultCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Last Fault (0x03) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToLastFaultResult()
		{
			var expectedLastFaultResult = new LastFaultResult
			{
				FaultCode = FaultCode.BIT_RF_SUBSYSTEM_FAULT
			};
			var expectation = new ResponseExpectation(expectedLastFaultResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x3,
				STATUS = 0x1,
				TMR_HI = 0x4,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x82 }
			};
			var lastFaultResult = _interpretator.ConvertResponseDataToLastFaultResult(responseData);

			Assert.IsTrue(expectation.Equals(lastFaultResult));
		}

		/// <summary>
		/// Get Status (0x04) command
		/// </summary>
		[TestMethod]
		public void TestGetStatusCommandData()
		{
			var expecation = new CommandExpectation(0xEE, 0x0, 0x04, new byte[0], 0x04);
			var commandData = _interpretator.GetStatusCommandData();

			Assert.IsTrue(expecation.Equals(commandData));
		}

		/// <summary>
		/// Get Status (0x04) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToGetStatusResult()
		{
			var expectedStatusResult = new GetStatusResult
			{
				Status = StatusCode.INVALID_RF_CHANNEL,
				Timer = 0x9,
				Timer2 = 0x0B,
				CalibrationStatus = CalibrationStatus.OutOfCal | CalibrationStatus.InvalidAntenna,
				Temperature = 0x1A,
				OperatingState = 0x5
			};
			var expectation = new ResponseExpectation(expectedStatusResult);

			var responseData = new ResponseData
			{
				LEN = 0x5,
				CMD = 0x4,
				STATUS = 0x36,
				TMR_HI = 0x9,
				TMR_LO = 0x0,
				DATA = new byte[] {0x0B, 0x0, 0x1A, 0x60, 0x5}
			};
			var getStatusResult = _interpretator.ConvertResponseDataToGetStatusResult(responseData);

			Assert.IsTrue(expectation.Equals(getStatusResult));
		}

		/// <summary>
		/// Clear Last Fault (0x06) command
		/// </summary>
		[TestMethod]
		public void TestGetClearLastFaultCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x6, new byte[0x0], 0x6);
			var commandData = _interpretator.GetClearLastFaultCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Date Time (0x08) command
		/// </summary>
		[TestMethod]
		public void TestConvertLaneDateTimeToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x7, 0x8, new byte[] {0x3, 0x10, 0x0C, 0x1F, 0x0F, 0x3B, 0x3B}, 0xD2);
			var laneDateTime = new LaneDateTime
			{
				Lane = 3,
				Year = 16,
				Month = 12,
				Day = 31,
				Hour = 15,
				Minute = 59,
				Second = 59
			};
			var commandData = _interpretator.ConvertLaneDateTimeToCommandData(laneDateTime);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Date Time (0x09) command
		/// </summary>
		[TestMethod]
		public void TestGetDateTimeCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x9, new byte[0x0], 0x9);
			var commandData = _interpretator.GetDateTimeCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Date Time (0x09) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToLaneDateTime()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.INVALID_PARAMETER,
				Timer = 0x9
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);
			var expectedLaneDateTime = new LaneDateTime
			{
				Lane = 3,
				Year = 16,
				Month = 12,
				Day = 31,
				Hour = 15,
				Minute = 59,
				Second = 59
			};
			var expectation = new ResponseExpectation(expectedLaneDateTime);

			var responseData = new ResponseData
			{
				LEN = 0x7,
				CMD = 0x8,
				STATUS = 0x5,
				TMR_HI = 0x9,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x3, 0x10, 0x0C, 0x1F, 0x0F, 0x3B, 0x3B }
			};
			var deviceResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var laneDateTime = _interpretator.ConvertResponseDataToLaneDateTime(responseData);

			Assert.IsTrue(resultExpectation.Equals(deviceResult));
			Assert.IsTrue(expectation.Equals(laneDateTime));
		}

		/// <summary>
		/// Test RF Settings (0x0B) command
		/// </summary>
		[TestMethod]
		public void TestConvertTestRfSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x6, 0x0B, new byte[] {0x30, 0x0, 0x0, 0x0, 0xB8, 0x0}, 0xF9);

			var testRFSettings = new TestRFSettings
			{
				RequestedFrequency = 0x30,
				RequestedPower = 0xB8
			};
			var commandData = _interpretator.ConvertTestRfSettingsToCommandData(testRFSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Test RF Settings (0x0B) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTestRFSettingsResult()
		{
			var expectedTetRfResult = new TestRFSettingsResult
			{
				Status = StatusCode.INVALID_READ_POWER,
				Timer = 0x3,
				Frequency = 0x31,
				PowerError = 0x5,
				Temp = RFTemperature.Between30And34_9C,
				CTFAD = 0x2,
				CTRAD = 0x9
			};
			var expectation = new ResponseExpectation(expectedTetRfResult);

			var responseData = new ResponseData
			{
				LEN = 0x0B,
				CMD = 0x0B,
				STATUS = 0x33,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] {0x31, 0x0, 0x0, 0x0, 0x5, 0x0, 0x2, 0x2, 0x0, 0x9, 0x0}
			};
			var testRfSettingsResult = _interpretator.ConvertResponseDataToTestRFSettingsResult(responseData);

			Assert.IsTrue(expectation.Equals(testRfSettingsResult));
		}

		/// <summary>
		/// Restart Reader To Bootloader (0x0E) command
		/// </summary>
		[TestMethod]
		public void TestGetRestartReaderCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x0E, new byte[0x0], 0x0E);
			var commandData = _interpretator.GetRestartReaderCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Last Message (0x0F) command
		/// </summary>
		[TestMethod]
		public void TestGetLastMessageCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x0F, new byte[0x0], 0x0F);
			var commandData = _interpretator.GetLastMessageCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Last Message (0x0F) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToLastMessageResult()
		{
			var expectedResult = new LastMessageResult
			{
				Status = StatusCode.M4E_WRITE_POWER_FAULT,
				Timer = 0x5,
				MessageId = 0x32
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x0F,
				STATUS = 0x44,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] {0x32}
			};
			var lastMsgResult = _interpretator.ConvertResponseDataToLastMessageResult(responseData);

			Assert.IsTrue(expectation.Equals(lastMsgResult));
		}
	}
}