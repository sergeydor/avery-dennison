using Autofac;
using Server.Svc.RemoteServices;
using Server.Svc.Services;
using System.ServiceModel;
using Topshelf;
using Autofac.Integration.Wcf;
using Common.Infrastructure.Logging;
//using Common.Infrastructure.Logging.LayoutRenderers;
using NLog.Config;
using System;
using Common.Infrastructure.Constants;

namespace Server.Svc
{
    class Program
    {
        static void Main(string[] args)
        {


			HostFactory.Run(x =>
            {
                IContainer container = null;
                ILifetimeScope scope = null;

				x.Service<TopshelfEntryPointService>(sc =>
	            {
		            sc.ConstructUsing(() =>
		            {
			            container = ContainerFactory.CreateContainer();
			            var serviceHost = new ServiceHost(typeof (AveryServerSvc));
			            serviceHost.AddDependencyInjectionBehavior<AveryServerSvc>(container);
			            serviceHost.Open();
			            scope = container.BeginLifetimeScope();
			            return scope.Resolve<TopshelfEntryPointService>();
		            });
		            sc.WhenStarted((ts, hostControl) =>
		            {
						//ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition(LoggerCustomLayouts.CallMember, typeof(Common.Infrastructure.Logging.LayoutRenderers.CallMemberLayoutRenderer));
						//ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition(LoggerCustomLayouts.CallerFilePath, typeof(Common.Infrastructure.Logging.LayoutRenderers.CallMemberFilePathLayoutRenderer));
						//ConfigurationItemFactory.Default.LayoutRenderers.RegisterDefinition(LoggerCustomLayouts.CallerLineNumber, typeof(Common.Infrastructure.Logging.LayoutRenderers.CallerLineNumberLayoutRenderer));

						return ts.Start(hostControl);
					});
		            sc.WhenStopped(ts =>
		            {
			            scope?.Dispose();
			            container?.Dispose();
		            });
	            });

				x.EnableServiceRecovery(ts =>
				{
					ts.RestartService(0);
					ts.OnCrashOnly();
				});

				x.OnException(ex =>
				{
					var logService = scope.Resolve<Logger>();
					logService.LogInfo(ex.ToString());
				});

				x.RunAsLocalSystem();                            

                x.SetDescription("Avery server");        
                x.SetDisplayName("AveryServerSvc");                       
                x.SetServiceName("AveryServerSvc");
            });
            Console.ReadKey();
		}
    }
}
