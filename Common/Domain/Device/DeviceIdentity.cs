using Common.Enums.GSTCommands;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.Device
{
    public class DeviceIdentity
    {
        [BsonIgnore]
        public bool IsReader { get { return DeviceType == HighSpeedTestDeviceType.Reader; } }
        public string MacAddress { get; set; }
        [BsonIgnore]
        public string ProductId { get; set; }
        [BsonIgnore]
        public string VendorId { get; set; }
        public int Lane { get; set; } = -1;
        public HighSpeedTestDeviceType DeviceType { get; set; } = HighSpeedTestDeviceType.None;
    }
}
