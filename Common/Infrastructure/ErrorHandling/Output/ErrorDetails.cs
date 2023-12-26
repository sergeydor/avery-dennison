using Common.Infrastructure.ErrorHandling.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Infrastructure.ErrorHandling.Output
{
    [Serializable]
    [DataContract]
    public class ErrorDetails
    {
        [DataMember]
        public ErrorCode ErrorCode { get; set; }

        [DataMember]
        public string ErrorText { get; set; }

        public ErrorDetails()
        { }

        public ErrorDetails(ErrorCode code, string text)
        {
            ErrorCode = code;
            ErrorText = text;
        }

        public override string ToString()
        {
            return " ErrorCode " + ErrorCode + " ErrorText " + ErrorText;
        }
    }
}
