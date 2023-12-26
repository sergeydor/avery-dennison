using System;
using System.Collections;
using System.Text;

namespace Common.Infrastructure.Extensions
{
	public static class LogExtensions
	{
		public static string ToLog(this object obj, int leftPadding = 0)
		{
			if (obj == null) return "null";
			var stringBuilder = new StringBuilder();

            try
            {
                if(obj is IDictionary)
                {
                    Tuple<object> objArr = new Tuple<object>(obj);
                    AppendObjectLog(stringBuilder, objArr, leftPadding, 1);
                }
                else
                {
                    AppendObjectLog(stringBuilder, obj, leftPadding, 1);
                }                
            }
            catch
            {
                return "Error when parsing object (ToLog). Object Type = " + obj.GetType().ToString();
            }
			return stringBuilder.ToString();
		}

		private static void AppendObjectLog(StringBuilder stringBuilder, object obj, int padding, int hierarchyLevel)
		{
			bool addedFirstLine = hierarchyLevel != 1;

			var type = obj.GetType();
			var properties = type.GetProperties();

			foreach (var property in properties)
			{
				if (addedFirstLine)
				{
					stringBuilder.AppendLine();
					stringBuilder.Append("".PadLeft(hierarchyLevel * padding));
				}
				else
				{
					addedFirstLine = true;
				}

				stringBuilder.Append(property.Name);
				stringBuilder.Append(": ");

				var propertyValue = obj.GetPropValue(property.Name);

				if (propertyValue != null)
				{
					var list = propertyValue as IList;
					if (list != null)
					{
						for (int i = 0; i < list.Count; i++)
						{
							stringBuilder.AppendLine();
							object item = list[i];
							var value = string.Empty;
							if (item is string)
							{
								value = $"\"{item}\"";
							}
							else if (item.GetType().IsValueType)
							{
								value = item.ToString();
							}
							else
							{
								AppendObjectLog(stringBuilder, item, padding, hierarchyLevel + 1);
							}
							stringBuilder.AppendFormat("{0}: {1}", i, value);
						}
						continue;
					}

					var dictionary = propertyValue as IDictionary;
					if (dictionary != null)
					{
						foreach (var key in dictionary.Keys)
						{
							object item = dictionary[key];
							var value = string.Empty;
							if (item is string)
							{
								value = $"\"{item}\"";
								stringBuilder.Append($"{key}: {value}");
							}
							else if (item.GetType().IsValueType)
							{
								value = item.ToString();
								stringBuilder.Append($"{key}: {value}");
							}
							else
							{
								stringBuilder.Append($"{key}: ");
								AppendObjectLog(stringBuilder, item, padding, hierarchyLevel + 1);
							}
							stringBuilder.AppendLine();
						}
						continue;
					}

					var valueType = propertyValue.GetType();
					var namesps = valueType.Namespace;
					if (namesps != null && (namesps.Contains("Common") || namesps.Contains("Client.Server.Communication") || namesps.Contains("Server.Device.Communication")) && !(propertyValue is Enum))
					{
						AppendObjectLog(stringBuilder, propertyValue, padding, hierarchyLevel + 1);
					}
					else
					{
						stringBuilder.Append(propertyValue);
					}
				}
				else
				{
					stringBuilder.Append("null");
				}
			}
		}

		private static object GetPropValue(this Object obj, String name)
		{
			foreach (var part in name.Split('.'))
			{
				if (obj == null) { return null; }

				var type = obj.GetType();
				var info = type.GetProperty(part);

				if (info == null) { return null; }

				obj = info.GetValue(obj, null);
			}
			return obj;
		}
	}
}