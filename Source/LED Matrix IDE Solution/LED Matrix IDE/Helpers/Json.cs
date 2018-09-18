using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LedMatrixIde.Helpers
{
	public static class Json
	{
		public static Task<T> ToObjectAsync<T>(string value)
		{
			return Task.FromResult(JsonConvert.DeserializeObject<T>(value));
		}

		public static Task<string> StringifyAsync(object value)
		{
			return Task.FromResult(JsonConvert.SerializeObject(value));
		}
	}
}
