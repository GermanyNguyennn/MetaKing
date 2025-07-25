﻿namespace MetaKing.BackendServer.Services
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);

        Task SetAsync<T>(string key, T value, int timeDurationInHours = 0);

        Task RemoveAsync(string key);
    }
}
