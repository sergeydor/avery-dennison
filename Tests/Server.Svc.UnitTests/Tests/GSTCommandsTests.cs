using Common.Domain.DeviceResults;
using Common.Domain.DeviceResults.GSTCommands;
using Common.Domain.GSTCommands;
using Common.Enums;
using Common.Enums.GSTCommands;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Server.Device.Communication;
using Server.Device.Communication.CommandInterpretators;
using Server.Svc.UnitTests.Helpers;

namespace Server.Svc.UnitTests.Tests
{
	[TestClass]
	public class GSTCommandsTests
	{
		private readonly GSTCommandsInterpretator _interpretator = new GSTCommandsInterpretator();

		/// <summary>
		/// Read Dip Switches (0x70) command
		/// </summary>
		[TestMethod]
		public void TestGetReadDipSwitchesCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x70, new byte[0x0], 0x70);
			var commandData = _interpretator.GetReadDipSwitchesCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Read Dip Switches (0x70) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToReadDipSwitchesResult()
		{
			var expectedResult = new ReadDipSwitchesResult
			{
				Status = StatusCode.M4E_TAG_PROTOCOL_FAULT,
				Timer = 0x5,
				Dip = 0x5
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x70,
				STATUS = 0x42,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x5 }
			};
			var actualResult = _interpretator.ConvertResponseDataToReadDipSwitchesResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get MAC Address (0x71) and Get WiFi Module MAC Address (0x7C) commands
		/// </summary>
		[TestMethod]
		public void TestGetMACAddressCommandData()
		{
			var expectation_0x71 = new CommandExpectation(0xEE, 0x1, 0x71, new byte[] { 0x6 }, 0x78);
			var expectation_0x7C = new CommandExpectation(0xEE, 0x1, 0x7C, new byte[] { 0x6 }, 0x83);

			var commandData_0x71 = _interpretator.GetMACAddressCommandData(0x71);
			var commandData_0x7C = _interpretator.GetMACAddressCommandData(0x7C);

			Assert.IsTrue(expectation_0x71.Equals(commandData_0x71));
			Assert.IsTrue(expectation_0x7C.Equals(commandData_0x7C));
		}

		/// <summary>
		/// Get MAC Address (0x71) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToMacAddressResult()
		{
			var expectedResult = new MACAddressResult
			{
				Status = StatusCode.CMD_UNKNOWN,
				Timer = 0x2,
				MACLength = 0x6,
				Address5 = 0x1A,
				Address4 = 0x1B,
				Address3 = 0x1C,
				Address2 = 0x1D,
				Address1 = 0x1E,
				Address0 = 0x1F,
				CommandCode = 0x71
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x71,
				STATUS = 0x3,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x6, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F }
			};
			var actualResult = _interpretator.ConvertResponseDataToMacAddressResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Common method for Set Devices Power commands. Creates CommandData object for commands:
		/// <list type="bullet">
		/// <item><description>0x72 (Set Barcode Reader Power On/Off)</description></item>
		/// <item><description>0x73 (Set RFID Reader Power On/Off)</description></item>
		/// <item><description>0x74 (Set Camera Power On/Off)</description></item>
		/// <item><description>0x75 (Set Motor Enable On/Off)</description></item>
		/// <item><description>0x78 (Set USB To Spare Serial Port Mirroring)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void TestGetDevicePowerSwitchCommandData()
		{
			var expectation_0x72 = new CommandExpectation(0xEE, 0x1, 0x72, new byte[] { 0x1 }, 0x74);
			var expectation_0x73 = new CommandExpectation(0xEE, 0x1, 0x73, new byte[] { 0x1 }, 0x75);
			var expectation_0x74 = new CommandExpectation(0xEE, 0x1, 0x74, new byte[] { 0x1 }, 0x76);
			var expectation_0x75 = new CommandExpectation(0xEE, 0x1, 0x75, new byte[] { 0x1 }, 0x77);
			var expectation_0x78 = new CommandExpectation(0xEE, 0x1, 0x78, new byte[] { 0x1 }, 0x7A);

			var commandData_0x72 = _interpretator.GetDevicePowerSwitchCommandData(0x72, PowerMode.On);
			var commandData_0x73 = _interpretator.GetDevicePowerSwitchCommandData(0x73, PowerMode.On);
			var commandData_0x74 = _interpretator.GetDevicePowerSwitchCommandData(0x74, PowerMode.On);
			var commandData_0x75 = _interpretator.GetDevicePowerSwitchCommandData(0x75, PowerMode.On);
			var commandData_0x78 = _interpretator.GetDevicePowerSwitchCommandData(0x78, PowerMode.On);

			Assert.IsTrue(expectation_0x72.Equals(commandData_0x72));
			Assert.IsTrue(expectation_0x73.Equals(commandData_0x73));
			Assert.IsTrue(expectation_0x74.Equals(commandData_0x74));
			Assert.IsTrue(expectation_0x75.Equals(commandData_0x75));
			Assert.IsTrue(expectation_0x78.Equals(commandData_0x78));
		}

