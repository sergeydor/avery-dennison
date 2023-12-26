using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;


namespace Common.Services.Output
{
    [DataContract]
    public class SvcOutputGeneric<TOutput> : SvcOutputBase
    {
        [DataMember]
        public TOutput Output { get; set; }

        public override string ToString()
        {
            if (this.Output is IEnumerable && !(this.Output is string))
            {
                string sitems = string.Empty;
                foreach(var item in this.Output as IEnumerable)
                {
                    sitems += (item + ", ");
                }
                return base.ToString() + " Output/items list begin" + sitems + " items list end ";
            }
            else
            {
                return base.ToString() + " Output " + Output;
            }
        }
    }
}
