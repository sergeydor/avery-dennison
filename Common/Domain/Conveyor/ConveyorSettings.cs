namespace Common.Domain.Conveyor
{
	public class ConveyorSettings
	{
		public const int TagListBuffer = 300;

		public int TotalLanesTemp { get; set; } = 0; // is defined by readers count, when init 2 step //TODO

		//public int TotalDistanceInMm { get; set; } = 0; // calculated on UI side, based on  TagsCountPerOneLane DistanceBetweenTagsInMm TagLengthInMm, no need in services

        public int TagLengthInMm { get; set; } = 0; // set from UI

		public int DistanceBetweenTagsInMm { get; set; } = 0; // set from UI

        public int TagsCountPerOneLane { get; set; } = 0; // set from UI

        public int EncoderReaderTagsDistance { get; set; } = 0; // set from UI & via commands get-set

        public int ReaderMarkerTagsDistance { get; set; } = 0; // set from UI & via commands get-set

        public int MarkerPuncherTagsDistance { get; set; } = 0; // set from UI & via commands get-set

        public long TimerCicleLengthMs { get; set; } = 1 << 16; // seems to be not needed

	}
}