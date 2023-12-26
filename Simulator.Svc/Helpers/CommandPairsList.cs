using System.Collections.Generic;
using System.Linq;
using Simulator.Svc.Context;

namespace Simulator.Svc.Helpers
{
	public class CommandPairsList
	{
		private static readonly List<CommandPair> Pairs = new List<CommandPair>
		{
			new CommandPair(null, 0x0),
			new CommandPair(null, 0x01),
			new CommandPair(0x06, null),
			new CommandPair(0x92, 0x93),
			new CommandPair(0x08, 0x09),
			new CommandPair(0x10, 0x30),
			new CommandPair(0x18, 0x38),
			new CommandPair(0x19, 0x39),
			new CommandPair(0x15, 0x35),
			new CommandPair(0x1A, 0x3A),
			new CommandPair(0x14, 0x34),
			new CommandPair(0x17, 0x37),
			new CommandPair(0x11, 0x31),
			new CommandPair(0x20, 0x40),
			new CommandPair(0x12, 0x32),
			new CommandPair(0x1C, 0x3C),
			new CommandPair(0x1D, 0x3D),
			new CommandPair(0x63, 0x03)
		};

		public static CommandPair GetCommandPair(byte commandId)
		{
			CommandPair pair = Pairs.FirstOrDefault(x => x.Setter == commandId);
			if (pair != null)
			{
				pair.IsSet = true;
				return pair;
			}

			pair = Pairs.FirstOrDefault(x => x.Getter == commandId);
			if (pair != null)
			{
				pair.IsSet = false;
			}

			return pair;
		}
	}
}