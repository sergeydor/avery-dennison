using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Server.Communication.Domain
{
    public class UIConveyorSettings
    {
        public int EncoderStepsPerTag { get; set; } = 3;

        public int VelocityTagsPerSec { get; set; } = 40;

        public int TestTagsNumber { get; set; } = 1000;

        public int TagLengthInMm { get; set; } = 40;

        public int DistanceBetweenTagsInMm { get; set; } = 20;

        public int TagsCountPerLane { get; set; } = 100;

        public int EncoderReaderTagsDistance { get; set; } = 10;

        public int ReaderMarkerTagsDistance { get; set; } = 10;

        public int MarkerPuncherTagsDistance { get; set; } = 10;
    }
}
