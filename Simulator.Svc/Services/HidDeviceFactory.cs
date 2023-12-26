using System;
using System.Collections.Generic;
using System.Linq;
using Common.Domain.Device;
using DSF;
using Simulator.Svc.Infrastructure.Devices;
using SoftEHCI;
using SoftHIDReceiver;
using SOFTUSB;

namespace Simulator.Svc.Services
{
	public class HidDeviceFactory
	{
		private readonly List<ISoftEHCIRootHubPort> _rootHubPorts = new List<ISoftEHCIRootHubPort>();
		private readonly List<ISoftUSBHubPort> _usbHubPorts = new List<ISoftUSBHubPort>();
		private readonly List<ISoftUSBHub> _usbHubs = new List<ISoftUSBHub>();
		private readonly List<HIDDevice> _hidDevices = new List<HIDDevice>();

		public List<DeviceBase> CreateDevices(DeviceConfig deviceConfig)
		{
			var devices = new List<DeviceBase>();

			int hubsCount = (int)Math.Ceiling(((double)deviceConfig.NumberOfReadersSetOnUI + 1) / 4);
			List<ISoftUSBHub> usbHubs = CreateUsbHubs(hubsCount);

			// create and plug virtual devices to USB hubs
			int usbPortNum = 1;
			int usbHubNum = 0;

			HIDDevice gpioDevice = PlugDevice(usbHubs[usbHubNum], usbPortNum, deviceConfig.VendorId, deviceConfig.GPIOProductId, 1);
			_hidDevices.Add(gpioDevice);
			var gpio = new GPIODevice(gpioDevice);
			devices.Add(gpio);

			usbPortNum += 1;

			for (int deviceIndex = 1; deviceIndex <= deviceConfig.NumberOfReadersSetOnUI; deviceIndex++)
			{
				ISoftUSBHub hub = usbHubs[usbHubNum];
				HIDDevice readerDevice = PlugDevice(hub, usbPortNum, deviceConfig.VendorId, deviceConfig.ReaderProductId, (short)(deviceIndex + 1));
				_hidDevices.Add(readerDevice);
				var reader = new ReaderDevice(readerDevice);
				devices.Add(reader);

				// when all USB ports in USB hub are busy increment usbHubNum to pull hext hub from hubs list 
				// and set usbPortNum to 1
				if (usbPortNum < 4)
				{
					usbPortNum++;
				}
				else
				{
					usbPortNum = 1;
					usbHubNum++;
				}
			}

			//int gpioPortNum = deviceConfig.ReadersCount % 4 + 1;
			/*HIDDevice gpioDevice = PlugDevice(usbHubs.Last(), gpioPortNum, deviceConfig.VendorId, deviceConfig.GPIOProductId, (short)(gpioPortNum + 1));
			_hidDevices.Add(gpioDevice);
			var gpio = new GPIODevice(gpioDevice);
			devices.Add(gpio);*/

			return devices;
		}

		public int UnplugDevices()
		{
			DeviceContainer.Container.StopProcessing();

			foreach (var usbHubPort in _usbHubPorts)
			{
				usbHubPort.Unplug();
			}

			int result = 0;
			foreach (var hidDevice in _hidDevices)
			{
				DeviceContainer.Container.RemoveDevice(hidDevice);
				hidDevice.Destroy();
				result++;
			}

			foreach (var rootHubPort in _rootHubPorts)
			{
				rootHubPort.Unplug();
			}

			foreach (var usbHub in _usbHubs)
			{
				usbHub.Destroy();
			}

			_hidDevices.Clear();
			_usbHubs.Clear();
			_usbHubPorts.Clear();
			_rootHubPorts.Clear();

			return result;
		}

		private List<ISoftUSBHub> CreateUsbHubs(int hubsCount)
		{
			ISoftEHCIRootHubPorts portsWrapper = CreateRootHubPorts();
			var usbHubs = new List<ISoftUSBHub>();
			for (int i = 1; i <= hubsCount; i++)
			{
				var usbHub = PlugUsbHub(portsWrapper, i);
				usbHubs.Add(usbHub);
			}

			return usbHubs;
		}

		private ISoftEHCIRootHubPorts CreateRootHubPorts()
		{
			IDSF dsf = new DSFClass();
			ISoftEHCICtrlr ehciDevice = null;

			IDSFDevice dsfDevice = dsf.Devices[0];
			if (dsfDevice == null)
			{
				return null;
			}

			var hasObject = dsfDevice.HasObject("{E927C266-5364-449E-AE52-D6A782AFDA9C}");

			if (hasObject)
			{
				var punkCtrlrDevice = dsfDevice.Object["{16017C34-A2BA-480B-8DE8-CD08756AD1F8}"];

				if (punkCtrlrDevice != null)
				{
					ehciDevice = (ISoftEHCICtrlr)punkCtrlrDevice;
				}
			}

			if (ehciDevice == null)
			{
				return null;
			}

			ISoftEHCIRootHubPorts ports = ehciDevice.Ports;

			return ports;
		}

		private ISoftUSBHub PlugUsbHub(ISoftEHCIRootHubPorts ports, int rootHubPort)
		{
			ISoftEHCIRootHubPort rootPort;
			if (ports != null)
			{
				rootPort = ports[rootHubPort];
			}
			else
			{
				return null;
			}

			ISoftUSBHub usbHub = new SoftUSBHubClass();
			ISoftUSBDevice piSoftUsbDevice = usbHub.SoftUSBDevice;
			IDSFDevice dsfDevice = piSoftUsbDevice.DSFDevice;
			rootPort.HotPlug((DSFDevice)dsfDevice);

			_rootHubPorts.Add(rootPort);

			return usbHub;
		}

		private HIDDevice PlugDevice(ISoftUSBHub usbHub, int port, short vendorId, short productId, short deviceId)
		{
			ISoftUSBHubPort usbPort = usbHub.Ports[port];

			var device = new HIDDeviceClass();
			DeviceContainer.Container.AddDevice(device);
			device.CreateCustomHIDDevice(vendorId, productId, deviceId);
			device.BuildHIDDescriptors();
			device.ConfigureDevice();

			usbPort.HotPlug(device.DSFDevice);

			_usbHubPorts.Add(usbPort);

			return device;
		}
	}
}