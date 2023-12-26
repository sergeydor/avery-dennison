using System;
using Common.Domain.DeviceResults.GSTCommands;
using Common.Domain.GSTCommands;
using Common.Enums.GSTCommands;

namespace Server.Device.Communication.CommandInterpretators
{
	public class GSTCommandsInterpretator : BaseInterpretator
	{
		/// <summary>
		/// Read Dip Switches (0x70) response
		/// </summary>
		public CommandData GetReadDipSwitchesCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x70
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Read Dip Switches (0x70) response
		/// </summary>
		public ReadDipSwitchesResult ConvertResponseDataToReadDipSwitchesResult(ResponseData responseData)
		{
			var result = new ReadDipSwitchesResult { Dip = responseData.DATA[0] };
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get MAC Address (0x71) and Get WiFi Module MAC Address (0x7C) commands
		/// </summary>
		public CommandData GetMACAddressCommandData(byte commandCode, byte macLength = 0x6)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = commandCode,
				DATA = new[] { macLength }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get MAC Address (0x71) and Get WiFi Module MAC Address (0x7C) responses
		/// </summary>
		public MACAddressResult ConvertResponseDataToMacAddressResult(ResponseData responseData)
		{
			var data = responseData.DATA;
			var result = new MACAddressResult
			{
				MACLength = data[0],
				Address5 = data[1],
				Address4 = data[2],
				Address3 = data[3],
				Address2 = data[4],
				Address1 = data[5],
				Address0 = data[6],
				CommandCode = responseData.CMD
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
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
		public CommandData GetDevicePowerSwitchCommandData(byte commandCode, PowerMode powerMode)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = commandCode,
				DATA = new[] { (byte)powerMode }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Common method for Set Devices Power responses. Creates DevicePowerSwitchResult object for responses:
		/// <item><description>0x72 (Set Barcode Reader Power On/Off)</description></item>
		/// <item><description>0x73 (Set RFID Reader Power On/Off)</description></item>
		/// <item><description>0x74 (Set Camera Power On/Off)</description></item>
		/// <item><description>0x75 (Set Motor Enable On/Off)</description></item>
		/// <item><description>0x78 (Set USB To Spare Serial Port Mirroring)</description></item>
		/// </summary>
		public DevicePowerSwitchResult ConvertResponseDataToDevicePowerSwitchResult(ResponseData responseData)
		{
			var result = new DevicePowerSwitchResult
			{
				CommandCode = responseData.CMD,
				PowerMode = (PowerMode)responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Set Motor Speed (0x76) command
		/// </summary>
		public CommandData ConvertMotoSpeedToCommandData(MotorSpeed motorSpeed)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = 0x76,
				DATA = new[] { motorSpeed.SpeedValue }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set Motor Speed (0x76) response
		/// </summary>
		public MotorSpeed ConvertResponseDataToMotorSpeed(ResponseData responseData)
		{
			var motorSpeed = new MotorSpeed { SpeedValue = responseData.DATA[0] };

			return motorSpeed;
		}

		/// <summary>
		/// Twinkle LED's (0x77) command
		/// </summary>
		public CommandData GetTwinkleLedCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x77
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Twinkle LED's (0x77) response
		/// </summary>
		public TwinkleLEDResult ConvertResponseDataToTwinkleLedResult(ResponseData responseData)
		{
			var result = new TwinkleLEDResult { LED = responseData.DATA[0] };
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get Camera Outputs (0x7B) command
		/// </summary>
		public CommandData GetCameraOutputCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x7B
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Camera Outputs (0x7B) response
		/// </summary>
		public CameraOutputsResult ConvertResponseDataToCameraOutputsResult(ResponseData responseData)
		{
			var result = new CameraOutputsResult { GPIO = responseData.DATA[0] };
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get WiFi Network ID (0x7D) and Get WiFI Network Security Key (0x7E) commands
		/// </summary>
		public CommandData GetWifiNetworkSettingsCommandData(byte commandCode, byte length)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = commandCode,
				DATA = new[] { length }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get WiFi Network ID (0x7D) and Get WiFI Network Security Key (0x7E) responses
		/// </summary>
		public WifiNetworkSettingsResult ConvertResponseDataToWifiNetworkSettingsResult(ResponseData responseData)
		{
			var ssid = new byte[0x20];
			Array.Copy(responseData.DATA, 1, ssid, 0, 0x20);
			var result = new WifiNetworkSettingsResult
			{
				CommandCode = responseData.CMD,
				Length = responseData.DATA[0],
				WifiSSID = ssid
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Common method for Set Devices Activity State commands. Creates CommandData object for commands:
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
		public CommandData GetDeviceActivityStateCommandData(byte commandCode, ActivityState activityState)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = commandCode,
				DATA = new[] { (byte)activityState }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Common method for Set Devices Activity State responses. Creates DeviceActivityStateResult object for responses:
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
		public DeviceActivityStateResult ConvertResponseDataToDeviceActivityStateResult(ResponseData responseData)
		{
			var result = new DeviceActivityStateResult
			{
				CommandCode = responseData.CMD,
				ActivityState = (ActivityState)responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
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
		public CommandData GetDeviceStatusCommandData(byte commandCode)
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = commandCode
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Common method for Get Devices Status responses. Creates DeviceStatusResult object for responses:
		/// <list type="bullet">
		/// <item><description>0x7F (Get Encoder A Status)</description></item>
		/// <item><description>0x80 (Get Encoder B Status)</description></item>
		/// <item><description>0x81 (Get Trigger Sensor 0 Status)</description></item>
		/// <item><description>0x82 (Get Trigger Sensor 1 Status)</description></item>
		/// <item><description>0x83 (Get Spare 1 Trigger Sensor Status)</description></item>
		/// <item><description>0x84 (Get Spare 2 Trigger Sensor Status)</description></item>
		/// </list>
		/// </summary>
		public DeviceStatusResult ConvertResponseDataToDeviceStatusResult(ResponseData responseData)
		{
			var result = new DeviceStatusResult
			{
				CommandCode = responseData.CMD,
				DeviceStatus = responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Set GPI0 RFID Input (0x85) command
		/// </summary>
		public CommandData GetGPI0RFIDInputCommandData(byte state)
		{
			var commandData = new CommandData
			{
				LEN = 0x1,
				CMD = 0x85,
				DATA = new[] { state }
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set GPI0 RFID Input (0x85) response
		/// </summary>
		public GPI0RFIDInputResult ConvertResponseDataToGPI0RFIDInputResult(ResponseData responseData)
		{
			var result = new GPI0RFIDInputResult
			{
				GPI0 = responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Set GPI1 RFID Input (0x86) command
		/// </summary>
		public CommandData GetGPI1RFIDInputCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x86
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Set GPI0 RFID Input (0x85) response
		/// </summary>
		public GPI1RFIDInputResult ConvertResponseDataToGPI1RFIDInputResult(ResponseData responseData)
		{
			var result = new GPI1RFIDInputResult
			{
				GPI1 = responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get GPIO RFID Reader Status (0x87) command
		/// </summary>
		public CommandData GetGpioRfidReaderStatusCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x87
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get GPIO RFID Reader Status (0x87) response
		/// </summary>
		public GpioRfidReaderStatusResult ConvertResponseDataToGpioRfidReaderStatusResult(ResponseData responseData)
		{
			var result = new GpioRfidReaderStatusResult { ReaderStatus = responseData.DATA[0] };
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get Board Temperature (0x88) command
		/// </summary>
		public CommandData GetBoardTemperatureCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x88
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Board Temperature (0x88) response
		/// </summary>
		public BoardTemperatureResult ConvertResponseDataToBoardTemperatureResult(ResponseData responseData)
		{
			var result = new BoardTemperatureResult { Temperature = BitConverter.ToInt16(responseData.DATA, 0) };
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Get Mark, Punch, Aux, Snap and Monarch Pause States (0x8E) command
		/// </summary>
		public CommandData GetDevicePauseStateCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x8E
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get Push Button Status (0x8F) command
		/// </summary>
		public CommandData GetPushButtonStatusCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x8F
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Common method for Get Mark, Punch, Aux, Snap and Monarch Pause States (0x8E) response and Get Push Button Status (0x8F) response
		/// </summary>
		public DeviceActionStatusResult ConvertResponseDataToDeviceActionStatusResult(ResponseData responseData)
		{
			var result = new DeviceActionStatusResult
			{
				ActionStatus = responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// High Speed GPIO Test Mode (0x90) command
		/// </summary>
		public CommandData GetHighSpeedGpioTestModeCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x90
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Exit High Speed GPIO/Reader Test Mode (0x91) command
		/// </summary>
		public CommandData GetExitHighSpeedTestModeCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x91
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// High Speed Reader Test Mode (0x94) command
		/// </summary>
		public CommandData GetHighSpeedReaderTestModeCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x94
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Common method for GPIO/Reader high speed test (start/exit). Creates DeviceHighSpeedTestResult object for responses:
		/// <list type="bullet">
		/// <item><description>High Speed GPIO Test Mode (0x90)</description></item>
		/// <item><description>Exit High Speed GPIO/Reader Test Mode (0x91)</description></item>
		/// <item><description>High Speed Reader Test Mode (0x94)</description></item>
		/// </list>
		/// </summary>
		public DeviceHighSpeedTestResult ConvertResponseDataToDeviceHighSpeedTestResult(ResponseData responseData)
		{
			HighSpeedTestDeviceType deviceType;
			if (responseData.CMD == 0x90 || responseData.CMD == 0x91)
			{
				deviceType = HighSpeedTestDeviceType.GPIO;
			}
			else if (responseData.CMD == 0x94)
			{
				deviceType = HighSpeedTestDeviceType.Reader;
			}
			else
			{
				deviceType = HighSpeedTestDeviceType.None;
			}

			var result = new DeviceHighSpeedTestResult
			{
				DeviceType = deviceType,
				IsExit = responseData.CMD == 0x91,
				Mode = responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// High Speed GPIO Test Mode Streaming (Packet Sent Asynchronously at GPIO Test Mode Timer Interval) (0x90)
		/// </summary>
		public HighSpeedGPIOTestResult ConvertResponseDataToHighSpeedGpioTestStreamingResult(
			ResponseData responseData)
		{
			var data = responseData.DATA;

			var result = new HighSpeedGPIOTestResult
			{
				Sensor0Count = BitConverter.ToUInt32(Normalize(data, 0, 4), 0),
				SensorACount = BitConverter.ToSingle(Normalize(data, 4, 4), 0),
				DeviceID = new byte[0x6]
			};
			Array.Copy(data, 8, result.DeviceID, 0, 0x6);
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

        

        /// <summary>
        /// Common method for set high speed test mode timer commands. Create CommandData for commands:
        /// <list type="bullet">
        /// <item><description>Set High Speed GPIO Test Mode Timer (0x92)</description></item>
        /// <item><description>Set High Speed Reader Test Mode Timer (0x95)</description></item>
        /// </list>
        /// </summary>
        public CommandData ConvertHighSpeedTestModeTimerToCommandData(HighSpeedTestModeTimer testTimer)
		{
			byte cmd;
			switch (testTimer.DeviceType)
			{
				case HighSpeedTestDeviceType.GPIO:
					cmd = 0x92;
					break;
				case HighSpeedTestDeviceType.Reader:
					cmd = 0x95;
					break;
				default:
					throw new NotSupportedException($"Device type {testTimer.DeviceType} is not supported");
			}
            var commandData = new CommandData
            {
                LEN = 0x2,
                CMD = cmd,
                DATA = Normalize(BitConverter.GetBytes(testTimer.D1D0), 0, 2)
			};
            
            CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get High Speed GPIO Test Mode Timer (0x93) command
		/// </summary>
		public CommandData GetHighSpeedGpioTestModeTimerCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x93
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Get High Speed Reader Test Mode Timer (0x96) command
		/// </summary>
		public CommandData GetHighSpeedReaderTestModeTimerCommandData()
		{
			var commandData = new CommandData
			{
				LEN = 0x0,
				CMD = 0x96
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Common method for set high speed test mode timer responses. Create CommandData for responses:
		/// <list type="bullet">
		/// <item><description>Get High Speed GPIO Test Mode Timer (0x93)</description></item>
		/// <item><description>Get High Speed Reader Test Mode Timer (0x96)</description></item>
		/// </list>
		/// </summary>
		public HighSpeedTestModeTimer ConvertResponseDataToHighSpeedTestModeTimer(ResponseData responseData)
		{
			HighSpeedTestDeviceType deviceType;
			if (responseData.CMD == 0x93)
			{
				deviceType = HighSpeedTestDeviceType.GPIO;
			}
			else if (responseData.CMD == 0x96)
			{
				deviceType = HighSpeedTestDeviceType.Reader;
			}
			else
			{
				throw new NotSupportedException($"Not supported command {responseData.CMD}");
			}

			var result = new HighSpeedTestModeTimer
			{
				DeviceType = deviceType,
				D1D0 = BitConverter.ToUInt16(Normalize(responseData.DATA,0,2), 0)
			};

			return result;
		}

		/// <summary>
		/// High Speed Reader Test Mode Streaming (Packet Sent Asynchronously at Reader Test Mode Timer Interval) (0x94)
		/// </summary>
		public HighSpeedReaderTestResult ConvertResponseDataToHighSpeedReaderTestResult(ResponseData responseData)
		{
			var data = responseData.DATA;
			var result = new HighSpeedReaderTestResult
			{
				Sensor0Count = BitConverter.ToUInt32(data, 0),
				SensorACount = BitConverter.ToUInt32(data, 4),
				DeviceID = new byte[0x6],
				TID = new byte[0xC],
				EPC = new byte[0xC],
				Result = responseData.DATA[38]
			};
			Array.Copy(data, 8, result.DeviceID, 0, 0x6);
			Array.Copy(data, 14, result.TID, 0, 0xC);
			Array.Copy(data, 26, result.EPC, 0, 0xC);
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// Calibrate (0x97) command
		/// </summary>
		public CommandData ConvertCalibrateSettingsToCommandData(CalibrateSettings calibrateSettings)
		{
			var commandData = new CommandData
			{
				LEN = 0x3,
				CMD = 0x97,
				DATA = new byte[0x3]
			};
			commandData.DATA[0] = (byte)calibrateSettings.Mode;
			BitConverter.GetBytes(calibrateSettings.Pwr).CopyTo(commandData.DATA, 1);
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// Calibrate (0x97) response
		/// </summary>
		public CalibrateResult ConvertResponseDataToCalibrateResult(ResponseData responseData)
		{
			var result = new CalibrateResult
			{
				Channel = responseData.DATA[0]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}

		/// <summary>
		/// RFPowerMeter (0x98) command
		/// </summary>
		public CommandData ConvertRfPowerMeterToCommandData(RFPowerMeterSettings settings)
		{
			var commandData = new CommandData
			{
				LEN = 0x2,
				CMD = 0x98,
				DATA = BitConverter.GetBytes(settings.Pwr)
			};
			CalculateFcs(commandData);

			return commandData;
		}

		/// <summary>
		/// RFPowerMeter (0x98) response
		/// </summary>
		public RFPowerMeterResult ConvertResponseDataToRfPowerMeterResult(ResponseData responseData)
		{
			var result = new RFPowerMeterResult
			{
				Request = (PowerMeterRequest)responseData.DATA[0],
				Channel = responseData.DATA[1],
				Averages = responseData.DATA[2]
			};
			CreateGeneralDeviceResult(result, responseData);

			return result;
		}
	}
}