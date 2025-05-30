using System.Text.Json;
using EvacuationPlanningandMonitoringAPI.Models;
using EvacuationPlanningandMonitoringAPI.Repositories.Interface;
using StackExchange.Redis;

namespace EvacuationPlanningandMonitoringAPI.Repositories
{
    public class RedisRepository : IRedisRepository
    {
        private readonly IDatabase _redis;
        public RedisRepository(IDatabase redis) {
            _redis = redis;
        }

        public async Task<List<RedisSyncData?>> GetAsync(string key)
        {
            var keys = await _redis.SetMembersAsync(key);
            if (keys.Length == 0) return new List<RedisSyncData>();

            RedisKey[] redisKeys = keys.Select(x => (RedisKey)x.ToString()).ToArray();
            var values = await _redis.StringGetAsync(redisKeys);

            var result = new List<RedisSyncData>();
            foreach (var value in values)
            {
                if (!value.IsNullOrEmpty)
                {
                    var obj = JsonSerializer.Deserialize<RedisSyncData>(value);
                    if (obj != null) result.Add(obj);
                }
            }

            return result;
        }

        public async Task SetAsync(RedisSyncData data,string key)
        {
            var json = JsonSerializer.Serialize(data);
            await _redis.StringSetAsync(data.Key, json);
            await _redis.SetAddAsync(key, data.Key);
        }

        public async Task DeleteAsync(string key)
        {
            var members = await _redis.SetMembersAsync(key);
            if (members.Length == 0) return;

            RedisKey[] redisKeys = members.Select(x => (RedisKey)x.ToString()).ToArray();

            await _redis.KeyDeleteAsync(redisKeys);
            await _redis.KeyDeleteAsync(key);
        }

    }
}
