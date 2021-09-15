using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSICPAS.Core.Data;

namespace SSICPAS.Tests.TestMocks
{
    
    public class MockSecureStorageServiceThrowingOnTooLargeValue<TValue> : ISecureStorageService<TValue>
    {
        private Dictionary<string, string> _dict = new Dictionary<string, string>();

        public MockSecureStorageServiceThrowingOnTooLargeValue()
        {
        }

        public async Task<TValue> GetSecureStorageAsync(string key)
        {
            if (_dict.ContainsKey(key)) {
                var dictValue = _dict[key];
                if (dictValue.Length > 240) throw new Exception("Too large data");
                return dictValue != default ? JsonConvert.DeserializeObject<TValue>(dictValue) : default(TValue);
            }

            return default(TValue);
        }

        public async Task SetSecureStorageAsync(string key, TValue value)
        {
            string dictValue = JsonConvert.SerializeObject(value);
            if (dictValue.Length > 240) throw new Exception("Too large data");
            _dict[key] = dictValue;
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
            if (_dict.ContainsKey(key)) {
                var dictValue = _dict[key];
                if (dictValue.Length > 240) throw new Exception("Too large data");
            }
            return await Task.FromResult<bool>(_dict.Remove(key));
        }
    }
}