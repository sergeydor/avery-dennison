using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain.Conveyor;
using Common.Domain.Device;
using Common.Enums;
using Common.Enums.UnsolicitedReplyCommands;
using Simulator.Svc.Context;
using Simulator.Svc.Helpers;
using Simulator.Svc.Infrastructure;
using Simulator.Svc.Infrastructure.Devices;
using SoftHIDReceiver;
using Server.Device.Communication.Domain;

namespace Simulator.Svc.Services
{
	public class DeviceProcessingService
	{
		// debug
		private List<string> _list = new List<string>();
		private readonly DateTime _originalDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		// debug

		private readonly DeviceProcessingContext _context;
		private SimulatorSettings _simulatorSettings;
        private CancellationTokenSource _cancellationTokenSource = null;
        private Task _testRunningTask = null;

        public DeviceProcessingService(DeviceProcessingContext context)
		{
			_context = context;
		}

        public void StopTest()
        {
            if (_cancellationTokenSource != null)
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
            }

            Task whileTestTaskIsRunning = Task.Run(() =>
            {
                if (_testRunningTask != null)
                    while (_testRunningTask.Status == TaskStatus.Running) ;
            });
            int res = Task.WaitAny(new Task[] { whileTestTaskIsRunning }, _simulatorSettings.PredicatedConveyorMoveTimeMs + 500);

            if (res == -1) // => timout
            {
                Console.WriteLine("stop error - cmmands generation is still running " + _testRunningTask.Status);
                throw new TimeoutException("generation is still running");
            }
            Console.WriteLine("Generation thread stopped succesfully");

            // not reset 

            foreach (var reader in _context.ReadersListCopy)
            {                
                reader.DeviceInstance.Reset();
                Console.WriteLine("reader reset() ");
            }

            _context.GPIO.DeviceInstance.Reset();
            Console.WriteLine("gpio reset()");
        }
        
		public void ListenToDevices()
		{
			DeviceContainer.Container.StartProcessing();

			while (true)
			{
				Array input = null;

				input = _context.GPIO.DeviceInstance.GetInputData();
				if (input != null)
				{
					ProcessCommand(input, _context.GPIO);
				}

				foreach (var reader in _context.ReadersListCopy)
				{
					input = reader.DeviceInstance.GetInputData();
					if (input == null)
					{
						continue;
					}

					ProcessCommand(input, reader);
				}

				if (input == null)
				{
					Task.Delay(200);
				}
			}
		}

        private void ProcessCommand(Array input, DeviceBase deviceWrapper)
        {
            byte commandId = (byte)input.GetValue(2);
            HIDDevice hidDevice = deviceWrapper.DeviceInstance;

            byte[] responseData;
            if (commandId == 0x71)
            {
                responseData = new byte[SimulatorConstants.BytesArrayLength];
                Array macAddress = hidDevice.GetMACAddress();
                Array.Copy(macAddress, 0, responseData, 6, macAddress.Length);
                SendResponse(responseData, hidDevice);
                return;
            }

            CommandPair commandPair = CommandPairsList.GetCommandPair(commandId);

            if (commandPair == null)  // handle NON get-set commands here
            {
                if (commandId == 0x90) // High Speed GPIO Test Mode (0x90)
                {
                    responseData = GetResponseData(commandId, 0x01);
                    responseData[6] = 0x01; // 1 = In high speed mode, STATUS = 55
                    responseData[3] = 0x55;
                    byte fcs = (byte)responseData.Where((x, index) => index != 0 && index != 3).Sum(x => x); // I suppose 3 not 5 ??
                    responseData[1 + 6] = fcs;
                    SendResponse(responseData, hidDevice);
                }
                else if(commandId == 0x91) // Exit High Speed GPIO/Reader Test Mode (0x91) -- stop testing
                {
                    responseData = GetResponseData(commandId, 0x01);
                    responseData[6] = 0x01; // 1 = In high speed mode, STATUS = 55
                    byte fcs = (byte)responseData.Where((x, index) => index != 0 && index != 3).Sum(x => x); // I suppose 3 not 5 ??
                    responseData[1 + 6] = fcs;
                    SendResponse(responseData, hidDevice);
                    Console.WriteLine("Sent command 0x91 -- Exit High Speed GPIO/Reader Test Mode");
                }
                else
                {
                    responseData = GetResponseData(commandId, 0x0); // getting empty response (no data bytes)
                    SendResponse(responseData, hidDevice);

                    if (commandId == 0x51)
                    {
                        Console.WriteLine("Sent command 0x51 -- Stop Test for reader");
                    }
                }

                if (commandId == 0x90)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    _testRunningTask = Task.Run(() => MoveConveyor(hidDevice, _cancellationTokenSource.Token), _cancellationTokenSource.Token);                    
                }
                if (commandId == 0x91) // Exit High Speed GPIO/Reader Test Mode (0x91) -- stop testing
                {
                    //if(_cancellationTokenSource != null)
                    //{
                    //    _cancellationTokenSource.Cancel();                        
                    //    _cancellationTokenSource = null;
                    //}
                    //hidDevice.Reset();
                }
                if(commandId == 0x51)
                {
                    //hidDevice.Reset();
                }

                return;
            }

