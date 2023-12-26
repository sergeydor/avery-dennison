namespace Common.Enums.GSTCommands
{
	public enum PowerMeterRequest : byte
	{
		NoReq = 0,
		RefreshSendOne = 1,
		SendContinuously = 2,
		StopSending = 3
	}
}