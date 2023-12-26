using Common.Domain.Conveyor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Server.Device.Communication.DataAccess.Repositories
{
    public class ConveyorShotRepository : BaseRepository<ConveyorShot>
    {
        public override int BatchSize { get; set; } = 50;

        public ConveyorShotRepository(IMongoDatabase database) : base(database)
        {
        }
    }
}
