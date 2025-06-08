using StackExchange.Redis;
using System.Text.Json;

namespace ApiServer.Service.Redis.Room
{
    public class RedisRoomService
    {
        private readonly IDatabase _db;
        public RedisRoomService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<List<string>> GetRoomListAsync()
        {
            var keys = new List<string>();
            var server = _db.Multiplexer.GetServer(_db.Multiplexer.GetEndPoints().First());
            foreach (var key in server.Keys(pattern: "room:*"))
            {
                keys.Add(key.ToString());
            }
            return await Task.FromResult(keys);
        }

        public async Task<bool> CreateRoomAsync(string roomId, string jsonRoomData)
        {
            var key = $"room:{roomId}";
            return await _db.StringSetAsync(key, jsonRoomData);
        }

        public async Task<string?> GetRoomAsync(string roomId)
        {
            return await _db.StringGetAsync($"room:{roomId}");
        }

    }
}
