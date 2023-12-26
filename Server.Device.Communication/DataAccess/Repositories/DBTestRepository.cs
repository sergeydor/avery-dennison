using Server.Device.Communication.DataAccess.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Server.Device.Communication.DataAccess.Repositories
{
    public class DBTestRepository : BaseRepository<DBTest>
    {
        public DBTestRepository(IMongoDatabase database) : base(database)
        {
        }
    }
}
