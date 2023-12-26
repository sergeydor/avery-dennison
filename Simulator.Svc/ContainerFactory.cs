using System.Configuration;
using Autofac;
using Common.Infrastructure.Logging;
using MongoDB.Driver;
using Server.Device.Communication.DataAccess.Repositories;
using Server.Device.Communication.Infrastructure.Logging;
using Simulator.Svc.Context;
using Simulator.Svc.RemoteServices;
using Simulator.Svc.Services;

namespace Simulator.Svc
{
    public class ContainerFactory
    {
		public static IContainer CreateContainer()
        {
            ContainerBuilder builder = new ContainerBuilder();
			builder.RegisterType<Logger>();
			//builder.RegisterType<DbLogger>();
			//builder.RegisterType<FileLogger>();
			builder.RegisterType<TopshelfEntryPointService>();
	        builder.RegisterType<HidDeviceFactory>().SingleInstance();
	        builder.RegisterType<DeviceProcessingService>().SingleInstance();
	        builder.RegisterType<DeviceProcessingContext>().SingleInstance();
	        builder.RegisterType<SimulatorSvc>().SingleInstance();

            string connectionStringValue = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            //var mongoUrl = MongoUrl.Create(connectionStringValue);
            builder.RegisterType<MongoClient>().WithParameter("connectionString", connectionStringValue).SingleInstance();
            builder.Register<IMongoDatabase>(c => c.Resolve<MongoClient>().GetDatabase(ConfigurationManager.AppSettings["AverySysDbName"]));

            builder.Register<MongoDbLogger>(c => new MongoDbLogger(c.Resolve<AppLogRepository>(), null));

			builder.RegisterType<AppLogRepository>();
			return builder.Build();
        }
    }
}
