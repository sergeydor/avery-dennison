using System;
using System.Linq;

namespace Common.Domain.TestModuleCommands
{
	public class LaneDateTime
	{
        public byte Lane { get; set; } = 0;
		public byte Year { get; set; } = 0;
        public byte Month { get; set; } = 0;
        public byte Day { get; set; } = 0;
        public byte Hour { get; set; } = 0;
        public byte Minute { get; set; } = 0;
        public byte Second { get; set; } = 0;

        public void Shuffle(string macAddr)
        {
            byte v = (byte)(macAddr.Sum(c => (int)c) % 10);
            Lane += v;
            Year += v;
            Month += v;
            Day += v;
            Hour += v;
            Minute += v;
            Second += v;
        }
	}
}