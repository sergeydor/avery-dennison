using Common.Infrastructure.ErrorHandling.Enums;
using Common.Infrastructure.ErrorHandling.Helpers;
using Common.Infrastructure.ErrorHandling.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure.ErrorHandling.Exceptions
{
    public class BusinessLogicException : Exception
    {
        public ErrorCode ErrorCode { get; private set; }

        public string ErrorText { get; private set; }

        public BusinessLogicException(ErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }

        public BusinessLogicException(ErrorCode errorCode, string errorText)
            : base(errorText)
        {
            ErrorCode = errorCode;
            ErrorText = errorText;
        }

        public virtual ErrorDetails GetErrorDetails()
        {
            var details = ErrorMessageHelper.CreateError(ErrorCode);
            if (!string.IsNullOrEmpty(ErrorText))
                details.ErrorText = ErrorText;
            return details;
        }
    }
}
