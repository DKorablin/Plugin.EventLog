using System;

namespace Plugin.EventLog.Data
{
	internal static class ObjectPropertyParser
	{
		/// <summary>Get the array of object properties saved to a string</summary>
		/// <param name="objectType">Object type</param>
		/// <param name="settingsString">Settings string from which to get the array of properties</param>
		/// <returns>Array of properties</returns>
		public static String[] GetPropertiesFromString(Type objectType, String settingsString)
		{
			Int32 indexStart = (settingsString ?? String.Empty).IndexOf(objectType.Name + ":");
			if(indexStart > -1)
			{
				indexStart += objectType.Name.Length + 1;
				Int32 indexEnd = settingsString.IndexOf(';', indexStart);
				String columnOrderString = settingsString.Substring(indexStart, indexEnd - indexStart);
				return columnOrderString.Split(new Char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
			} else
				return new String[] { };
		}

		/// <summary>Create a string from properties</summary>
		/// <param name="objectType">Object type</param>
		/// <param name="properties">Properties</param>
		/// <param name="settingsString">Old settings string</param>
		/// <returns>New settings string</returns>
		public static String SetPropertiesToString(Type objectType, String[] properties, String settingsString)
		{
			if(settingsString == null)
				settingsString = String.Empty;

			Type type = objectType;
			String typeName = type.Name;

			String result = String.Join(",", properties);

			Int32 startIndex = settingsString.IndexOf(typeName);
			if(startIndex > -1)
			{
				Int32 endIndex = settingsString.IndexOf(';', startIndex);
				settingsString = settingsString.Remove(startIndex, endIndex + 1 - startIndex);
			}

			return result.Length == 0
				? settingsString
				: settingsString + typeName + ":" + result + ';';
		}
	}
}