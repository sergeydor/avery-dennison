namespace Common.Enums
{
	public enum WriteType : byte
	{
		UseCurrentTagID = 0,
		IncrementGivenTagID = 1,
		UseDateTimeValues = 2,
		StrapData = 3
	}
}