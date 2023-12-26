using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Simulator.Svc.Infrastructure.Devices;

namespace Simulator.Svc.Context
{
	public class DeviceProcessingContext
	{
		private List<ReaderDevice> Readers { get; set; } = new List<ReaderDevice>();
        private static object _syncObj = new object();

        public void AddReader(ReaderDevice reader)
        {
            lock (_syncObj)
                this.Readers.Add(reader);
        }
        public void RemoveReader(ReaderDevice reader)
        {
            lock (_syncObj)
                this.Readers.Remove(reader);
        }

        public void ClearReaders()
        {
            lock (_syncObj)
                this.Readers.Clear();
        }

        public List<ReaderDevice> ReadersListCopy
        {
            get
            {
                lock(_syncObj)
                    return new List<ReaderDevice>(Readers);
            }
        }

        public GPIODevice GPIO { get; set; }

		public DeviceBase GetDeviceByMacAddr(string macAddress)
		{
			if (GPIO.ToString() == macAddress)
			{
				return GPIO;
			}

			var reader = Readers.FirstOrDefault(x => x.ToString() == macAddress);

			return reader;
		}

		public int BadTagsPercent
		{
			get
			{
				string configValue = ConfigurationManager.AppSettings["BadTagsPercent"];
				int value;
				if (int.TryParse(configValue, out value))
				{
					return value;
				}

				return 0;
			}
		}

		public int MissedTagsPercent
		{
			get
			{
				string configValue = ConfigurationManager.AppSettings["MissedTagsPercent"];
				int value;
				if (int.TryParse(configValue, out value))
				{
					return value;
				}

				return 0;
			}
		}

		public int SpeedUpTagsCount
		{
			get
			{
				string configValue = ConfigurationManager.AppSettings["SpeedUpTagsCount"];
				int value;
				if (int.TryParse(configValue, out value))
				{
					return value;
				}

				return 0;
			}
		}

		public int SpeedDownTagsCount
		{
			get
			{
				string configValue = ConfigurationManager.AppSettings["SpeedDownTagsCount"];
				int value;
				if (int.TryParse(configValue, out value))
				{
					return value;
				}

				return 0;
			}
		}
	}
}