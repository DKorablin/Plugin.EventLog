using System;

namespace Plugin.EventLog.Data
{
	internal static class ObjectPropertyParser
	{
		/// <summary>Получить массив свойст объекта сохранённые в строку</summary>
		/// <param name="objectType">Тип объекта</param>
		/// <param name="settingsString">Строка с настройками из которой получить масив свойств</param>
		/// <returns>Масив свойств</returns>
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

		/// <summary>Создать строку из свойств</summary>
		/// <param name="objectType">Тип объекта</param>
		/// <param name="properties">Свойства</param>
		/// <param name="settingsString">Старая строка с настройками</param>
		/// <returns>Новая строка с настройками</returns>
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