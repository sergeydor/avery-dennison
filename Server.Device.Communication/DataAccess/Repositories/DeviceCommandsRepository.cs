using Server.Device.Communication.DataAccess.DBEntities;
using Server.Device.Communication.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Common.Domain.Device;

namespace Server.Device.Communication.DataAccess.Repositories
{
    public class DeviceCommandsRepository : BaseRepository<DBCollectionCommand>
    {
        private DeviceIdentity _device;

        public DeviceCommandsRepository(IMongoDatabase database, DeviceIdentity device) : base(database)
        {
            _device = device;
        }

        protected override IMongoCollection<DBCollectionCommand> Collection
        {
            get
            {
                string collName = base.CollectionName + '_' + _device.MacAddress;
                return _database.GetCollection<DBCollectionCommand>(collName);
            }
        }
    }
}
