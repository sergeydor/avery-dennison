using System;
using System.Runtime.CompilerServices;

namespace Common.Infrastructure.Logging
{
    public abstract class Logger //: NLog.Logger
    {
        public virtual void LogError(string error, object callerContext = null, [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerModuleNameOrFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogMessage(LogLevel.Error, error, callerContext, callerMemberName, callerModuleNameOrFilePath, callerLineNumber);
        }

        public virtual void LogWarning(string msg, object callerContext = null, [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerModuleNameOrFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogMessage(LogLevel.Warning, msg, callerContext, callerMemberName, callerModuleNameOrFilePath, callerLineNumber);
        }

        public virtual void LogInfo(string msg, object callerContext = null, [CallerMemberName] string callerMemberName = null,
            [CallerFilePath] string callerModuleNameOrFilePath = null,
            [CallerLineNumber] int callerLineNumber = -1)
        {
            LogMessage(LogLevel.Info, msg, callerContext, callerMemberName, callerModuleNameOrFilePath, callerLineNumber);
        }

	    protected abstract void LogMessage(LogLevel level, string message, object callerContext, string callerMemberName, string callerModuleNameOrFilePath, int callerLineNumber);

	    //     protected virtual void LogMessage(LogLevel level, string message, string callerMemberName, string callerModuleNameOrFilePath, Int32 callerLineNumber)
	    //     {
	    //         var logger = GetLogger();
	    //var eventInfo = new LogEventInfo(level, logger.Name, message);

	    //eventInfo.Properties.Add(LoggerCustomLayouts.CallMember, callerMemberName);
	    //eventInfo.Properties.Add(LoggerCustomLayouts.CallerFilePath, callerModuleNameOrFilePath);
	    //eventInfo.Properties.Add(LoggerCustomLayouts.CallerLineNumber, callerLineNumber);

	    //logger.Log(eventInfo);
	    //     }

	    //protected abstract NLog.Logger GetLogger();
    }
}
