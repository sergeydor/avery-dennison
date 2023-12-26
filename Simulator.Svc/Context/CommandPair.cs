namespace Simulator.Svc.Context
{
	public class CommandPair
	{
		public CommandPair() { }

		public CommandPair(byte? setter, byte? getter)
		{
			Setter = setter;
			Getter = getter;
		}

		public byte? Setter { get; set; }
		public byte? Getter { get; set; }
		public bool IsSet { get; set; }
	}
}