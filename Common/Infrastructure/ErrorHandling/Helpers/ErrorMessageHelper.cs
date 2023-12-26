using Common.Infrastructure.ErrorHandling.Enums;
using Common.Infrastructure.ErrorHandling.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Infrastructure.ErrorHandling.Extensions;

namespace Common.Infrastructure.ErrorHandling.Helpers
{
    public class ErrorMessageHelper
    {
        public static string GetErrorMessage(ErrorCode errorCode)
        {
            string description = errorCode.GetDescription();
            if (!string.IsNullOrEmpty(description))
                return description;
            return Enum.GetName(typeof(ErrorCode), errorCode);
        }

        public static ErrorDetails CreateError(ErrorCode code)
        {
            return new ErrorDetails(code, GetErrorMessage(code));
        }

        public static ErrorDetails CreateError(ErrorCode code, string messageText)
        {
            return new ErrorDetails(code, messageText);
        }
    }
}
