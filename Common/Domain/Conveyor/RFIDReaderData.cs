namespace Common.Domain.Conveyor
{
    public struct RFIDReaderData
    {
        /// <summary>
        /// Electronic product code. 
        /// </summary>
        public string EPC { get; set; }

        /// <summary>
        /// Tag identification memory 
        /// consists of memory about the tag itself, such as the tag ID
        /// </summary>
        public string TID { get; set; }

        /// <summary>
        /// Received Signal Strength Indication
        /// </summary>
        public double RSSI { get; set; }

        /// <summary>
        /// The line where the reader is placed on
        /// </summary>
        public int LineYNumber { get; set; }
    }
}
