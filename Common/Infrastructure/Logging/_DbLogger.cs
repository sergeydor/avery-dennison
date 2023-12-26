//using System;
//using NLog;

//namespace Common.Infrastructure.Logging
//{
//	public class DbLogger : Logger
//	{
//		protected override NLog.Logger GetLogger()
//		{
//			NLog.Logger logger;
//			try
//			{
//				logger = LogManager.GetCurrentClassLogger(typeof (DbLogger));
//			}
//			catch (Exception ex)
//			{
//				logger = null;
//			}

//			return logger;
//		}
//	}
//}