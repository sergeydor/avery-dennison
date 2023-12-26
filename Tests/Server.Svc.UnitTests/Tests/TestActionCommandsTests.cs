using Common.Domain.DeviceResults.TestActionCommands;
using Common.Enums;
using Common.Enums.TestActionCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class TestActionCommandsTests
	{
		private readonly TestActionCommandsInterpretator _interpretator = new TestActionCommandsInterpretator();

		/// <summary>
		/// Start Testing (0x50) command
		/// </summary>
		[TestMethod]
		public void TestGetStartTestingCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x01, 0x50, new byte[] {0x2}, 0x53);
			var commandData = _interpretator.GetStartTestingCommandData(TestMode.HSTest);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Stop Testing (0x51) command
		/// </summary>
		[TestMethod]
		public void TestGetStopTestingCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x51, new byte[0], 0x51);
			var commandData = _interpretator.GetStopTestingCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Test (0x52) command
		/// </summary>
		[TestMethod]
		public void TestGetTest52CommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x52, new byte[0], 0x52);
			var commandData = _interpretator.GetTest52CommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Sensitivity Trigger Input (0x54) command
		/// </summary>
		[TestMethod]
		public void TestGetSensitivityTriggerInputCommandData()
		{
			var expecation = new CommandExpectation(0xEE, 0x0, 0x54, new byte[0], 0x54);
			var commandData = _interpretator.GetSensitivityTriggerInputCommandData();

			Assert.IsTrue(expecation.Equals(commandData));
		}

		/// <summary>
		/// Sensitivity Trigger Input (0x54) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToSensitivityTriggerInputResult()
		{
			var expecteDeviceResult = new SensitivityTriggerInputResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x3,
				TriggerCount = 0x1,
				EncoderCount = 0x2
			};

			var expectation = new ResponseExpectation(expecteDeviceResult);

			var responseData = new ResponseData
			{
				LEN = 0x06,
				CMD = 0x54,
				STATUS = 0x2C,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] {0x01, 0x0, 0x0, 0x0, 0x02, 0x0},
				FCS = 0x60
			};
			var deviceResult = _interpretator.ConvertResponseDataToSensitivityTriggerInputResult(responseData);

			Assert.IsTrue(expectation.Equals(deviceResult));
		}

		/// <summary>
		/// Punch (0x56) command
		/// </summary>
		[TestMethod]
		public void TestGetPunchCommandData()
		{
			var expecation = new CommandExpectation(0xEE, 0x0, 0x56, new byte[0], 0x56);
			var commandData = _interpretator.GetPunchCommandData();

			Assert.IsTrue(expecation.Equals(commandData));
		}

		/// <summary>
		/// Get Hardware Version (0x62) command
		/// </summary>
		[TestMethod]
		public void TestGetHardwareVersion()
		{
			var expecation = new CommandExpectation(0xEE, 0x0, 0x62, new byte[0], 0x62);
			var commandData = _interpretator.GetHardwareVersion();

			Assert.IsTrue(expecation.Equals(commandData));
		}

		/// <summary>
		/// Get Hardware Version (0x62) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToHardwareVersionResult()
		{
			var expecteDeviceResult = new HardwareVersionResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x3,
				CheckByte1 = 0x0A,
				CheckByte2 = 0x0D,
				BoardType = BoardType.STBoard,
				BoardRevision = 0x0C,
				ID4 = 0x4,
				ID3 = 0x3,
				ID2 = 0x2,
				ID1 = 0x1
			};
			var expectation = new ResponseExpectation(expecteDeviceResult);

			var responseData = new ResponseData
			{
				LEN = 0x08,
				CMD = 0x62,
				STATUS = 0x2C,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x0A, 0x0D, 0x2, 0x0C, 0x4, 0x3, 0x2, 0x1 }
			};
			var deviceResult = _interpretator.ConvertResponseDataToHardwareVersionResult(responseData);

			Assert.IsTrue(expectation.Equals(deviceResult));
		}

		/// <summary>
		/// Set Fault (0x63) command
		/// </summary>
		[TestMethod]
		public void TestGetSetFaultCommandData()
		{
			var expecation = new CommandExpectation(0xEE, 0x1, 0x63, new byte[0x1] {0x81}, 0xE5);
			var commandData = _interpretator.GetSetFaultCommandData(FaultCode.BIT_COMMS_FAULT);

			Assert.IsTrue(expecation.Equals(commandData));
		}

		/// <summary>
		/// Set Digital IO (0x68) command
		/// </summary>
		[TestMethod]
		public void TestGetSetDigitalIOCommandData()
		{
			var expecation = new CommandExpectation(0xEE, 0x1, 0x68, new byte[] { 0x2 }, 0x6B);
			var commandData = _interpretator.GetSetDigitalIOCommandData(InputDigitalValue.Aux);

			Assert.IsTrue(expecation.Equals(commandData));
		}

		/// <summary>
		/// Get Digital IO (0x69) command
		/// </summary>
		[TestMethod]
		public void TestGetDigitalIOCommandData()
		{
			var expecation = new CommandExpectation(0xEE, 0x0, 0x69, new byte[0x0], 0x69);
			var commandData = _interpretator.GetDigitalIOCommandData();

			Assert.IsTrue(expecation.Equals(commandData));
		}

		/// <summary>
		/// Get Digital IO (0x69) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToDigitalIOResult()
		{
			var expecteDeviceResult = new DigitalIOResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0x3,
				IOValue = OutputDigitalValue.Encoder
			};
			var expectation = new ResponseExpectation(expecteDeviceResult);

			var responseData = new ResponseData
			{
				LEN = 0x08,
				CMD = 0x62,
				STATUS = 0x2C,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0xB }
			};
			var deviceResult = _interpretator.ConvertResponseDataToDigitalIOResult(responseData);

			Assert.IsTrue(expectation.Equals(deviceResult));
		}
	}
}