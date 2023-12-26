using Common.Enums;

namespace Common.Domain.DeviceResults
{
	public class GeneralDeviceResult
	{
		public StatusCode Status { get; set; }
		public ushort Timer { get; set; }

        public override string ToString()
        {
            return " Status " + Status;
        }
    }
}