		/// <summary>
		/// Common method for Set Devices Power responses. Creates DevicePowerSwitchResult object for responses:
		/// <item><description>0x72 (Set Barcode Reader Power On/Off)</description></item>
		/// <item><description>0x73 (Set RFID Reader Power On/Off)</description></item>
		/// <item><description>0x74 (Set Camera Power On/Off)</description></item>
		/// <item><description>0x75 (Set Motor Enable On/Off)</description></item>
		/// <item><description>0x78 (Set USB To Spare Serial Port Mirroring)</description></item>
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToDevicePowerSwitchResult()
		{
			var commandCodes = new byte[] {0x72, 0x73, 0x74, 0x75, 0x78};
			foreach (var commandCode in commandCodes)
			{
				var expectedResult = new DevicePowerSwitchResult
				{
					Status = StatusCode.M4E_TAG_PROTOCOL_FAULT,
					Timer = 0x5,
					CommandCode = commandCode,
					PowerMode = PowerMode.On
				};
				var expectation = new ResponseExpectation(expectedResult);

				var responseData = new ResponseData
				{
					LEN = 0x1,
					CMD = commandCode,
					STATUS = 0x42,
					TMR_HI = 0x5,
					TMR_LO = 0x0,
					DATA = new byte[] { 0x1 }
				};
				var actualResult = _interpretator.ConvertResponseDataToDevicePowerSwitchResult(responseData);

				Assert.IsTrue(expectation.Equals(actualResult));
			}
		}

