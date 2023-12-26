using Server.Svc.UnitTests.Extensions;

namespace Server.Svc.UnitTests.Helpers
{
	public class ResponseExpectation
	{
		public ResponseExpectation() { }

		public ResponseExpectation(object expectedDomainObject)
		{
			ExpectedDomainObject = expectedDomainObject;
		}

		public object ExpectedDomainObject { get; set; }

		public override bool Equals(object obj)
		{
			var expectedResultType = ExpectedDomainObject.GetType();
			var actualResultType = obj.GetType();

			var expectedResultProperties = expectedResultType.GetProperties();
			
			foreach (var expectedResultProperty in expectedResultProperties)
			{
				var actualResultProperty = actualResultType.GetProperty(expectedResultProperty.Name);

				var expectedValue = expectedResultProperty.GetValue(ExpectedDomainObject);
				var actualValue = actualResultProperty.GetValue(obj);

				if (expectedResultProperty.PropertyType.IsArray && actualResultProperty.PropertyType.IsArray)
				{
					var expectedValueArray = expectedValue as byte[];
					var actualValueArray = actualValue as byte[];

					if (!expectedValueArray.ArrayEquals(actualValueArray))
					{
						return false;
					}

					continue;
				}

				if (expectedResultProperty.PropertyType.IsEnum && actualResultProperty.PropertyType.IsEnum)
				{
					var expectedEnumValue = (byte) expectedValue;
					var actualEnumValue = (byte) actualValue;

					if (expectedEnumValue != actualEnumValue)
					{
						return false;
					}
				}

				if (!expectedValue.Equals(actualValue))
				{
					return false;
				}
			}

			return true;
		}
	}
}