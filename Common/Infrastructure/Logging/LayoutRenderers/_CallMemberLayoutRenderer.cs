//using System.Linq;
//using System.Text;
//using Common.Infrastructure.Constants;
//using NLog;
//using NLog.LayoutRenderers;

//namespace Common.Infrastructure.Logging.LayoutRenderers
//{
//	[LayoutRenderer(LoggerCustomLayouts.CallMember)]
//	public class CallMemberLayoutRenderer : LayoutRenderer
//	{
//		protected override void Append(StringBuilder builder, LogEventInfo logEvent)
//		{
//			if (logEvent.Properties == null || !logEvent.Properties.Any())
//			{
//				return;
//			}

//			builder.Append(logEvent.Properties[LoggerCustomLayouts.CallMember]);
//		}
//	}
//}