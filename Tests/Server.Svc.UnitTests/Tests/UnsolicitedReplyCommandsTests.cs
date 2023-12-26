using Common.Domain.DeviceResults;
using Common.Domain.DeviceResults.UnsolicitedReplyCommands;
using Common.Enums;
using Common.Enums.UnsolicitedReplyCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class UnsolicitedReplyCommandsTests
	{
		private readonly UnsolicitedReplyCommandsInterpretator _interpretator = new UnsolicitedReplyCommandsInterpretator();

		/// <summary>
		/// Send RF Data (0xE2) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToSendRFDataResult()
		{
			var expectedDeviceResult = new SendRFDataResult
			{
				Status = StatusCode.ANTENNA_1_FAULT,
				Timer = 0x5,
				IFAD = 0x1C,
				IRAD = 0xAA
			};

			var expectation = new ResponseExpectation(expectedDeviceResult);

			var responseData = new ResponseData
			{
				LEN = 0x04,
				CMD = 0xE2,
				STATUS = 0x37,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1C, 0x0, 0xAA, 0x0 },
				FCS = 0x0D
			};
			var actualDeviceResult = _interpretator.ConvertResponseDataToSendRFDataResult(responseData);

			Assert.IsTrue(expectation.Equals(actualDeviceResult));
		}

		/// <summary>
		/// Test (0xE3) command
		/// </summary>
		[TestMethod]
		public void TestGetTestE3CommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0xE3, new byte[0], 0x52);
			var command = _interpretator.GetTestE3CommandData();

			Assert.IsTrue(expectation.Equals(command));
		}


		/// <summary>
		/// Fault (0xE0) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseToFaultResult()
		{
			var expectedFaultResult = new FaultResult
			{
				Status = StatusCode.TRANSMIT_POWER_FAULT,
				Timer = 0x5,
				FaultID = FaultCode.BIT_RF_SUBSYSTEM_FAULT
			};
			var expectation = new ResponseExpectation(expectedFaultResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0xE0,
				STATUS = 0x3A,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x82 }
			};
			var faultResult = _interpretator.ConvertResponseToFaultResult(responseData);

			Assert.IsTrue(expectation.Equals(faultResult));
		}

		/// <summary>
		/// Message (0xE1) command
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToMessageResult()
		{
			var expectedMessageResult = new MessageResult
			{
				Status = StatusCode.TRANSMIT_POWER_FAULT,
				Timer = 0x5,
				MessageID = 0x2
			};
			var expectation = new ResponseExpectation(expectedMessageResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0xE1,
				STATUS = 0x3A,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x2 }
			};
			var messageResult = _interpretator.ConvertResponseDataToMessageResult(responseData);

			Assert.IsTrue(expectation.Equals(messageResult));
		}

		/// <summary>
		/// Test (0xE3) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTestE3Result()
		{
            var expecteDeviceResult = new TestE3Result
            {
                Status = StatusCode.FCS_ERROR,
                Timer = 0x3,
                TestType = TestE3Type.F3Read | TestE3Type.ReadTest | TestE3Type.WriteTest,
                TestStatus = TestE3Status.RdrFailCRC | TestE3Status.ReadFail,
                TestCount = 0x0A,
				EPCTagID = new byte[] {0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C},
				Sensitivity = 0x0B,
				TID = new byte[] { 0x1, 0x0, 0x3, 0x0, 0x0, 0x5, 0x0, 0x0, 0x6, 0x0, 0x0, 0x0 }
			};

			var expectation = new ResponseExpectation(expecteDeviceResult);

			var responseData = new ResponseData
			{
				LEN = 0x20,
				CMD = 0xE3,
				STATUS = 0x2,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0xC4, 0x84, 0x0A, 0x0, 0x0, 0x0, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0x0A, 0x0B, 0x0C, 0x0B, 0x0, 0x1, 0x0, 0x3, 0x0, 0x0, 0x5, 0x0, 0x0, 0x6, 0x0, 0x0, 0x0 },
				FCS = 0xA1
			};
			var actualDeviceResult = _interpretator.ConvertResponseDataToTestE3Result(responseData);

			Assert.IsTrue(expectation.Equals(actualDeviceResult));
		}

		/// <summary>
		/// Results of Sensitivity Test (0xE4) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToSensitivityTestResult()
		{
			var expectedSenstitivityTestResult = new SensitivityTestResult
			{
				Status = StatusCode.TRANSMIT_POWER_FAULT,
				Timer = 0x5,
				PassState = PassState.Pass,
				Power = 0x0C
			};
			var expectation = new ResponseExpectation(expectedSenstitivityTestResult);

			var responseData = new ResponseData
			{
				LEN = 0x3,
				CMD = 0xE4,
				STATUS = 0x3A,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] {0x1, 0x0C, 0x0}
			};
			var sensitivityTestResult = _interpretator.ConvertResponseDataToSensitivityTestResult(responseData);

			Assert.IsTrue(expectation.Equals(sensitivityTestResult));
		}

		/// <summary>
		/// Event (0xE5) response
		/// </summary>
		[TestMethod]
		public void ConvertResponseDataToEventResult()
		{
			var expectedEventResult = new EventResult
			{
				Status = StatusCode.TRANSMIT_POWER_FAULT,
				Timer = 0x5,
				EventId = EventID.ResetStats,
				TBD = new byte[] {0x2, 0x5, 0x7, 0x9}
			};
			var expectation = new ResponseExpectation(expectedEventResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0xE5,
				STATUS = 0x3A,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1, 0x2, 0x5, 0x7, 0x9 }
			};
			var eventResult = _interpretator.ConvertResponseDataToEventResult(responseData);

			Assert.IsTrue(expectation.Equals(eventResult));
		}
	}
}
