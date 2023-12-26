using Common.Domain.Conveyor;
using Common.Services.Output;
using Server.Device.Communication.Domain;
using System;
using System.Collections.Generic;
using Topshelf;

namespace Server.Svc.Services
{
    internal class TopshelfEntryPointService : ServiceControl
    {
        private CommandsPooler _pooler;
        private readonly ProcessingSvcFacade _processingSvcFacade;

        public TopshelfEntryPointService(CommandsPooler pooler, ProcessingSvcFacade processingSvcFacade)
        {
            _pooler = pooler;
            _processingSvcFacade = processingSvcFacade;
        }

        public bool Start(HostControl hostControl)
        {
            ProcessingSvcFacade unmgFacade = _processingSvcFacade;
            //SvcOutputGeneric<UnmanagedCallResult> res = unmgFacade.StartTestInternalProcessing();
            //SvcOutputGeneric<UnmanagedCallResult> res1 = unmgFacade.StartTestInternalProcessing();
    //        List<ConveyorShot> res2 = unmgFacade.Test("sdfsdf 234234 er werqw", DateTime.Now);
            return true;
        }

	    public bool Stop(HostControl hostControl)
	    {
		    return true;
	    }
    }
}
