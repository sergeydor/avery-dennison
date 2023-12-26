using Server.Device.Communication;
using Server.Svc.UnitTests.Extensions;

namespace Server.Svc.UnitTests.Helpers
{
	public class CommandExpectation
	{
		public CommandExpectation() { }

		public CommandExpectation(byte sof, byte len, byte cmd, byte[] data, byte fcs)
		{
			SOF = sof;
			LEN = len;
			CMD = cmd;
			DATA = data;
			FCS = fcs;
		}

		public byte SOF { get; set; }
		public byte LEN { get; set; }
		public byte CMD { get; set; }
		public byte[] DATA { get; set; }
		public byte FCS { get; set; }

		public override bool Equals(object obj)
		{
			var command = obj as CommandData;

			if (command == null)
			{
				return base.Equals(obj);
			}

			var result = this.SOF == command.SOF
				&& this.LEN == command.LEN
				&& this.CMD == command.CMD
				&& DATA.ArrayEquals(command.DATA)
				&& this.FCS == command.FCS;

			return result;
		}
	}
}