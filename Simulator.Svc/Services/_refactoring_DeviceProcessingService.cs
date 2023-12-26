using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Domain.Conveyor;
using Common.Domain.Device;
using Common.Enums.UnsolicitedReplyCommands;
using Simulator.Svc.Context;
using Simulator.Svc.Helpers;
using Simulator.Svc.Infrastructure;
using Simulator.Svc.Infrastructure.Devices;
using SoftHIDReceiver;
using Server.Device.Communication.Domain;

namespace Simulator.Svc.Services
{
    public class DeviceProcessingService_refactoring
    {
        // debug
        //private List<string> _list = new List<string>();
        //private readonly DateTime _originalDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        // debug

        private readonly DeviceProcessingContext _context;
        private SimulatorSettings _simulatorSettings;
        private CancellationTokenSource _cancellationTokenSource = null;

        public DeviceProcessingService_refactoring(DeviceProcessingContext context)
        {
            _context = context;
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
                byte len = 0x7;
                responseData = GetResponseData(commandId, len);
                Array macAddress = hidDevice.GetMACAddress();
                Array.Copy(macAddress, 0, responseData, 6, macAddress.Length);
                byte fcs = CalculateChecksum(responseData, len);
                responseData[len + 6] = fcs;
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
                    byte fcs = CalculateChecksum(responseData, 1);
                    responseData[1 + 6] = fcs;
                    SendResponse(responseData, hidDevice);
                }
                else if (commandId == 0x91) // Exit High Speed GPIO/Reader Test Mode (0x91) -- stop testing
                {
                    responseData = GetResponseData(commandId, 0x01);
                    responseData[6] = 0x01; // 1 = In high speed mode, STATUS = 55
                    byte fcs = CalculateChecksum(responseData, 1);
                    responseData[1 + 6] = fcs;
                    SendResponse(responseData, hidDevice);
                }
                else
                {
                    responseData = GetResponseData(commandId, 0x0); // getting empty response (no data bytes)
                    SendResponse(responseData, hidDevice);
                }

                if (commandId == 0x90)
                {
                    _cancellationTokenSource = new CancellationTokenSource();
                    Task.Run(() => MoveConveyor(hidDevice, _cancellationTokenSource.Token), _cancellationTokenSource.Token);
                }
                if (commandId == 0x91) // Exit High Speed GPIO/Reader Test Mode (0x91) -- stop testing
                {
                    if (_cancellationTokenSource != null)
                    {
                        _cancellationTokenSource.Cancel();
                        _cancellationTokenSource = null;
                    }
                    hidDevice.Reset();
                }
                if (commandId == 0x51)
                {
                    hidDevice.Reset();
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
                byte fcs = CalculateChecksum(responseData, (byte)data.Length); // I suppose 3 not 5 ??
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
            int scanerTriggerTimeInMsec = (int)(scannerStepInMm / velocityInMMPerMSec);
            //long scanerTriggerTimeInTicks = (long)(scanerTriggerTimeInMsec * 1000 * 10);
            double timeForDistanceBetweenTagsInMSec = (_simulatorSettings.DistanceBetweenTagsInMm + _simulatorSettings.TagLengthInMm) / velocityInMMPerMSec;

            DateTime testStartTime = DateTime.Now;
            DateTime utcTestStartTime = testStartTime.ToUniversalTime().AddMilliseconds(1000);
            DateTime originalDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime nextTriggerStartTime = testStartTime.AddMilliseconds(1000);

            var tagsCountBeforeSleep = (int)(_simulatorSettings.PredicatedConveyorMoveTimeMs / timeForDistanceBetweenTagsInMSec);
            int algorithmIterations = (int)Math.Ceiling((double)_simulatorSettings.TestTagsNumber / tagsCountBeforeSleep);
            int tagNumBerforeSleep = 1;

            TimeSpan lastAddedTime = DateTime.Now.TimeOfDay;
            for (int i = 0; i < algorithmIterations; i++)
            {
                TimeSpan timeToAdd = i == 0
                    ? TimeSpan.FromMilliseconds((int)(0.8 * _simulatorSettings.PredicatedConveyorMoveTimeMs))
                    : TimeSpan.FromMilliseconds(_simulatorSettings.PredicatedConveyorMoveTimeMs);

                lastAddedTime = lastAddedTime.Add(timeToAdd);
                wakeUpTimes.Enqueue(lastAddedTime);
            }

            while (true)
            {
                if (cancelToken.IsCancellationRequested)
                    return;

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

                    float encoderACount = (float)((currentStepTime - startTriggerTime).TotalMilliseconds * velocityInMMPerMSec);

                    // Encoder 0 Count bytes
                    byte[] encoder0CountBytes = BitConverter.GetBytes(triggersCounter);
                    Array.Reverse(encoder0CountBytes);
                    Array.Copy(encoder0CountBytes, 0, output, 6, encoder0CountBytes.Length);

                    // Encoder A Count btyes
                    byte[] encoderACountBytes = BitConverter.GetBytes(encoderACount);
                    Array.Reverse(encoderACountBytes);
                    Array.Copy(encoderACountBytes, 0, output, 10, encoderACountBytes.Length);

                    // MAC address bytes (Device ID in HEX view)
                    Array.Copy(macAddress, 0, output, 14, macAddress.Length);

                    byte fcs = CalculateChecksum(output, len);

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
                        for (int i = 0; i < count; i++)
                        {
                            var reader = _context.ReadersListCopy[i];
                            object[] readerArray = GenerateReaderE3Response(readerTriggersCounter);
                            var readerTime = timeToPost + i * 3;
                            reader.DeviceInstance.ScheduleOutMessage(readerArray, readerTime);
                        }
                        readerTriggersCounter++;
                    }

                    // debug
                    //_list.Add(timeToPost.ToString());
                    // debug
                }
                nextTriggerStartTime = startTriggerTime.AddMilliseconds(timeForDistanceBetweenTagsInMSec);

