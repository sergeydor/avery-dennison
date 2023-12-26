using Common.Domain.TestModuleCommands;
using Common.Enums.GSTCommands;
using Common.Services.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.RemoteServices.Dtos.Input
{
    [Serializable]
    public class SetDataToDeviceSvcInput<TEntity> : DeviceEntitySvcInput<TEntity>
    {
    }
}
