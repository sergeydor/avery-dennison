using Common.Domain.ExtendedTestSetupCommands;
using Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class ExtendedTestSetupCommandsTests
	{
		private readonly ExtendedTestSetupCommadsInterpretator _interpretator = new ExtendedTestSetupCommadsInterpretator();

		/// <summary>
		/// Set Punch Settings (0x20) command
		/// </summary>
		[TestMethod]
		public void TestConvertPunchSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x4, 0x20, new byte[] {0x1, 0x21, 0x6, 0xB}, 0x57);
			
			var settings = new PunchSettings
			{
				Enable = PunchEnableMode.PunchBad,
				Position = 0x21,
				Duration = 0x6,
				Offset = 0xB
			};
			var commandData = _interpretator.ConvertPunchSettingsToCommandData(settings);

			Assert.IsTrue(expectation.Equals(commandData));
		}
	}
}