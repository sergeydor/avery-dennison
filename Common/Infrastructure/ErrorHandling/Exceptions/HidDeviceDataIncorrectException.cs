using System;

namespace Common.Infrastructure.ErrorHandling.Exceptions
{
    public class HidDeviceDataIncorrectException : Exception
    {
        public HidDeviceDataIncorrectException(string message) : base(message)
        {
            
        }
    }
}