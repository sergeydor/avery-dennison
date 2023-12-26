using Common.Domain.DeviceResults.UnsolicitedReplyCommands;
using System;

namespace Common.Domain.Conveyor
{
    public struct RFIDTag
    {
        /// <summary>
        /// Unique tag's identifier. Each analyzed through sowftware tag obtains it's own identifier
        /// </summary>
        //public Guid Id { get; set; }

        /// <summary>
        /// Calculated value which means the line number where the rfid reader is placed on
        /// </summary>
        public int LineYIndex { get; set; }

        public TestE3Result RFIDReaderData
        {
            get; set;
        }

        /// <summary>
        /// The data which is taken from NWA Cooper Mountain TR5048 and associated with the tag
        /// </summary>
        public VNAData VNAData { get; set; }

        /// <summary>
        /// This means the test which ia based on VNA - device that sweeps the tag performance for a predefined frequency range and gathers amplitude values
        /// </summary>
        public bool IsVNATestPassed { get; set; }


    }
}
