using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.DeviceResults
{
    public class EntityDeviceResult<TEntity> : GeneralDeviceResult
    {
        public TEntity Entity { get; set; }
    }
}
