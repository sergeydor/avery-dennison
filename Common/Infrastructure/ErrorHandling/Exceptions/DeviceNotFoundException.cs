using System;

namespace Common.Infrastructure.ErrorHandling.Exceptions
{
    public class DeviceNotFoundException : Exception
    {
        public DeviceNotFoundException(string message) : base(message)
        {
            
        }
    }
}