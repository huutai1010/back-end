namespace Common.Interfaces
{
    public interface IRedisCacheService
    {

        Task<TResult?> Get<TResult>(string key);
        Task RemoveAsync(string key);
        Task SaveCacheAsync<TResult>(string key, TResult jsonData);
        Task SaveCacheAsync<TResult>(string key, TResult jsonData, double? absoluteExpiration = null, double? slidingExpiration = null);
        Task SaveCachePermanentAsync<TResult>(string key, TResult jsonData);

    }
}
