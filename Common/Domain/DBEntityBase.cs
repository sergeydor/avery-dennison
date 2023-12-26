using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain
{
    public class DBEntityBase
    {
        public ObjectId Id { get; set; }
    }
}
