using Common.Domain;
using Common.Domain.Device;
using Common.Enums;
using MongoDB.Bson;
using Server.Device.Communication.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace Server.Device.Communication.DataAccess.DBEntities
{
    public class DBCollectionCommand : DBEntityBase
    {
        public static readonly byte[] UnsolicitedCommands = new byte[] { 0xE3, 0x90 };

        public CommandData RequestData { get; set; }

        public ResponseData ResponseData { get; set; }

        public string UserFriendlyCmdView { get; set; }

        public DeviceIdentity Device { get; set; }

        public ObjectId AppSessionId { get; set; } = ObjectId.Empty;

        public ObjectId TestId { get; set; } = ObjectId.Empty;

        public Direction Direction { get; set; } = Direction.NDef;

		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime ReceiveDt { get; set; }

        public bool IsUnsolicited { get; set; } = false;

        public uint TempLabelNum { get; set; } = 0;

        public float TempLabelOffset { get; set; } = 0;
    }
}
