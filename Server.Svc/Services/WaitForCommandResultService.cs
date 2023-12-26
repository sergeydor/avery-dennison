using Server.Device.Communication.Domain;
using Server.Svc.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Svc.Services
{
    public class WaitForCommandResultService
    {
        private CommandsProcessingContext _commandsProcessingContext;

        public WaitForCommandResultService(CommandsProcessingContext commandsProcessingContext)
        {
            _commandsProcessingContext = commandsProcessingContext;
        }

        public int? _deviceResponseAwaitTimeoutInMs = null;
        public int DeviceResponseAwaitTimeoutInMs
        {
            get
            {
                if(_deviceResponseAwaitTimeoutInMs == null)
                {
                    _deviceResponseAwaitTimeoutInMs = int.Parse(ConfigurationManager.AppSettings["DeviceResponseAwaitTimeoutInMs"]);
                }
                return _deviceResponseAwaitTimeoutInMs.Value;
            }
        }

        /// <summary>
        /// Generates TimeoutException if result is not received in time.
        /// </summary>
        /// <param name="outgoing"></param>
        /// <returns></returns>
        public DeviceIncommingCommand WaitForResult(DeviceOutgoingCommand outgoing, long? timeout=null)
        {
            long timeoutInMs = timeout.HasValue ? timeout.Value : DeviceResponseAwaitTimeoutInMs;
            int hash = outgoing.ComputeHash();
            
            bool processed = _commandsProcessingContext.CheckIfProcessed(hash);
            if(!processed)
            {
                Stopwatch swatch = Stopwatch.StartNew();
	            while (swatch.ElapsedMilliseconds < timeoutInMs)
                {
                    processed = _commandsProcessingContext.CheckIfProcessed(hash);
                    if(processed)
                    {
                        return _commandsProcessingContext.GetResult(hash);
                    }
                    Thread.Sleep(100);
                }
                _commandsProcessingContext.ClearFaulted(hash); // otherwise it's not possible to replaa command
                throw new TimeoutException();
            }
            else
            {
                return _commandsProcessingContext.GetResult(hash);
            }
        }
    }
}