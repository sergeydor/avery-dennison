using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Common.Domain.Conveyor;
using Common.Domain.Device;
using Common.Enums;
using Common.Infrastructure.Devices;
using Common.Infrastructure.Extensions;
using Common.Services;
using Common.Services.Output;
using Server.Device.Communication.RemoteServices.ServiceContracts;
using Simulator.Svc.Context;
using Simulator.Svc.Infrastructure;
using Simulator.Svc.Infrastructure.Devices;
using Simulator.Svc.Services;
using Server.Device.Communication.Domain;
using Common.Services.Input;
using Server.Device.Communication.Infrastructure.Logging;
using System;

namespace Simulator.Svc.RemoteServices
{
	public class SimulatorSvc : ServiceBase, ISimulatorSvc
	{
		private readonly HidDeviceFactory _deviceFactory;
		private readonly DeviceProcessingService _deviceProcessingService;
		private readonly DeviceProcessingContext _context;
		private readonly ConveyorSettings _conveyorSettings;
		private readonly SimulatorSettings _simulatorSettings;
		private Thread _thread;

		public SimulatorSvc(MongoDbLogger logger, HidDeviceFactory deviceFactory, DeviceProcessingService deviceProcessingService, 
			DeviceProcessingContext context) : base(logger)
		{
			_deviceFactory = deviceFactory;
			_deviceProcessingService = deviceProcessingService;
			_context = context;
			_simulatorSettings = SimulatorSettingsSingleton.GetInstance();
			_conveyorSettings = ConveyorSettingsSingleton.GetInstance();
		}

		public SvcOutputGeneric<DeviceConfig> Initialize(SvcInputGeneric<int> readersCountInput)
		{
			var result = new SvcOutputGeneric<DeviceConfig>();

			RunCode(readersCountInput, result, () =>
			{
				var config = (DeviceConfigSection)ConfigurationManager.GetSection("devices");
				DeviceConfig deviceConfig = config.GetDeviceConfig();
				deviceConfig.NumberOfReadersSetOnUI = readersCountInput.Input;

				var devices = _deviceFactory.CreateDevices(deviceConfig);

				_context.GPIO = devices.FirstOrDefault(x => x.DeviceType == DeviceType.GPIO) as GPIODevice;
				var readers = devices.Where(x => x.DeviceType == DeviceType.Reader);
				foreach (var reader in readers)
				{
					_context.AddReader(reader as ReaderDevice);
				}

				_thread = new Thread(() => _deviceProcessingService.ListenToDevices());
				_thread.Start();

				result.Output = deviceConfig;
			});
			
			return result;
		}

		public SvcOutputGeneric<long> GetTimerOffset()
		{
			var result = new SvcOutputGeneric<long>();

			var timerOffset = long.Parse(ConfigurationManager.AppSettings["timerOffset"]);
			result.Output = timerOffset;

			return result;
		}

		public SvcOutputBase ReinstallDsf()
		{
			var result = new SvcOutputBase();

			RunCode(null, result, () =>
			{
				_thread?.Abort();
				int unplugedDevicesCount = _deviceFactory.UnplugDevices();

				_context.GPIO = null;
				_context.ClearReaders();

				if (unplugedDevicesCount == 0)
				{
					var proc = new ProcessStartInfo();
					string dsfPath = ConfigurationManager.AppSettings["dsfPath"];
					string fileName = Path.Combine(dsfPath, "softehcicfg.exe");
					if (!File.Exists(fileName))
					{
						throw new FileNotFoundException("Path to softehcicfg.exe is not correct. Please, set correct path to App.config key 'dsfPath'");
					}

					proc.FileName = fileName;
					proc.Arguments = "/remove";
					Process.Start(proc);
					Thread.Sleep(200);

					proc.Arguments = "/install";
					Process.Start(proc);
				}
			});

			return result;
		}

		public SvcOutputBase ExcludeDevices(List<DeviceIdentity> devices)
		{
			var result = new SvcOutputBase();

			base.RunCode(null, result, () =>
			{
				if (devices != null)
				{
					_deviceProcessingService.ExcludeDevices(devices);
				}
			});

			return result;
		}

        public SvcOutputBase Reset()
		{
			var result = new SvcOutputBase();

			base.RunCode(null, result, () =>
			{
				_deviceProcessingService.Reset();
			});

			return result;
		}

        public SvcOutputBase SetSimulatorSettings(SvcInputGeneric<SimulatorSettings> input)
        {
            var result = new SvcOutputBase();
            base.RunCode(input, result, () =>
            {
                _simulatorSettings.DistanceBetweenTagsInMm = input.Input.DistanceBetweenTagsInMm;
                _simulatorSettings.EncoderReaderTagsDistance = input.Input.EncoderReaderTagsDistance;
                _simulatorSettings.EncoderStepsPerTag = input.Input.EncoderStepsPerTag;
                _simulatorSettings.TagLengthInMm = input.Input.TagLengthInMm;
                _simulatorSettings.TestTagsNumber = input.Input.TestTagsNumber;
                _simulatorSettings.VelocityTagsPerMSec = input.Input.VelocityTagsPerMSec;
            });
            return result;            
        }

        public SvcOutputBase StopTest()
        {
            var result = new SvcOutputBase();
            base.RunCode(null, result, () =>
            {
                _deviceProcessingService.StopTest();
            });
            return result;
        }            
    }
}
