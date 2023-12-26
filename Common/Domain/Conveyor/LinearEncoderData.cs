using System.Collections.Generic;
using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Domain.Conveyor
{
    public struct LinearEncoderData
    {
        public List<LinearEncoderDataItem> Items { get; set; }

        /// <summary>
        /// Tag's number on the conveyor, identified by encoder
        /// </summary>
        public uint TriggerNumber { get; set; }

        public double VelocityAvgInMMPerMSec { get; set; }

        [BsonIgnore]
        public LinearEncoderDataItem CurrentItem
        {
            get
            {
                if (this.Items == null || this.Items.Count == 0)
                    return default(LinearEncoderDataItem);
                else
                    return this.Items[this.Items.Count - 1]; // it's always last-added item
            }
        }

        [BsonIgnore]
        public bool HasItems
        {
            get
            {
                return this.Items.Count > 0;
            }
        }
    }
}
