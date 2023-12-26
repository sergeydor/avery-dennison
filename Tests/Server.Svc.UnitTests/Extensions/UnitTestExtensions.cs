namespace Server.Svc.UnitTests.Extensions
{
	internal static class UnitTestExtensions
	{
		public static bool ArrayEquals(this byte[] sourceArray, byte[] destinationArray)
		{
			if (sourceArray.Length != destinationArray.Length)
				return false;

			for (int i = 0; i < sourceArray.Length; i++)
			{
				if (sourceArray[i] != destinationArray[i]) return false;
			}

			return true;
		}
	}
}
