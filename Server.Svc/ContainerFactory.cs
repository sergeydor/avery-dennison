using Autofac;
using Common.Infrastructure.Logging;
using Server.Svc.Context;
using Server.Svc.RemoteServices;
using Server.Svc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Server.Device.Communication.CommandInterpretators;
using System.Configuration;
using Common.Infrastructure;
using MongoDB.Driver;
using Server.Device.Communication.DataAccess.Repositories;
using Server.Device.Communication.Infrastructure.Logging;

namespace Server.Svc
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

            builder.RegisterType<CommandsPooler>().SingleInstance();            
            builder.RegisterType<CommandsProcessingContext>().SingleInstance();
	        
            
            builder.RegisterType<AveryServerSvc>().SingleInstance();
            builder.RegisterType<ProcessingSvcFacade>().SingleInstance();
            builder.RegisterType<WaitForCommandResultService>().SingleInstance();
            //builder.RegisterType<TempSnapshotsQueue>().SingleInstance();
	        //builder.RegisterType<TempUnsolicitedCommandsLogQueue>().SingleInstance();

            // interpretators registration
            builder.RegisterType<TestSetupCommandsInterpretator>();
            builder.RegisterType<TestModuleCommandsInterpretator>();
            builder.RegisterType<TestActionCommandsInterpretator>();
            builder.RegisterType<GSTCommandsInterpretator>();
            builder.RegisterType<GetTestSetupCommandsInterpretator>();
	        builder.RegisterType<ExtendedGetTestSetupCommandsInterpretator>();
	        builder.RegisterType<ExtendedTestSetupCommadsInterpretator>();
            builder.RegisterType<UnsolicitedReplyCommandsInterpretator>();
            

            string connectionStringValue = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
            //var mongoUrl = MongoUrl.Create(connectionStringValue);
            builder.RegisterType<MongoClient>().WithParameter("connectionString", connectionStringValue).SingleInstance();
            builder.Register<IMongoDatabase>(c => c.Resolve<MongoClient>().GetDatabase(ConfigurationManager.AppSettings["AverySysDbName"]));

            builder.RegisterType<AppSessionRepository>();
            builder.RegisterType<DBTestRepository>();
			builder.RegisterType<AppLogRepository>();

			builder.Register<MongoDbLogger>(c => new MongoDbLogger(c.Resolve<AppLogRepository>(), c.Resolve<CommandsProcessingContext>()));

			//builder.RegisterType<DeviceOutgoingCommandRepository>();

			return builder.Build();
        }
    }
}
