using Newtonsoft.Json;
using SSICPAS.Core.Logging;
using SSICPAS.Core.CustomExceptions;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace SSICPAS.Core.Data
{
    public class SecureStorageService<TValue> : ISecureStorageService<TValue>
    {
        private readonly ILoggingService _loggingService;

        public SecureStorageService(ILoggingService loggingService)
        {
            _loggingService = loggingService;
        }

        public async Task<TValue> GetSecureStorageAsync(string key)
        {
            try
            {
                var store = await SecureStorage.GetAsync(key);

                return store != default ? JsonConvert.DeserializeObject<TValue>(store) : default(TValue);
            }
            catch (Exception e)
            {
                throw new FailedOperationSecureStorageException($"Error[{nameof(SecureStorage)}] Error Getting Key {key} in {nameof(GetSecureStorageAsync)}", e);
            }
        }

        public async Task SetSecureStorageAsync(string key, TValue value)
        {
            try
            {
                await SecureStorage.SetAsync(key, JsonConvert.SerializeObject(value));
            }
            catch (Exception e)
            {
                throw new FailedOperationSecureStorageException($"Error[{nameof(SecureStorage)}] Error Setting Key {key} in {nameof(SetSecureStorageAsync)}", e);
            }
        }

        public async Task<bool> HasValue(string key)
        {
            return await GetSecureStorageAsync(key) != null;
        }

        public async Task<bool> Clear(string key)
        {
            return await Task.FromResult<bool>(SecureStorage.Remove(key));
        }
    }
}