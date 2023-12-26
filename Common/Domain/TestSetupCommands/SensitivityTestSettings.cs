using Common.Enums;

namespace Common.Domain.TestSetupCommands
{
	public class SensitivityTestSettings
	{
        public ReadWriteMode ReadWriteMode { get; set; } = ReadWriteMode.Read;
        public AntPort AntPort { get; set; } = AntPort.Tx1Rx1;
        public uint Frequency { get; set; } = 0x000DF638;
        public short MinPower { get; set; } = 0x0BB8;
        public short MaxPower { get; set; } = 0x0BB8;
        public byte SearchDepth { get; set; } = 0x01;
        public ushort Timeout { get; set; } = 0x01F4;
        public short PassThreshold { get; set; } = 0;
        //public SensitivityTestOptions Options { get; set; }
        public byte Options { get; set; } = 0;
    }
}