using Topshelf;

namespace Simulator.Svc.Services
{
    internal class TopshelfEntryPointService : ServiceControl
    {
        public TopshelfEntryPointService()
        {
        }

	    public bool Start(HostControl hostControl)
	    {
		    return true;
	    }

	    public bool Stop(HostControl hostControl)
	    {
		    return true;
	    }
    }
}
