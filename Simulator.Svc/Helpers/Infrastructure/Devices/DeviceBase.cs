using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Enums;
using Simulator.Svc.Context;
using Simulator.Svc.Helpers;
using SoftHIDReceiver;

namespace Simulator.Svc.Infrastructure.Devices
{
	public class DeviceBase
	{
		public DeviceBase(HIDDevice deviceInstance)
		{
			DeviceInstance = deviceInstance;
		}

		public HIDDevice DeviceInstance { get; set; }

		public DeviceType DeviceType { get; protected set; }

		public Array MACAddress { get; set; }

		public List<DeviceSettings> Settings { get; set; } = new List<DeviceSettings>();

		public override string ToString()
		{
			var macAddress = new StringBuilder();
			for (int i = 0; i < MACAddress.Length; i++)
			{
				string item = MACAddress.GetValue(i).ToString();
				macAddress.AppendLine(item);
				if (i != MACAddress.Length - 1)
				{
					macAddress.AppendLine("-");
				}
			}

			return macAddress.ToString();
		}

		public void SetSettingsData(Array commandBytes, CommandPair commandPair)
		{
			DeviceSettings deviceSettingses = Settings.FirstOrDefault(x => x.Pair.Setter == commandPair.Setter) ?? new DeviceSettings {Pair = commandPair};
			byte len = (byte)commandBytes.GetValue(1);

			var data = new byte[len];
			Array.Copy(commandBytes, 3, data, 0, len);

			deviceSettingses.Data = data;

			Settings.Add(deviceSettingses);
		}

		public byte[] GetSettingsData(byte commandId)
		{
			DeviceSettings deviceSettingses = Settings.FirstOrDefault(x => x.Pair.Getter == commandId);
			byte[] data = deviceSettingses?.Data ?? new byte[0x0];

			return data;
		}
	}
}