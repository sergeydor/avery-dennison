//using System.Configuration;

//namespace Simulator.Svc.Infrastructure.Config
//{
//	public class ConveyorSettingsSection : ConfigurationSection
//	{
//		[ConfigurationProperty("totalDistanceInMm")]
//		public int TotalDistanceInMm
//		{
//			get { return (int) this["totalDistanceInMm"]; }
//			set { this["totalDistanceInMm"] = value; }
//		}

//		[ConfigurationProperty("tagLengthInMm")]
//		public int TagLengthInMm
//		{
//			get { return (int) this["tagLengthInMm"]; }
//			set { this["tagLengthInMm"] = value; }
//		}

//		[ConfigurationProperty("distanceBetweenTagsInMm")]
//		public int DistanceBetweenTagsInMm
//		{
//			get { return (int) this["distanceBetweenTagsInMm"]; }
//			set { this["distanceBetweenTagsInMm"] = value; }
//		}

//		[ConfigurationProperty("tagsCountPerOneLane")]
//		public int TagsCountPerOneLane
//		{
//			get { return (int) this["tagsCountPerOneLane"]; }
//			set { this["tagsCountPerOneLane"] = value; }
//		}

//		[ConfigurationProperty("encoderReaderTagsDistance")]
//		public int EncoderReaderTagsDistance
//		{
//			get { return (int) this["encoderReaderTagsDistance"]; }
//			set { this["encoderReaderTagsDistance"] = value; }
//		}

//		[ConfigurationProperty("readerMarkerTagsDistance")]
//		public int ReaderMarkerTagsDistance
//		{
//			get { return (int) this["readerMarkerTagsDistance"]; }
//			set { this["readerMarkerTagsDistance"] = value; }
//		}

//		[ConfigurationProperty("markerPuncherTagsDistance")]
//		public int MarkerPuncherTagsDistance
//		{
//			get { return (int) this["markerPuncherTagsDistance"]; }
//			set { this["markerPuncherTagsDistance"] = value; }
//		}

//		[ConfigurationProperty("timerCicleLengthMs")]
//		public long TimerCicleLengthMs
//		{
//			get { return (long) this["timerCicleLengthMs"]; }
//			set { this["timerCicleLengthMs"] = value; }
//		}
//	}
//}