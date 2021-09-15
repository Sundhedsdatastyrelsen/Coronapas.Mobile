using System;
using System.Threading.Tasks;
using SSICPAS.Configuration;
using SSICPAS.Core.Data;

namespace SSICPAS.Utils
{
    public static class SecureStorageLargeDataHelper
    {
        public static async Task TryClear<TValue>(this ISecureStorageService<TValue> storage, string key)
        {
            ISecureStorageService<string> stringSecureStorageService =
                IoCContainer.Resolve<ISecureStorageService<string>>();

            try
            {
                if (await storage.HasValue(key))
                {
                    await storage.Clear(key);
                    return;
                }

                if (await stringSecureStorageService.HasLargeValueAsync(key))
                {
                    await stringSecureStorageService.RemoveLargeValue(key);
                }
            }
            catch (Exception)
            {
                if (await stringSecureStorageService.HasLargeValueAsync(key))
                {
                    await stringSecureStorageService.RemoveLargeValue(key);
                    return;
                }

                throw;
            }
        }

        public static async Task TrySetSecureStorageAsync<TValue>(this ISecureStorageService<TValue> service,
            string key, TValue data)
        {
            try
            {
                await service.SetSecureStorageAsync(key, data);
            }
            catch
            {
                ISecureStorageService<string> stringSecureStorageService =
                    IoCContainer.Resolve<ISecureStorageService<string>>();
                await stringSecureStorageService.SetLargeValue(key, data);
            }
        }

        public static async Task<TValue> TryGetSecureStorageAsync<TValue>(this ISecureStorageService<TValue> service,
            string key)
        {
            ISecureStorageService<string> stringSecureStorageService =
                IoCContainer.Resolve<ISecureStorageService<string>>();
            try
            {
                if (await service.HasValue(key))
                {
                    return await service.GetSecureStorageAsync(key);
                }

                if (await stringSecureStorageService.HasLargeValueAsync(key))
                {
                    return await stringSecureStorageService.GetLargeValue<TValue>(key);
                }
            }
            catch (Exception)
            {
                if (await stringSecureStorageService.HasLargeValueAsync(key))
                {
                    return await stringSecureStorageService.GetLargeValue<TValue>(key);
                }

                throw;
            }

            return default;
        }

        public static async Task<bool> TryHasValue<TValue>(this ISecureStorageService<TValue> service, string key)
        {
            ISecureStorageService<string> stringSecureStorageService =
                IoCContainer.Resolve<ISecureStorageService<string>>();
            return await service.HasValue(key) || await stringSecureStorageService.HasLargeValueAsync(key);
        }
    }
}