using Server.Device.Communication.DataAccess.DBEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Server.Device.Communication.DataAccess.Repositories
{
    public class AppSessionRepository : BaseRepository<DBAppSession>
    {
        public AppSessionRepository(IMongoDatabase database) : base(database)
        {
        }
    }
}
