using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Domain.Conveyor
{
    public class TagColumn
    {
        /// <summary>
        /// Tags in column sorted by lane position. 0 - uppermost
        /// </summary>
        public List<Tuple<int, RFIDTag>> Tags { get; set; } = null; // new Dictionary<int, RFIDTag>();

        /// <summary>
        /// This data is taken from linear encoder device. Based on it the conveyor's speed and tag's position calculated.
        /// </summary>
        public LinearEncoderData LinearEncoderData { get; set; }

        /// <summary>
        /// It's based on tags distance, tag length, and first encoder items' timestamps
        /// </summary>
        public double VelocityBOnDistInMmPerMSec { get; set; }

        /// <summary>
        /// This value will ber calculated at drawing-time, based on length/distance
        /// </summary>
        public double CurrentXPosition { get; set; } = -1;

        public void SetTagOnLane(int lane, RFIDTag tag)
        {
            if(!Tags.Any(t => t.Item1 == lane))
            {
                Tags.Add(new Tuple<int, RFIDTag>(lane, tag));
            }
        }
    }
}
