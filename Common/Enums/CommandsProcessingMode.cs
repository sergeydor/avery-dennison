using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    public enum CommandsProcessingMode
    {
        NotRunning = 0,
        Running = 1,
        NotRunningPending = 2 // moving to stop state
    }
}
