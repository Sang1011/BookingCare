using BookingCare.Application.Common.Interfaces.Services;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace BookingCare.Infrastructure.Cache
{
    public class RedisEmailVerificationService : IEmailVerificationService
    {
        private readonly IConnectionMultiplexer _redis;
        private const int ExpiryHours = 24;

        public RedisEmailVerificationService(IConnectionMultiplexer redis)
            => _redis = redis;

        private static string Key(Guid userId) => $"email:verify:{userId}";

        public async Task<string> GenerateTokenAsync(Guid userId, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            var hashedToken = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(token)));

            await db.StringSetAsync(
                Key(userId),
                hashedToken,
                TimeSpan.FromHours(ExpiryHours));

            return token;
        }

        public async Task<Guid?> ValidateTokenAsync(string token, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            var hashedToken = Convert.ToBase64String(
                SHA256.HashData(Encoding.UTF8.GetBytes(token)));

            var server = _redis.GetServer(_redis.GetEndPoints().First());
            var keys = server.Keys(pattern: "email:verify:*");

            foreach (var key in keys)
            {
                var value = await db.StringGetAsync(key);
                if (value == hashedToken)
                {
                    var userIdStr = key.ToString().Replace("email:verify:", "");
                    return Guid.TryParse(userIdStr, out var userId) ? userId : null;
                }
            }

            return null;
        }

        public async Task RemoveTokenAsync(Guid userId, CancellationToken ct = default)
        {
            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync(Key(userId));
        }
    }
}
