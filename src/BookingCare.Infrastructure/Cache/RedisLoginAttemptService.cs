using BookingCare.Application.Common.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Infrastructure.Cache
{
    public class RedisLoginAttemptService : ILoginAttemptService
    {
        private readonly IConnectionMultiplexer _redis;
        private const int MaxAttempts = 5;
        private const int LockMinutes = 15;

        public RedisLoginAttemptService(IConnectionMultiplexer redis)
            => _redis = redis;

        private static string Key(string email) => $"login:failed:{email.ToLowerInvariant()}";

        public async Task<bool> IsLockedAsync(string email, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(Key(email));
            return value.HasValue && (int)value >= MaxAttempts;
        }

        public async Task<int> IncrementAsync(string email, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var key = Key(email);

            var attempts = await db.StringIncrementAsync(key);

            // Set TTL lần đầu tiên
            if (attempts == 1)
                await db.KeyExpireAsync(key, TimeSpan.FromMinutes(LockMinutes));

            return (int)attempts;
        }

        public async Task ResetAsync(string email, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(Key(email));
        }

        public async Task<int> GetFailedAttemptsAsync(string email, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var value = await db.StringGetAsync(Key(email));
            return value.HasValue ? (int)value : 0;
        }
    }
}
