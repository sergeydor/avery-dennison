using System;
using Common.Domain;
using Common.Infrastructure;
using Common.Infrastructure.Extensions;
using Common.Infrastructure.Logging;
using MongoDB.Bson;
using Server.Device.Communication.DataAccess.Repositories;

namespace Server.Device.Communication.Infrastructure.Logging
{
	public class MongoDbLogger : Logger
	{
		private readonly AppLogRepository _appLogRepository;
		private readonly IAppContext _context;

		public MongoDbLogger(AppLogRepository appLogRepository, IAppContext context)
		{
			_appLogRepository = appLogRepository;
			_context = context;
		}

		protected override void LogMessage(LogLevel level, string message, object callerContext, string callerMemberName, string callerModuleNameOrFilePath,
			int callerLineNumber)
		{
            if (callerMemberName == "GetDeviceDataLogItems" || callerMemberName == "GetUnsolicitedStats")
                return;

            var appLog = new AppLog
			{
				Id = ObjectId.GenerateNewId(),
				Date = DateTime.Now,
				Message = message,
				Level = level,
				CallerMemberName = callerMemberName,
				CallerFilePath = callerModuleNameOrFilePath,
				CallerLineNumber = callerLineNumber,
				CallerContext = callerContext.ToLog(5),
				AppSession = _context?.CurrentSessionId,
				Test = _context?.CurrentTestId
			};

			_appLogRepository.Create(appLog);
		}
	}
}