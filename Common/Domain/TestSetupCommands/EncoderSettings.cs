using MongoDB.Bson;

namespace Common.Domain.TestSetupCommands
{
	public class EncoderSettings
	{
        //public ObjectId Id { get; set; }

        public ushort TriggerFilterMin { get; set; } = 0x0001;
        public ushort TesterOffset { get; set; } = 0x0002;
        public ushort MarkerOffset { get; set; } = 0x0003;
        public ushort PunchOffset { get; set; } = 0x0003;
		public ushort PunchFlight { get; set; } = 0x0004;
        public ushort TriggerFilterMax { get; set; } = 0x0001;
	}
}