using Common.Domain;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Device.Communication.DataAccess.DBEntities
{
    public class DBTest : DBEntityBase
    {
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime Started { get; set; }
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime Finished { get; set; }
        public ObjectId AppSession { get; set; }
        public string TestName { get; set; }
    }
}
