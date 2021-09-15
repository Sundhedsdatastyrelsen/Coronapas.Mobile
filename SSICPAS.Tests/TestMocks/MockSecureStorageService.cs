using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSICPAS.Core.Data;

namespace SSICPAS.Tests.TestMocks
{
    public class MockSecureStorageService<TValue> : ISecureStorageService<TValue>
    {
        private Dictionary<string, string> _dict = new Dictionary<string, string>();

        public MockSecureStorageService()
        {
        }

        public async Task<TValue> GetSecureStorageAsync(string key)
        {
            if (_dict.ContainsKey(key)) {
                var dictValue = _dict[key];
                return dictValue != default ? JsonConvert.DeserializeObject<TValue>(dictValue) : default(TValue);
            }

            return default(TValue);
        }

        public async Task SetSecureStorageAsync(string key, TValue value)
        {
            _dict[key] = JsonConvert.SerializeObject(value);
        }

        public async Task<bool> HasValue(string key)
        {
            if (typeof(TValue) == typeof(int))
            {
                return (int)(object)await GetSecureStorageAsync(key) != default;
            }
            return await GetSecureStorageAsync(key) != null;
        }

        public async Task<bool> Clear(string key)
        {
            return await Task.FromResult<bool>(_dict.Remove(key));
        }
    }
}
