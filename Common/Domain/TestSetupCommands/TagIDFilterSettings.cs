using System.Linq;

namespace Common.Domain.TestSetupCommands
{
	public class TagIDFilterSettings
	{
        public byte Options { get; set; } = 0;
        public byte[] NibbleEnable { get; set; } = new byte[] { 0xC0, 0, 0 }.Reverse().ToArray();
        public byte[] TagIDFilter { get; set; } = new byte[] { 0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A, 0x0B, 0x0C }.Reverse().ToArray();
    }
}