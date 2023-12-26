using MongoDB.Bson;

namespace Common.Infrastructure
{
	public interface IAppContext
	{
		ObjectId CurrentSessionId { get; }
		ObjectId CurrentTestId { get; }
	}
}