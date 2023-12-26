using Common.Infrastructure.ErrorHandling.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Services.Output
{
    [DataContract]
    public class SvcOutputBase
    {
        [DataMember]
        public ErrorDetails ErrorMessage { get; set; }

        public bool IsOk
        {
            get { return this.ErrorMessage == null; }
        }

        public override string ToString()
        {
            string res = " IsOk " + IsOk + " Svc.ErrorMessage " + (ErrorMessage == null ? "null" : ErrorMessage.ToString());
            return res;
        }
    }
}
