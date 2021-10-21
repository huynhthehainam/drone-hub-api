using System;
using System.Text.Json;
using MiSmart.Infrastructure.Constants;
using MiSmart.Infrastructure.Settings;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
namespace MiSmart.Infrastructure.Services
{
    public class CacheService
    {
        private IDistributedCache distributedCache;
        private KeySettings keySettings;
        private ExpiredTimeSettings expiredTimeSettings;
        public CacheService(IDistributedCache distributedCache, IOptions<KeySettings> options1, IOptions<ExpiredTimeSettings> options2)
        {
            this.expiredTimeSettings = options2.Value;
            this.distributedCache = distributedCache;
            this.keySettings = options1.Value;
        }
        private String GetKey(Int64 id)
        {
            var key = $"{keySettings.AuthCacheKey}_!@{id}";
            return key;
        }
        public void SaveUserCache(UserCacheViewModel user)
        {
            var key = GetKey(user.ID);
            DistributedCacheEntryOptions distributedCacheEntryOptions = new DistributedCacheEntryOptions { AbsoluteExpiration = DateTime.Now.AddMinutes(expiredTimeSettings.AccessTokenExpirationTime), };
            distributedCache.SetString(key, JsonSerializer.Serialize(user), distributedCacheEntryOptions);
        }
        public UserCacheViewModel GetUserCache(Int64 id)
        {
            var key = GetKey(id);
            var result = distributedCache.GetString(key);
            if (String.IsNullOrEmpty(result))
            {
                return null;
            }
            else
            {
                return JsonSerializer.Deserialize<UserCacheViewModel>(result);
            }
        }
        public void RemoveUserCache(Int64 id)
        {
            var key = GetKey(id);
            distributedCache.Remove(key);
        }
    }
}