                if (triggersCounter == _simulatorSettings.TestTagsNumber)
                {
                    while (readerTriggersCounter <= triggersCounter)
                    {
                        if (cancelToken.IsCancellationRequested)
                            return;

                        DateTime currentStepTime = nextTriggerStartTime.AddMilliseconds(scanerTriggerTimeInMsec);
                        double timer = (currentStepTime - testStartTime).TotalMilliseconds;

                        DateTime utcStepTime = utcTestStartTime.AddMilliseconds(timer);
                        long timeToPost = (long)(utcStepTime - originalDateTime).TotalMilliseconds;
                        if (timeToPost < 0)
                        {
                            throw new Exception("Time to post is less than zero");
                        }

                        int count = _context.ReadersListCopy.Count;
                        for (int i = 0; i < count; i++)
                        {
                            var reader = _context.ReadersListCopy[i];
                            object[] readerArray = GenerateReaderE3Response(readerTriggersCounter);
                            var readerTime = timeToPost + i * 3;
                            reader.DeviceInstance.ScheduleOutMessage(readerArray, readerTime);
                        }
                        nextTriggerStartTime = nextTriggerStartTime.AddMilliseconds(timeForDistanceBetweenTagsInMSec);

                        readerTriggersCounter++;

                        // debug
                        //_list.Add(timeToPost.ToString());
                        // debug
                    }
                    break;
                }
                triggersCounter++; // the next tag is moving soon

                if (tagNumBerforeSleep == tagsCountBeforeSleep)
                {
                    tagNumBerforeSleep = 1;

                    TimeSpan wakeUp = wakeUpTimes.Dequeue();

                    var sleepTs = wakeUp - DateTime.Now.TimeOfDay;
                    if (sleepTs.TotalMilliseconds > 0)
                    {
                        if (cancelToken.IsCancellationRequested)
                            return;
                        Thread.Sleep(sleepTs);
                    }
                }
                else
                {
                    tagNumBerforeSleep++;
                }
            }

            // debug
            //var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logtest.txt");
            //using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            //{
            //	fs.SetLength(0);
            //	using (var writer = new StreamWriter(fs))
            //	{
            //		foreach (var item in _list)
            //		{
            //			writer.WriteLine(item);
            //		}
            //	}
            //}

            //_list = new List<string>();
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
            //if (_context.Readers != null)  not possible
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
            //var ms = (long)(DateTime.UtcNow - _originalDateTime).TotalMilliseconds;
            //_list.Add(ms.ToString());
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

            byte fcs = CalculateChecksum(responseBytes, len);
            responseBytes[len + 6] = fcs;

            return responseBytes;
        }

        private object[] GenerateReaderE3Response(uint triggersCounter)
        {
            byte len = 0x20;
            byte cmd = 0xE3;

            var output = new byte[0x40];

            // E3 command filling

            output[0] = 0xEE; // SOF
            output[1] = len; // LEN
            output[2] = cmd; // CMD
            output[3] = 0x0; // STATUS

            //var tmrHiLoBytes = BitConverter.GetBytes(tmrHiLo);
            byte tmrHi = 0x0;
            byte tmrLo = 0x0;
            output[4] = tmrHi; //tmrHiLoBytes[0];
            output[5] = tmrLo; //tmrHiLoBytes[1];

            // Test Type
            output[6] = 0x0; //(byte)TestE3Type.ReadTest;

            // Test Status
            output[7] = 0x0; //(byte)TestE3Status.RFU;

            // Test Count
            byte[] testCountBytes = BitConverter.GetBytes(triggersCounter);
            Array.Reverse(testCountBytes);
            Array.Copy(testCountBytes, 0, output, 8, testCountBytes.Length);

            // EPC Tag ID
            byte[] epcTagIdBytes = new byte[12];
            Array.Copy(epcTagIdBytes, 0, output, 12, epcTagIdBytes.Length);

            // Sensitivity
            ushort sensitivity = 0x0;
            byte[] sensitivityBytes = BitConverter.GetBytes(sensitivity);
            Array.Reverse(sensitivityBytes);
            Array.Copy(sensitivityBytes, 0, output, 24, sensitivityBytes.Length);

            // TID
            byte[] tidBytes = new byte[12];
            Array.Copy(tidBytes, 0, output, 26, tidBytes.Length);

            byte fcs = CalculateChecksum(output, len);

            output[38] = fcs;

            // E3 command filling

            var array = new object[output.Length];
            output.CopyTo(array, 0);

            return array;
        }

        private byte CalculateChecksum(byte[] responseBytes, byte len)
        {
            byte fcs = 0;
            byte[] bytes = responseBytes.Where((x, index) => index != 0 && index != 3).ToArray(); // Checksum. The sum of the LEN (index 1), CMD (index 2), TMR HI (index 4), TMR LO (5) and data bytes (index >5). Modulo 256.
            for (int i = 0; i < bytes.Length; i++)
            {
                fcs += bytes[i];
            }

            return fcs;
        }
    }
}