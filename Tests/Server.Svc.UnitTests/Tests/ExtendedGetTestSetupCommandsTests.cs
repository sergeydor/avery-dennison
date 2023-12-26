using Common.Domain.DeviceResults;
using Common.Domain.ExtendedTestSetupCommands;
using Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class ExtendedGetTestSetupCommandsTests
	{
		private readonly ExtendedGetTestSetupCommandsInterpretator _interpretator = new ExtendedGetTestSetupCommandsInterpretator();

		/// <summary>
		/// Get Punch Settings (0x40) command
		/// </summary>
		[TestMethod]
		public void TestGetPunchSettingsCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x40, new byte[0x0], 0x40);
			var commandData = _interpretator.GetPunchSettingsCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Punch Settings (0x40) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToPunchSettings()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.HARDWARE_UNAVAILABLE,
				Timer = 0xD
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedPunchSettings = new PunchSettings
			{
				Enable = PunchEnableMode.PunchGood,
				Position = 0x2,
				Duration = 0x8,
				Offset = 0x6
			};
			var expectation = new ResponseExpectation(expectedPunchSettings);

			var responseData = new ResponseData
			{
				LEN = 0x4,
				CMD = 0x40,
				STATUS = 0x2C,
				TMR_HI = 0xD,
				TMR_LO = 0x0,
				DATA = new byte[] {0x2, 0x2, 0x8, 0x6}
			};
			var actualResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var punchSettings = _interpretator.ConvertResponseDataToPunchSettings(responseData);

			Assert.IsTrue(resultExpectation.Equals(actualResult));
			Assert.IsTrue(expectation.Equals(punchSettings));
		}
	}
}