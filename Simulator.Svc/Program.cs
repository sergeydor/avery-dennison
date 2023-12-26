using System.ServiceModel;
using Autofac;
using Common.Infrastructure.Logging;
//using Common.Infrastructure.Logging.LayoutRenderers;
using Simulator.Svc.Services;
using NLog.Config;
using Simulator.Svc.RemoteServices;
using Topshelf;
using Autofac.Integration.Wcf;
using Common.Infrastructure.Constants;
using Simulator.Svc.Context;

namespace Simulator.Svc
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

						var serviceHost = new ServiceHost(typeof(SimulatorSvc));
			            serviceHost.AddDependencyInjectionBehavior<SimulatorSvc>(container);
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
					var logger = scope.Resolve<Logger>();
					logger.LogError(ex.ToString());
				});

                x.RunAsLocalSystem();

                x.SetDescription("Avery simulator");
                x.SetDisplayName("AverySimulatorSvc");
                x.SetServiceName("AverySimulatorSvc");
            });
        }
	}
}
