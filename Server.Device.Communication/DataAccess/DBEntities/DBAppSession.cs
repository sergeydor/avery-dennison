using Common.Domain;
using Common.Domain.Device;
using Common.Enums;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Device.Communication.DataAccess.DBEntities
{
    public class DBAppSession : DBEntityBase
    {
		public string Name { get; set; }

        public List<DeviceIdentity> Devices { get; set; }

		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime StartDt { get; set; }

        public AppMode AppMode { get; set; }
    }
}