		/// <summary>
		/// Set Motor Speed (0x76) command
		/// </summary>
		[TestMethod]
		public void TestConvertMotoSpeedToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x1, 0x76, new byte[] { 0x60 }, 0xD7);

			var motorSpeed = new MotorSpeed {SpeedValue = 0x60};
			var commandData = _interpretator.ConvertMotoSpeedToCommandData(motorSpeed);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set Motor Speed (0x76) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToMotorSpeed()
		{
			var expectedDeviceResult = new GeneralDeviceResult
			{
				Status = StatusCode.CMD_UNKNOWN,
				Timer = 0x2
			};
			var resultExpectation = new ResponseExpectation(expectedDeviceResult);

			var expectedMotorSpeed = new MotorSpeed
			{
				SpeedValue = 0x50
			};
			var expectation = new ResponseExpectation(expectedMotorSpeed);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x76,
				STATUS = 0x3,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x50 }
			};
			var actualResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var motorSpeed = _interpretator.ConvertResponseDataToMotorSpeed(responseData);

			Assert.IsTrue(resultExpectation.Equals(actualResult));
			Assert.IsTrue(expectation.Equals(motorSpeed));
		}

		/// <summary>
		/// Twinkle LED's (0x77) command
		/// </summary>
		[TestMethod]
		public void TestGetTwinkleLedCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x77, new byte[0x0], 0x77);
			var commandData = _interpretator.GetTwinkleLedCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Twinkle LED's (0x77) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToTwinkleLedResult()
		{
			var expectedResult = new TwinkleLEDResult
			{
				Status = StatusCode.M4E_TAG_PROTOCOL_FAULT,
				Timer = 0x5,
				LED = 0x60
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x77,
				STATUS = 0x42,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x60 }
			};
			var actualResult = _interpretator.ConvertResponseDataToTwinkleLedResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get Camera Outputs (0x7B) command
		/// </summary>
		[TestMethod]
		public void TestGetCameraOutputCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x7B, new byte[0x0], 0x7B);
			var commandData = _interpretator.GetCameraOutputCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Camera Outputs (0x7B) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToCameraOutputsResult()
		{
			var expectedResult = new CameraOutputsResult
			{
				Status = StatusCode.M4E_TAG_PROTOCOL_FAULT,
				Timer = 0x5,
				GPIO = 0x23
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x7B,
				STATUS = 0x42,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x23 }
			};
			var actualResult = _interpretator.ConvertResponseDataToCameraOutputsResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get WiFi Module MAC Address (0x7C) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToWifiModuleMacAddressResult()
		{
			var expectedResult = new MACAddressResult
			{
				Status = StatusCode.CMD_UNKNOWN,
				Timer = 0x2,
				MACLength = 0x6,
				Address5 = 0x1A,
				Address4 = 0x1B,
				Address3 = 0x1C,
				Address2 = 0x1D,
				Address1 = 0x1E,
				Address0 = 0x1F,
				CommandCode = 0x7C
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x7C,
				STATUS = 0x3,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x6, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F }
			};
			var actualResult = _interpretator.ConvertResponseDataToMacAddressResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get WiFi Network ID (0x7D) and Get WiFI Network Security Key (0x7E) commands
		/// </summary>
		[TestMethod]
		public void TestGetWifiNetworkSettingsCommandData()
		{
			var expectation_0x7D = new CommandExpectation(0xEE, 0x1, 0x7D, new byte[] { 0x6 }, 0x84);
			var expectation_0x7E = new CommandExpectation(0xEE, 0x1, 0x7E, new byte[] { 0x6 }, 0x85);

			var commandData_0x7D = _interpretator.GetWifiNetworkSettingsCommandData(0x7D, 0x6);
			var commandData_0x7E = _interpretator.GetWifiNetworkSettingsCommandData(0x7E, 0x6);

			Assert.IsTrue(expectation_0x7D.Equals(commandData_0x7D));
			Assert.IsTrue(expectation_0x7E.Equals(commandData_0x7E));
		}

		/// <summary>
		/// Get WiFi Network ID (0x7D) and Get WiFI Network Security Key (0x7E) responses
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToWifiNetworkSettingsResult()
		{
			var expectedResult = new WifiNetworkSettingsResult
			{
				Status = StatusCode.CMD_UNKNOWN,
				Timer = 0x2,
				CommandCode = 0x7C,
				Length = 0x20,
				WifiSSID = new byte[0x20] { 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20 }
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x7C,
				STATUS = 0x3,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x20, 0x1, 0x2, 0x3, 0x4, 0x5, 0x6, 0x7, 0x8, 0x9, 0xA, 0xB, 0xC, 0xD, 0xE, 0xF, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16, 0x17, 0x18, 0x19, 0x1A, 0x1B, 0x1C, 0x1D, 0x1E, 0x1F, 0x20 }
			};
			var actualResult = _interpretator.ConvertResponseDataToWifiNetworkSettingsResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Common method for Set Devices Activity State commands. Creates CommandData object for commands:
		/// <list type="bullet">
		/// <item><description>0x7A (Set Camera Teach to Active)</description></item>
		/// <item><description>0x79 (Set Camera Trigger to Active)</description></item>
		/// <item><description>0x89 (Set Mark Active)</description></item>
		/// <item><description>0x8A (Set Punch Active)</description></item>
		/// <item><description>0x8B (Set Aux Active)</description></item>
		/// <item><description>0x8C (Set Snap Relay Active)</description></item>
		/// <item><description>0x8D (Set Monarch Pause Active)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void TestGetDeviceActivityStateCommandData()
		{
			var expectation_0x7A = new CommandExpectation(0xEE, 0x1, 0x7A, new byte[] { 0x1 }, 0x7C);
			var expectation_0x79 = new CommandExpectation(0xEE, 0x1, 0x79, new byte[] { 0x1 }, 0x7B);
			var expectation_0x89 = new CommandExpectation(0xEE, 0x1, 0x89, new byte[] { 0x1 }, 0x8B);
			var expectation_0x8A = new CommandExpectation(0xEE, 0x1, 0x8A, new byte[] { 0x1 }, 0x8C);
			var expectation_0x8B = new CommandExpectation(0xEE, 0x1, 0x8B, new byte[] { 0x1 }, 0x8D);
			var expectation_0x8C = new CommandExpectation(0xEE, 0x1, 0x8C, new byte[] { 0x1 }, 0x8E);
			var expectation_0x8D = new CommandExpectation(0xEE, 0x1, 0x8D, new byte[] { 0x1 }, 0x8F);

			var commandData_0x7A = _interpretator.GetDeviceActivityStateCommandData(0x7A, ActivityState.Active);
			var commandData_0x79 = _interpretator.GetDeviceActivityStateCommandData(0x79, ActivityState.Active);
			var commandData_0x89 = _interpretator.GetDeviceActivityStateCommandData(0x89, ActivityState.Active);
			var commandData_0x8A = _interpretator.GetDeviceActivityStateCommandData(0x8A, ActivityState.Active);
			var commandData_0x8B = _interpretator.GetDeviceActivityStateCommandData(0x8B, ActivityState.Active);
			var commandData_0x8C = _interpretator.GetDeviceActivityStateCommandData(0x8C, ActivityState.Active);
			var commandData_0x8D = _interpretator.GetDeviceActivityStateCommandData(0x8D, ActivityState.Active);

			Assert.IsTrue(expectation_0x7A.Equals(commandData_0x7A));
			Assert.IsTrue(expectation_0x79.Equals(commandData_0x79));
			Assert.IsTrue(expectation_0x89.Equals(commandData_0x89));
			Assert.IsTrue(expectation_0x8A.Equals(commandData_0x8A));
			Assert.IsTrue(expectation_0x8B.Equals(commandData_0x8B));
			Assert.IsTrue(expectation_0x8C.Equals(commandData_0x8C));
			Assert.IsTrue(expectation_0x8D.Equals(commandData_0x8D));
		}

		/// <summary>
		/// Common method for Set Devices Activity State responses. Creates CommandData object for responses:
		/// <list type="bullet">
		/// <item><description>0x79 (Set Camera Trigger to Active)</description></item>
		/// <item><description>0x7A (Set Camera Teach to Active)</description></item>
		/// <item><description>0x89 (Set Mark Active)</description></item>
		/// <item><description>0x8A (Set Punch Active)</description></item>
		/// <item><description>0x8B (Set Aux Active)</description></item>
		/// <item><description>0x8C (Set Snap Relay Active)</description></item>
		/// <item><description>0x8D (Set Monarch Pause Active)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToDeviceActivityStateResult()
		{
			var commandCodes = new byte[] { 0x79, 0x7A, 0x89, 0x8A, 0x8B, 0x8C, 0x8D };
			foreach (var commandCode in commandCodes)
			{
				var expectedResult = new DeviceActivityStateResult
				{
					Status = StatusCode.M4E_BAUD_RATE_FAULT,
					Timer = 0x9,
					CommandCode = commandCode,
					ActivityState = ActivityState.Active
				};
				var expectation = new ResponseExpectation(expectedResult);

				var responseData = new ResponseData
				{
					LEN = 0x1,
					CMD = commandCode,
					STATUS = 0x45,
					TMR_HI = 0x9,
					TMR_LO = 0x0,
					DATA = new byte[] { 0x1 }
				};
				var actualResult = _interpretator.ConvertResponseDataToDeviceActivityStateResult(responseData);

				Assert.IsTrue(expectation.Equals(actualResult));
			}
		}

		/// <summary>
		/// Common method for Get Devices Status commands. Creates CommandData object for commands:
		/// <list type="bullet">
		/// <item><description>0x7F (Get Encoder A Status)</description></item>
		/// <item><description>0x80 (Get Encoder B Status)</description></item>
		/// <item><description>0x81 (Get Trigger Sensor 0 Status)</description></item>
		/// <item><description>0x82 (Get Trigger Sensor 1 Status)</description></item>
		/// <item><description>0x83 (Get Spare 1 Trigger Sensor Status)</description></item>
		/// <item><description>0x84 (Get Spare 2 Trigger Sensor Status)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void TestGetDeviceStatusCommandData()
		{
			var expectation_0x7F = new CommandExpectation(0xEE, 0x0, 0x7F, new byte[0], 0x7F);
			var expectation_0x80 = new CommandExpectation(0xEE, 0x0, 0x80, new byte[0], 0x80);
			var expectation_0x81 = new CommandExpectation(0xEE, 0x0, 0x81, new byte[0], 0x81);
			var expectation_0x82 = new CommandExpectation(0xEE, 0x0, 0x82, new byte[0], 0x82);
			var expectation_0x83 = new CommandExpectation(0xEE, 0x0, 0x83, new byte[0], 0x83);
			var expectation_0x84 = new CommandExpectation(0xEE, 0x0, 0x84, new byte[0], 0x84);

			var commandData_0x7F = _interpretator.GetDeviceStatusCommandData(0x7F);
			var commandData_0x80 = _interpretator.GetDeviceStatusCommandData(0x80);
			var commandData_0x81 = _interpretator.GetDeviceStatusCommandData(0x81);
			var commandData_0x82 = _interpretator.GetDeviceStatusCommandData(0x82);
			var commandData_0x83 = _interpretator.GetDeviceStatusCommandData(0x83);
			var commandData_0x84 = _interpretator.GetDeviceStatusCommandData(0x84);

			Assert.IsTrue(expectation_0x7F.Equals(commandData_0x7F));
			Assert.IsTrue(expectation_0x80.Equals(commandData_0x80));
			Assert.IsTrue(expectation_0x81.Equals(commandData_0x81));
			Assert.IsTrue(expectation_0x82.Equals(commandData_0x82));
			Assert.IsTrue(expectation_0x83.Equals(commandData_0x83));
			Assert.IsTrue(expectation_0x84.Equals(commandData_0x84));
		}

		/// <summary>
		/// Common method for Get Devices Status responses. Creates CommandData object for responses:
		/// <list type="bullet">
		/// <item><description>0x7F (Get Encoder A Status)</description></item>
		/// <item><description>0x80 (Get Encoder B Status)</description></item>
		/// <item><description>0x81 (Get Trigger Sensor 0 Status)</description></item>
		/// <item><description>0x82 (Get Trigger Sensor 1 Status)</description></item>
		/// <item><description>0x83 (Get Spare 1 Trigger Sensor Status)</description></item>
		/// <item><description>0x84 (Get Spare 2 Trigger Sensor Status)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToDeviceStatusResult()
		{
			var commandCodes = new byte[] { 0x7F, 0x80, 0x81, 0x82, 0x83, 0x84 };
			foreach (var commandCode in commandCodes)
			{
				var expectedResult = new DeviceStatusResult
				{
					Status = StatusCode.TEST_ERROR,
					Timer = 0x3,
					CommandCode = commandCode,
					DeviceStatus = 0x1
				};
				var expectation = new ResponseExpectation(expectedResult);

				var responseData = new ResponseData
				{
					LEN = 0x1,
					CMD = commandCode,
					STATUS = 0x52,
					TMR_HI = 0x3,
					TMR_LO = 0x0,
					DATA = new byte[] { 0x1 }
				};
				var actualResult = _interpretator.ConvertResponseDataToDeviceStatusResult(responseData);

				Assert.IsTrue(expectation.Equals(actualResult));
			}
		}

		/// <summary>
		/// Set GPI0 RFID Input (0x85) command
		/// </summary>
		[TestMethod]
		public void TestGetGPI0RFIDInputCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x1, 0x85, new byte[] { 0x1 }, 0x87);
			var commandData = _interpretator.GetGPI0RFIDInputCommandData(0x1);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set GPI0 RFID Input (0x85) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToGPI0RFIDInputResult()
		{
			var expectedResult = new GPI0RFIDInputResult
			{
				Status = StatusCode.INVALID_CAL_SETTING,
				Timer = 0x6,
				GPI0 = 0x1
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x85,
				STATUS = 0x3F,
				TMR_HI = 0x6,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1 }
			};
			var actualResult = _interpretator.ConvertResponseDataToGPI0RFIDInputResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Set GPI1 RFID Input (0x86) command
		/// </summary>
		[TestMethod]
		public void TestGetGPI1RFIDInputCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x86, new byte[0x0], 0x86);
			var commandData = _interpretator.GetGPI1RFIDInputCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Set GPI1 RFID Input (0x86) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToGPI1RFIDInputResult()
		{
			var expectedResult = new GPI1RFIDInputResult
			{
				Status = StatusCode.INVALID_CAL_SETTING,
				Timer = 0x6,
				GPI1 = 0x1
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x85,
				STATUS = 0x3F,
				TMR_HI = 0x6,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1 }
			};
			var actualResult = _interpretator.ConvertResponseDataToGPI1RFIDInputResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get GPIO RFID Reader Status (0x87) command
		/// </summary>
		[TestMethod]
		public void TestGetGpioRfidReaderStatusCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x87, new byte[0x0], 0x87);
			var commandData = _interpretator.GetGpioRfidReaderStatusCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get GPIO RFID Reader Status (0x87) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToGpioRfidReaderStatusResult()
		{
			var expectedResult = new GpioRfidReaderStatusResult
			{
				Status = StatusCode.INVALID_FREQUENCY,
				Timer = 0x2,
				ReaderStatus = 0x6
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x87,
				STATUS = 0x30,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] {0x6}
			};
			var actualResult = _interpretator.ConvertResponseDataToGpioRfidReaderStatusResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get Board Temperature (0x88) command
		/// </summary>
		[TestMethod]
		public void TestGetBoardTemperatureCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x88, new byte[0x0], 0x88);
			var commandData = _interpretator.GetBoardTemperatureCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Board Temperature (0x88) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToBoardTemperatureResult()
		{
			var expectedResult = new BoardTemperatureResult
			{
				Status = StatusCode.INVALID_FREQUENCY,
				Timer = 0x2,
				Temperature = 0x29
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x88,
				STATUS = 0x30,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x29, 0x0 }
			};
			var actualResult = _interpretator.ConvertResponseDataToBoardTemperatureResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Get Mark, Punch, Aux, Snap and Monarch Pause States (0x8E) command
		/// </summary>
		[TestMethod]
		public void TestGetDevicePauseStateCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x8E, new byte[0x0], 0x8E);
			var commandData = _interpretator.GetDevicePauseStateCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get Push Button Status (0x8F) command
		/// </summary>
		[TestMethod]
		public void TestGetPushButtonStatusCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x8F, new byte[0x0], 0x8F);
			var commandData = _interpretator.GetPushButtonStatusCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Common method for Get Mark, Punch, Aux, Snap and Monarch Pause States (0x8E) response and Get Push Button Status (0x8F) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToDeviceActionStatusResult()
		{
			var expectedResult = new DeviceActionStatusResult
			{
				Status = StatusCode.OK,
				Timer = 0x2,
				ActionStatus = 0x8
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x8E,
				STATUS = 0x0,
				TMR_HI = 0x2,
				TMR_LO = 0x0,
				DATA = new byte[] {0x8}
			};
			var actualResult = _interpretator.ConvertResponseDataToDeviceActionStatusResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// High Speed GPIO Test Mode (0x90) command
		/// </summary>
		[TestMethod]
		public void TestGetHighSpeedGpioTestModeCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x90, new byte[0x0], 0x90);
			var commandData = _interpretator.GetHighSpeedGpioTestModeCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Exit High Speed GPIO/Reader Test Mode (0x91) command
		/// </summary>
		[TestMethod]
		public void TestGetExitHighSpeedTestModeCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x91, new byte[0x0], 0x91);
			var commandData = _interpretator.GetExitHighSpeedTestModeCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// High Speed Reader Test Mode (0x94) command
		/// </summary>
		[TestMethod]
		public void TestGetHighSpeedReaderTestModeCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x94, new byte[0x0], 0x94);
			var commandData = _interpretator.GetHighSpeedReaderTestModeCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Common method for GPIO/Reader high speed test (start/exit). Creates DeviceHighSpeedTestResult object for responses:
		/// <list type="bullet">
		/// <item><description>High Speed GPIO Test Mode (0x90)</description></item>
		/// <item><description>Exit High Speed GPIO/Reader Test Mode (0x91)</description></item>
		/// <item><description>High Speed Reader Test Mode (0x94)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToDeviceHighSpeedTestResult()
		{
			// GPIO test
			var gpioExpectedResult = new DeviceHighSpeedTestResult
			{
				Status = StatusCode.ANTENNA_1_FAULT,
				Timer = 0x3,
				Mode = 0x1,
				DeviceType = HighSpeedTestDeviceType.GPIO
			};
			var gpioExpectation = new ResponseExpectation(gpioExpectedResult);

			var gpioResponseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x90,
				STATUS = 0x37,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1 }
			};
			var gpioActualResult = _interpretator.ConvertResponseDataToDeviceHighSpeedTestResult(gpioResponseData);

			Assert.IsTrue(gpioExpectation.Equals(gpioActualResult));

			// Reader test
			var readerExpectedResult = new DeviceHighSpeedTestResult
			{
				Status = StatusCode.ANTENNA_1_FAULT,
				Timer = 0x3,
				Mode = 0x1,
				DeviceType = HighSpeedTestDeviceType.Reader
			};
			var readerExpectation = new ResponseExpectation(readerExpectedResult);

			var readerResponseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x94,
				STATUS = 0x37,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1 }
			};
			var readerActualResult = _interpretator.ConvertResponseDataToDeviceHighSpeedTestResult(readerResponseData);

			Assert.IsTrue(readerExpectation.Equals(readerActualResult));

			// Exit GPIO test
			var exitGpioExpectedResult = new DeviceHighSpeedTestResult
			{
				Status = StatusCode.ANTENNA_1_FAULT,
				Timer = 0x3,
				Mode = 0x1,
				DeviceType = HighSpeedTestDeviceType.Reader
			};
			var exitGpioExpectation = new ResponseExpectation(exitGpioExpectedResult);

			var exitGpioResponseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x94,
				STATUS = 0x37,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x1 }
			};
			var exitGpioActualResult = _interpretator.ConvertResponseDataToDeviceHighSpeedTestResult(exitGpioResponseData);

			Assert.IsTrue(exitGpioExpectation.Equals(exitGpioActualResult));
		}

		/// <summary>
		/// High Speed GPIO Test Mode Streaming (Packet Sent Asynchronously at GPIO Test Mode Timer Interval) (0x90)
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToHighSpeedGpioTestModeStreamingResult()
		{
			var expectedResult = new HighSpeedGPIOTestResult
			{
				Status = StatusCode.ANTENNA_1_FAULT,
				Timer = 0x3,
				Sensor0Count = 0x4,
				SensorACount = 1000,
				DeviceID = new byte[] {0x1, 0x5, 0x3, 0x12, 0x32, 0x6}
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x90,
				STATUS = 0x37,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] { 0x0, 0x0, 0x0, 0x4,  0x0, 0x0, 0x03, 0xE8,  0x1, 0x5, 0x3, 0x12, 0x32, 0x6 }
			};
			var actualResult = _interpretator.ConvertResponseDataToHighSpeedGpioTestStreamingResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Common method for set high speed test mode timer commands. Create CommandData for commands:
		/// <list type="bullet">
		/// <item><description>Set High Speed GPIO Test Mode Timer (0x91)</description></item>
		/// <item><description>Set High Speed Reader Test Mode Timer (0x95)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void TestConvertHighSpeedTestModeTimerToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x2, 0x92, new byte[] {0x1, 0x4}, 0x99);
			
			var highSpeedTestTimer = new HighSpeedTestModeTimer
			{
				DeviceType = HighSpeedTestDeviceType.GPIO,
				D1D0 = 0x401
			};
			var commandData = _interpretator.ConvertHighSpeedTestModeTimerToCommandData(highSpeedTestTimer);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get High Speed GPIO Test Mode Timer (0x93) command
		/// </summary>
		[TestMethod]
		public void TestGetHighSpeedGpioTestModeTimerCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x93, new byte[0x0], 0x93);
			var commandData = _interpretator.GetHighSpeedGpioTestModeTimerCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Get High Speed Reader Test Mode Timer (0x96) command
		/// </summary>
		[TestMethod]
		public void TestGetHighSpeedReaderTestModeTimerCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x0, 0x96, new byte[0x0], 0x96);
			var commandData = _interpretator.GetHighSpeedReaderTestModeTimerCommandData();

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Common method for set high speed test mode timer responses. Create CommandData for responses:
		/// <list type="bullet">
		/// <item><description>Get High Speed GPIO Test Mode Timer (0x93)</description></item>
		/// <item><description>Get High Speed Reader Test Mode Timer (0x96)</description></item>
		/// </list>
		/// </summary>
		[TestMethod]
		public void ConvertResponseDataToHighSpeedTestModeTimer()
		{
			var deviceResultExpected = new GeneralDeviceResult
			{
				Status = StatusCode.BOOTLOADER_MODE,
				Timer = 0x5
			};
			var deviceResExpectation = new ResponseExpectation(deviceResultExpected);

			var testModeTimerExpected = new HighSpeedTestModeTimer
			{
				DeviceType = HighSpeedTestDeviceType.GPIO,
				D1D0 = 0x400
			};
			var resultExpectation = new ResponseExpectation(testModeTimerExpected);

			var responseData = new ResponseData
			{
				LEN = 0x2,
				CMD = 0x93,
				STATUS = 0x4,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[] {0x0, 0x4}
			};
			var actualResult = _interpretator.ConvertResponseDataToGeneralDeviceResult(responseData);
			var actualTestModeTimner = _interpretator.ConvertResponseDataToHighSpeedTestModeTimer(responseData);

			Assert.IsTrue(deviceResExpectation.Equals(actualResult));
			Assert.IsTrue(resultExpectation.Equals(actualTestModeTimner));
		}

		/// <summary>
		/// High Speed Reader Test Mode Streaming (Packet Sent Asynchronously at Reader Test Mode Timer Interval) (0x94)
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToHighSpeedReaderTestResult()
		{
			var expectedResult = new HighSpeedReaderTestResult
			{
				Status = StatusCode.FAILED,
				Timer = 0x5,
				Sensor0Count = 0x4,
				SensorACount = 0x7,
				DeviceID = new byte[] { 0x1, 0x5, 0x3, 0x12, 0x32, 0x6 },
				TID = new byte[] {0x1, 0x3, 0x4, 0x6, 0x8, 0x8, 0x1, 0x3, 0x4, 0x6, 0xA, 0xE},
				EPC = new byte[] { 0x1, 0x3, 0x4, 0x6, 0x8, 0x8, 0x3, 0x4, 0x6, 0x7, 0xC, 0xF },
				Result = 0x6
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x27,
				CMD = 0x94,
				STATUS = 0x1,
				TMR_HI = 0x5,
				TMR_LO = 0x0,
				DATA = new byte[]
				{
					0x4, 0x0, 0x0, 0x0, 0x7, 0x0, 0x0, 0x0, 0x1, 0x5, 0x3, 0x12, 0x32, 0x6,
					0x1, 0x3, 0x4, 0x6, 0x8, 0x8, 0x1, 0x3, 0x4, 0x6, 0xA, 0xE,
					0x1, 0x3, 0x4, 0x6, 0x8, 0x8, 0x3, 0x4, 0x6, 0x7, 0xC, 0xF, 0x6
				}
			};
			var actualResult = _interpretator.ConvertResponseDataToHighSpeedReaderTestResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// Calibrate (0x97) command
		/// </summary>
		[TestMethod]
		public void TestConvertCalibrateSettingsToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x3, 0x97, new byte[]{0x2, 0x0, 0x4}, 0xA0);
			
			var calibrateSettings = new CalibrateSettings
			{
				Mode = CalibrateMode.Abort,
				Pwr = 0x400
			};
			var commandData = _interpretator.ConvertCalibrateSettingsToCommandData(calibrateSettings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// Calibrate (0x97) response
		/// </summary>
		[TestMethod]
		public void TestConvertResponseDataToCalibrateResult()
		{
			var expectedResult = new CalibrateResult
			{
				Status = StatusCode.INVALID_CAL_SETTING,
				Timer = 0x6,
				Channel = 0x2
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x1,
				CMD = 0x97,
				STATUS = 0x3F,
				TMR_HI = 0x6,
				TMR_LO = 0x0,
				DATA = new byte[] {0x2}
			};
			var actualResult = _interpretator.ConvertResponseDataToCalibrateResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}

		/// <summary>
		/// RFPowerMeter (0x98) command
		/// </summary>
		public void TestConvertRfPowerMeterToCommandData()
		{
			var expectation = new CommandExpectation(0xEE, 0x2, 0x98, new byte[]{0x1, 0x3}, 0x9B);
			
			var settings = new RFPowerMeterSettings {Pwr = 0x301};
			var commandData = _interpretator.ConvertRfPowerMeterToCommandData(settings);

			Assert.IsTrue(expectation.Equals(commandData));
		}

		/// <summary>
		/// RFPowerMeter (0x98) response
		/// </summary>
		public void ConvertResponseDataToRfPowerMeterResult()
		{
			var expectedResult = new RFPowerMeterResult
			{
				Status = StatusCode.FAILED,
				Timer = 0x3,
				Request = PowerMeterRequest.RefreshSendOne,
				Channel = 0x6,
				Averages = 0x7
			};
			var expectation = new ResponseExpectation(expectedResult);

			var responseData = new ResponseData
			{
				LEN = 0x3,
				CMD = 0x98,
				STATUS = 0x1,
				TMR_HI = 0x3,
				TMR_LO = 0x0,
				DATA = new byte[] {0x1, 0x6, 0x7}
			};
			var actualResult = _interpretator.ConvertResponseDataToRfPowerMeterResult(responseData);

			Assert.IsTrue(expectation.Equals(actualResult));
		}
	}
}