            if (commandPair.IsSet) // handle get-set commands here
            {
                deviceWrapper.SetSettingsData(input, commandPair);
                responseData = GetResponseData(commandId, 0x0);
            }
            else // get 
            {
                byte[] data = deviceWrapper.GetSettingsData(commandId);
                responseData = GetResponseData(commandId, (byte)data.Length);
                data.CopyTo(responseData, 6);
                //byte fcs = (byte)responseData.Where((x, index) => index != 0 && index != 5).Sum(x => x); // I suppose 3 not 5 ??
                byte fcs = (byte)responseData.Where((x, index) => index != 0 && index != 3).Sum(x => x); // I suppose 3 not 5 ??
                responseData[data.Length + 6] = fcs;
            }

            SendResponse(responseData, hidDevice);
        }

        private void MoveConveyor(HIDDevice device, CancellationToken cancelToken)
		{
			var wakeUpTimes = new Queue<TimeSpan>();
			_simulatorSettings = SimulatorSettingsSingleton.GetInstance();

			uint triggersCounter = 1;
			uint readerTriggersCounter = 1;
            double velocityInMMPerMSec = _simulatorSettings.VelocityTagsPerMSec * (_simulatorSettings.TagLengthInMm + _simulatorSettings.DistanceBetweenTagsInMm);
            double scannerStepInMm = _simulatorSettings.TagLengthInMm / _simulatorSettings.EncoderStepsPerTag;
            double timeForDistanceBetweenTagsInMSec = (_simulatorSettings.DistanceBetweenTagsInMm + _simulatorSettings.TagLengthInMm) / velocityInMMPerMSec;
			
			DateTime testStartTime = DateTime.Now;
			DateTime utcTestStartTime = testStartTime.ToUniversalTime().AddMilliseconds(1000);
			DateTime originalDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			DateTime nextTriggerStartTime = testStartTime.AddMilliseconds(1000);

	        int tagNumberStartSpeedDown = _simulatorSettings.TestTagsNumber - _context.SpeedDownTagsCount;

			var tagsCountBeforeSleep = (int)(_simulatorSettings.PredicatedConveyorMoveTimeMs / timeForDistanceBetweenTagsInMSec);
			int algorithmIterations = (int)Math.Ceiling((double)_simulatorSettings.TestTagsNumber / tagsCountBeforeSleep);
			int tagNumBerforeSleep = 1;

			var random = new Random();

			int badTagsNumber = (int)Math.Ceiling(_simulatorSettings.TestTagsNumber * ((double)_context.BadTagsPercent / (double)100));
	        int badTagsCounter = 0;
			string[] badTagsStatuses = Enum.GetNames(typeof(StatusCode)).Except(new[] { StatusCode.OK.ToString(), StatusCode.FAILED.ToString(), StatusCode.HIGH_SPEED_STREAMING_MODE.ToString() }).ToArray();

	        int missedTagsNumber = (int) Math.Ceiling(_simulatorSettings.TestTagsNumber*((double) _context.MissedTagsPercent/100));
	        int missedTagsCounter = 0;

			TimeSpan lastAddedTime = DateTime.Now.TimeOfDay;
			for (int i = 0; i < algorithmIterations; i++)
			{
				TimeSpan timeToAdd = i == 0
					? TimeSpan.FromMilliseconds((int)(0.8 * _simulatorSettings.PredicatedConveyorMoveTimeMs))
					: TimeSpan.FromMilliseconds(_simulatorSettings.PredicatedConveyorMoveTimeMs);

				lastAddedTime = lastAddedTime.Add(timeToAdd);
				wakeUpTimes.Enqueue(lastAddedTime);
			}

			double velocityIncrementValue = velocityInMMPerMSec / (_context.SpeedUpTagsCount + 1);
			double velocityDecrementValue = velocityInMMPerMSec / (_context.SpeedDownTagsCount + 1);
			velocityInMMPerMSec = velocityIncrementValue;
			int scanerTriggerTimeInMsec = (int)(scannerStepInMm / velocityInMMPerMSec);
			timeForDistanceBetweenTagsInMSec = (_simulatorSettings.DistanceBetweenTagsInMm + _simulatorSettings.TagLengthInMm) / velocityInMMPerMSec;

			while (true)
			{
                if (cancelToken.IsCancellationRequested)
                {
                    Console.WriteLine("cancelToken.IsCancellationRequested");
                    return;
                }

				// generate a batch of encoder triggers for the tag           
				DateTime startTriggerTime = nextTriggerStartTime;

				for (int encoderStep = 1; encoderStep <= _simulatorSettings.EncoderStepsPerTag; encoderStep++)
				{
					DateTime currentStepTime = startTriggerTime.AddMilliseconds(scanerTriggerTimeInMsec * encoderStep);
					double timer = (currentStepTime - testStartTime).TotalMilliseconds;
					//ushort tmrHiLo = timer > ushort.MaxValue ? (ushort)(timer - ushort.MaxValue) : (ushort)timer;

					// 0x90 response filling

					// get MAC address of asked device
					Array macAddress = device.GetMACAddress().Cast<byte>().Reverse().ToArray();

					byte len = 0xE;
					byte cmd = 0x90;

					var output = new byte[0x40];
					output[0] = 0xEE; // SOF
					output[1] = len; //LEN
					output[2] = cmd; // CMD code
					output[3] = 0x55; // STATUS

					//var tmrHiLoBytes = BitConverter.GetBytes(tmrHiLo);
					byte tmrHi = 0x0;
					byte tmrLo = 0x0;
					output[4] = tmrHi; //tmrHiLoBytes[0];
					output[5] = tmrLo; //tmrHiLoBytes[1];

					// Encoder 0 Count bytes
					byte[] encoder0CountBytes = BitConverter.GetBytes(triggersCounter);
					Array.Reverse(encoder0CountBytes);
					Array.Copy(encoder0CountBytes, 0, output, 6, encoder0CountBytes.Length);

					float encoderACount = (float)((currentStepTime - startTriggerTime).TotalMilliseconds * velocityInMMPerMSec);

					// Encoder A Count btyes
					byte[] encoderACountBytes = BitConverter.GetBytes(encoderACount);
					Array.Reverse(encoderACountBytes);
					Array.Copy(encoderACountBytes, 0, output, 10, encoderACountBytes.Length);

					// MAC address bytes (Device ID in HEX view)
					Array.Copy(macAddress, 0, output, 14, macAddress.Length);
					
					int dataSum = output.Where((x, index) => index >= 6).Sum(x => x);
					byte fcs = (byte)(len + cmd + tmrHi + tmrLo + dataSum);

					output[20] = fcs;

					// 0x90 response filling

					DateTime utcStepTime = utcTestStartTime.AddMilliseconds(timer);
					long timeToPost = (long)(utcStepTime - originalDateTime).TotalMilliseconds;
					if (timeToPost < 0)
					{
						throw new Exception("Time to post is less than zero");
					}
					var array = new object[output.Length];
					output.CopyTo(array, 0);
					device.ScheduleOutMessage(array, timeToPost);
					
					if (encoderStep == 1 && _simulatorSettings.EncoderReaderTagsDistance == triggersCounter - readerTriggersCounter)
					{
                        int count = _context.ReadersListCopy.Count;
                        var rlist = _context.ReadersListCopy;

                        for (int i = 0; i < count; i++)
						{
							var reader = rlist[i];
							bool isBadTag = false;
							bool isMissedTag = false;
							int randomValue = random.Next(100);

							if (badTagsCounter < badTagsNumber)
							{
								isBadTag = randomValue <= _context.BadTagsPercent;
								if (isBadTag)
								{
									badTagsCounter++;
								}
							}

							if (missedTagsCounter < missedTagsNumber)
							{
								isMissedTag = randomValue > _context.BadTagsPercent && randomValue <= (_context.BadTagsPercent + _context.MissedTagsPercent);
								if (isMissedTag)
								{
									missedTagsCounter++;
								}
							}

							object[] readerArray = GenerateReaderE3Response(readerTriggersCounter, isBadTag, badTagsStatuses, isMissedTag);
							var readerTime = timeToPost + i * 3;
							reader.DeviceInstance.ScheduleOutMessage(readerArray, readerTime);
						}
						readerTriggersCounter++;
					}

					// debug
					_list.Add(timeToPost.ToString());
					// debug
				}
				nextTriggerStartTime = startTriggerTime.AddMilliseconds(timeForDistanceBetweenTagsInMSec);

				if (triggersCounter == _simulatorSettings.TestTagsNumber)
				{
					break;
				}
				triggersCounter++; // the next tag is moving soon
				// speed is being up
				if (triggersCounter <= _context.SpeedUpTagsCount + 1)
				{
					velocityInMMPerMSec += velocityIncrementValue;
					timeForDistanceBetweenTagsInMSec = (_simulatorSettings.DistanceBetweenTagsInMm + _simulatorSettings.TagLengthInMm) / velocityInMMPerMSec;
					scanerTriggerTimeInMsec = (int)(scannerStepInMm / velocityInMMPerMSec);
				}
				// speed is being down
				if (triggersCounter > tagNumberStartSpeedDown)
				{
					velocityInMMPerMSec -= velocityDecrementValue;
					timeForDistanceBetweenTagsInMSec = (_simulatorSettings.DistanceBetweenTagsInMm + _simulatorSettings.TagLengthInMm) / velocityInMMPerMSec;
					scanerTriggerTimeInMsec = (int)(scannerStepInMm / velocityInMMPerMSec);
				}

				if (tagNumBerforeSleep == tagsCountBeforeSleep)
				{
					tagNumBerforeSleep = 1;

					TimeSpan wakeUp = wakeUpTimes.Dequeue();

					var sleepTs = wakeUp - DateTime.Now.TimeOfDay;
                    if (sleepTs.TotalMilliseconds > 0)
                    {
                        if (cancelToken.IsCancellationRequested)
                        {
                            Console.WriteLine("cancelToken.IsCancellationRequested");
                            return;
                        }
                        Thread.Sleep(sleepTs);
                    }
				}
				else
				{
					tagNumBerforeSleep++;
				}
			}

			// debug
			var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logtest.txt");
			using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
			{
				fs.SetLength(0);
				using (var writer = new StreamWriter(fs))
				{
					foreach (var item in _list)
					{
						writer.WriteLine(item);
					}
				}
			}

			_list = new List<string>();
			// debug
		}

		public void ExcludeDevices(List<DeviceIdentity> devices)
		{
			var gpioArray = (object[])_context.GPIO.DeviceInstance.GetMACAddress();
			byte[] gpioMacAddressBytes = new byte[gpioArray.Length];
			for (int i = 0; i < gpioArray.Length; i++)
			{
				gpioMacAddressBytes[i] = (byte)gpioArray[i];
			}
			string gpioMacAddress = BitConverter.ToString(gpioMacAddressBytes).Replace("-", string.Empty);
			if (devices.Any(x => x.MacAddress == gpioMacAddress))
			{
				DeviceContainer.Container.RemoveDevice(_context.GPIO.DeviceInstance);
			}

			foreach (var reader in _context.ReadersListCopy)
			{
				var readerArray = (object[])reader.DeviceInstance.GetMACAddress();
				byte[] readerMacAddressBytes = new byte[readerArray.Length];
				for (int i = 0; i < readerArray.Length; i++)
				{
					readerMacAddressBytes[i] = (byte)readerArray[i];
				}
				string readerMacAddress = BitConverter.ToString(readerMacAddressBytes).Replace("-", string.Empty);
				if (devices.Any(x => x.MacAddress == readerMacAddress))
				{
					DeviceContainer.Container.RemoveDevice(reader.DeviceInstance);
					_context.RemoveReader(reader);
					reader.DeviceInstance.Destroy();
				}
			}
		}

		public void Reset()
		{
			DeviceContainer.Container.StopProcessing();

			_context.GPIO?.DeviceInstance.Reset();
			//if (_context.Readers != null)
			{
				foreach (var reader in _context.ReadersListCopy)
				{
					reader.DeviceInstance.Reset();
				}
			}

			DeviceContainer.Container.StartProcessing();
		}

		

		private void SendResponse(byte[] responseData, HIDDevice hidDevice)
		{
			var array = new object[responseData.Length];
			responseData.CopyTo(array, 0);

			// debug
			var ms = (long)(DateTime.UtcNow - _originalDateTime).TotalMilliseconds;
			_list.Add(ms.ToString());
			// debug

			hidDevice.ScheduleOutMessage(array, 0);
		}

		private byte[] GetResponseData(byte commandId, byte len)
		{
			var responseBytes = new byte[SimulatorConstants.BytesArrayLength];
			responseBytes[0] = 0xEE;
			responseBytes[1] = len;
			responseBytes[2] = commandId;
			responseBytes[3] = 0x0;
			responseBytes[4] = 0x0;
			responseBytes[5] = 0x0;
			if (len == 0x0)
			{
				responseBytes[6] = commandId;
			}

			return responseBytes;
		}

		private object[] GenerateReaderE3Response(uint triggersCounter, bool isBadTag, string[] badTagsStatuses, bool isMissedTag)
		{
			var random = new Random();

			byte len = 0x20;
			byte cmd = 0xE3;

            var output = new byte[0x40];

			// E3 command filling

			output[0] = 0xEE; // SOF
			output[1] = len; // LEN
			output[2] = cmd; // CMD
			
			StatusCode status;
			if (isBadTag)
			{
				string statusName = badTagsStatuses[random.Next(badTagsStatuses.Length)];
				status = (StatusCode) Enum.Parse(typeof (StatusCode), statusName);
			}
			else if (isMissedTag)
			{
				status = StatusCode.FAILED;
			}
			else
			{
				status = StatusCode.OK;
			}
			output[3] = (byte)status; // STATUS

			//var tmrHiLoBytes = BitConverter.GetBytes(tmrHiLo);
			byte tmrHi = 0x0;
			byte tmrLo = 0x0;
			output[4] = tmrHi; //tmrHiLoBytes[0];
			output[5] = tmrLo; //tmrHiLoBytes[1];

			// Test Type
			Array testTypeValues = Enum.GetValues(typeof (TestE3Type));
			TestE3Type testType = (TestE3Type)testTypeValues.GetValue(random.Next(testTypeValues.Length));
			output[6] = (byte)testType;

			// Test Status
			Array testStatusValues = Enum.GetValues(typeof (TestE3Status));
			TestE3Status testStatus = (TestE3Status) testStatusValues.GetValue(random.Next(testStatusValues.Length));
			output[7] = (byte)testStatus;

			// Test Count
			byte[] testCountBytes = BitConverter.GetBytes(triggersCounter);
			Array.Reverse(testCountBytes);
			Array.Copy(testCountBytes, 0, output, 8, testCountBytes.Length);

			// EPC Tag ID
			byte[] epcTagIdBytes = new byte[12];
			random.NextBytes(epcTagIdBytes);
			Array.Copy(epcTagIdBytes, 0, output, 12, epcTagIdBytes.Length);

			// Sensitivity
			ushort sensitivity = (ushort)random.Next(ushort.MinValue, ushort.MaxValue);
			byte[] sensitivityBytes = BitConverter.GetBytes(sensitivity);
			Array.Reverse(sensitivityBytes);
			Array.Copy(sensitivityBytes, 0, output, 24, sensitivityBytes.Length);

			// TID
			byte[] tidBytes = new byte[12];
			random.NextBytes(tidBytes);
			Array.Copy(tidBytes, 0, output, 26, tidBytes.Length);

			int dataSum = output.Where((x, index) => index >= 6).Sum(x => x);
			byte fcs = (byte)(len + cmd + tmrHi + tmrLo + dataSum);

			output[38] = fcs;

			// E3 command filling

			var array = new object[output.Length];
			output.CopyTo(array, 0);

			return array;
		}
	}
}