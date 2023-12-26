using Common.Domain;
using MongoDB.Driver;

namespace Server.Device.Communication.DataAccess.Repositories
{
	public class AppLogRepository : BaseRepository<AppLog>
	{
		public AppLogRepository(IMongoDatabase database) : base(database)
		{
		}
	}
}