using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SSICPAS.Configuration;
using SSICPAS.Core.CustomExceptions;
using SSICPAS.Core.Data;
using Xamarin.Essentials;
using Xamarin.Forms.Internals;

namespace SSICPAS.Utils
{
    public static class StringChunkExtension
    {
        public static IEnumerable<string> Chunked(this string str, int maxChunkSize) {
            for (int i = 0; i < str.Length; i += maxChunkSize) 
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length-i));
        }
    } 
    
    public static class SecureStorageHelper
    {
        private static int ChunkSize = 240;
        public static string NumberOfChunkKeys(string key) => $"{key}_numberOfChunks";
        
        public static async Task SetLongStringValue(this ISecureStorageService<string> storage, string key, string value)
        {
            try
            {
                List<string> chunks = value.Chunked(ChunkSize).ToList();
                ISecureStorageService<int> intStorageService = IoCContainer.Resolve<ISecureStorageService<int>>();
                await intStorageService.TrySetSecureStorageAsync(NumberOfChunkKeys(key), chunks.Count);
            
                chunks
                    .Select((chunk, index) => (chunk, index))
                    .ForEach(async c => await storage.TrySetSecureStorageAsync($"{key}{c.index}", c.chunk));
            }
            catch (Exception e)
            {
                throw new FailedOperationSecureStorageException($"Error[{nameof(SecureStorage)}] Error setting Key {key} in {nameof(SetLongStringValue)}", e);
            }
        }

        public static async Task<string> GetLongStringValue(this ISecureStorageService<string> storage, string key)
        {
            try
            {
                ISecureStorageService<int> intStorageService = IoCContainer.Resolve<ISecureStorageService<int>>();
                int numberOfChunks = await intStorageService.TryGetSecureStorageAsync(NumberOfChunkKeys(key));
                if (numberOfChunks == 0) return null;

                return await Enumerable
                    .Range(0, numberOfChunks)
                    .Select(async index => await storage.TryGetSecureStorageAsync($"{key}{index}"))
                    .Aggregate(async (accumulator, chunk) => await accumulator + await chunk);
            }
            catch (Exception e)
            {
                throw new FailedOperationSecureStorageException($"Error[{nameof(SecureStorage)}] Error getting Key {key} in {nameof(GetLongStringValue)}", e);
            }
        }
        
        public static async Task RemoveLongStringValue(this ISecureStorageService<string> storage, string key)
        {
            try
            {
                ISecureStorageService<int> intStorageService = IoCContainer.Resolve<ISecureStorageService<int>>();

                int numberOfChunks = await intStorageService.TryGetSecureStorageAsync(NumberOfChunkKeys(key));

                Enumerable
                    .Range(0, numberOfChunks)
                    .ForEach(async index => await storage.TryClear($"{key}{index}"));

                await intStorageService.TryClear(NumberOfChunkKeys(key));
            }
            catch (Exception e)
            {
                throw new FailedOperationSecureStorageException($"Error[{nameof(SecureStorage)}] Error Removing Key {key} in {nameof(RemoveLongStringValue)}", e);
            }
        }

        public static async Task<bool> HasLongStringValueAsync(this ISecureStorageService<string> _, string key)
        {
            try
            {
                ISecureStorageService<int> intStorageService = IoCContainer.Resolve<ISecureStorageService<int>>();
                return await intStorageService.GetSecureStorageAsync(NumberOfChunkKeys(key)) != default(int);
            }
            catch
            {
                return false;
            }
        }

        public static async Task<TValue> GetLargeValue<TValue>(this ISecureStorageService<string> storage, string key)
        {
            var longStringValue = await storage.GetLongStringValue(key);
            return Deserialize<TValue>(longStringValue);
        }

        public static async Task SetLargeValue<TValue>(this ISecureStorageService<string> storage, string key,
            TValue value)
        {
            await storage.SetLongStringValue(key, Serialize(value));
        }

        public static async Task RemoveLargeValue(this ISecureStorageService<string> storage, string key)
        {
            await storage.RemoveLongStringValue(key);
        }

        public static async Task<bool> HasLargeValueAsync(this ISecureStorageService<string> storage, string key)
        {
            return await storage.HasLongStringValueAsync(key);
        }
        
        private static TValue Deserialize<TValue>(string data)
        {
            return data is null ?
                default(TValue) :
                JsonConvert.DeserializeObject<TValue>(data);
        }
        
        private static string Serialize<TValue>(TValue data)
        {
            return JsonConvert.SerializeObject(data);
        }
    }
}