using System;

namespace Plugin.EventLog.Data
{
	internal static class Utils
	{
		public static Boolean IsBitSet(UInt32 flags, Int32 bit)
			=> (flags & Convert.ToInt32(System.Math.Pow(2, bit))) != 0;

		public static UInt32[] BitToInt(params Boolean[] bits)
		{
			UInt32[] result = new UInt32[] { };
			Int32 counter = 0;
			for(Int32 loop = 0; loop < bits.Length; loop++)
			{
				if(result.Length <= loop)//Увеличиваю массив на один, если не помещается значение
					Array.Resize<UInt32>(ref result, result.Length + 1);

				for(Int32 innerLoop = 0; innerLoop < 32; innerLoop++)
				{
					result[loop] |= Convert.ToUInt32(bits[counter++]) << innerLoop;
					if(counter >= bits.Length)
						break;
				}
				if(counter >= bits.Length)
					break;
			}
			return result;
		}
	}
}