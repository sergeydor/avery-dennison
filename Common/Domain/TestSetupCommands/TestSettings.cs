using Common.Enums;
using System.Linq;

namespace Common.Domain.TestSetupCommands
{
	public class TestSettings
	{
        public Test10Type TestType { get; set; } = Test10Type.ReadTest;
        public TagClass TagClass { get; set; } = TagClass.EPC1Gen2;
        public AntPort AntPort { get; set; } = AntPort.Tx1Rx1;
        public short ReadPower { get; set; } = 0x0BB8;
        public ushort ReadTimeout { get; set; } = 0x0064;
        public uint Frequency1 { get; set; } = 0x3889CAC0;
		public uint Frequency2 { get; set; } = 0;
        public uint Frequency3 { get; set; } = 0;
        public short WritePower { get; set; } = 0x0BB8;
        public ushort WriteTimeout { get; set; } = 0x01F4;
        public WriteType WriteType { get; set; } = WriteType.IncrementGivenTagID;
        public byte[] StartTagID { get; set; } = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C }.Reverse().ToArray();
	}
}