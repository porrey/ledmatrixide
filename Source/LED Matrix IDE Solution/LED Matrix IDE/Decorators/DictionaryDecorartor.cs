using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LedMatrixIde.Decorators
{
	public static class DictionaryDecorartor
	{
		public static Task<TCast> TryGetValue<TKey, TValue, TCast>(this Dictionary<TKey, TValue> dictionary, TKey key, TCast defaultValue)
		{
			TCast returnValue = defaultValue;

			if (dictionary.ContainsKey(key))
			{
				returnValue= (TCast)Convert.ChangeType(dictionary[key], typeof(TCast));
			}

			return Task.FromResult(returnValue);
		}
	}
}
