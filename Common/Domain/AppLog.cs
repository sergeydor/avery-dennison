using System;
using Common.Infrastructure.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Domain
{
	public class AppLog : DBEntityBase
	{
		[BsonDateTimeOptions(Kind = DateTimeKind.Local)]
		public DateTime Date { get; set; }
		public LogLevel Level { get; set; }
		public string Message { get; set; }
		public string CallerContext { get; set; }
		public string CallerMemberName { get; set; }
		public string CallerFilePath { get; set; }
		public int CallerLineNumber { get; set; }
		public ObjectId? AppSession { get; set; }
		public ObjectId? Test { get; set; }
	}
}