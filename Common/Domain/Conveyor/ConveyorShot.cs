using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.Conveyor
{
    /// <summary>
    /// The current state of conveyor.
    /// I assume, that shots will be made each ~5ms
    /// </summary>
    public class ConveyorShot : DBEntityBase
    {
        public DateTime CreateDt { get; set; }

        public DateTime PlanToCreateDt { get; set; }

        /// <summary>
        /// 0th is the most left which is just appered
        /// </summary>
        public List<TagColumn> TagsColumns { get; set; } = null;

        /// <summary>
        /// Must be set from outside
        /// </summary>
        public int TotalLanes { get; set; } = 0;

        /// <summary>
        /// Calculated, based on encoder data, tag length and distance
        /// </summary>
        public double VelocityInMMPerMSec { get; set; } = 0;

        public int GetTagIndexByTriggerNo(int lane, uint triggerNo)
        {
            int index = TagsColumns.FindIndex(t => t.LinearEncoderData.TriggerNumber == triggerNo);
            return index;
        }

        [BsonIgnore]
        public TagColumn CurrentColumn
        {
            get
            {
                if (this.TagsColumns.Count == 0)
                    return default(TagColumn);
                else
                    return this.TagsColumns[0]; // because of tagcolumn is struct - a copy is always retrieved from array
            }
        }
    }
}
