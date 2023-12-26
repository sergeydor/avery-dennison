using Common.Enums;
using MongoDB.Bson;

namespace Common.Domain.TestSetupCommands
{
	public class AuxSettings
	{
        public AuxSettingsFunction Function { get; set; } = AuxSettingsFunction.QuadrantureEncoder;
		public byte Option1 { get; set; } = 0x0;
		public byte Option2 { get; set; } = 0x0;
        public EdgeType EdgeType { get; set; } = EdgeType.Rising;
        public byte Debounce { get; set; } = 0x01;
        public byte DeafTime { get; set; } = 0x01;
	}
}