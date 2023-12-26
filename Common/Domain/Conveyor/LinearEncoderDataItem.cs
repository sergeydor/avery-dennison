using System;

namespace Common.Domain.Conveyor
{
    public struct LinearEncoderDataItem
    {
        public LinearEncoderDataItem(double velocityInMMPerMSec, DateTime processingTime, TimeSpan deviceTimeStamp, uint triggerNumber, double xPosition)
        {
            this.XPosition = xPosition;
            this.TriggerNumber = triggerNumber;
            this.DeviceTimeStamp = deviceTimeStamp;
            this.VelocityInMMPerMSec = velocityInMMPerMSec;
            this.ProcessingMockTime = processingTime;
        }

        /// <summary>
        /// It should be based on device timestamp and processing time. Something average
        /// </summary>
        public DateTime ProcessingMockTime { get; set; }

        /// <summary>
        /// This is cyclic device time
        /// </summary>
        public TimeSpan DeviceTimeStamp { get; set; }

        /// <summary>
        /// Tag's number on the conveyor, identified by encoder
        /// </summary>
        public uint TriggerNumber { get; set; }

        /// <summary>
        /// The position of identified tag
        /// </summary>
        public double XPosition { get; set; }

        public double VelocityInMMPerMSec { get; set; }
    }
}
