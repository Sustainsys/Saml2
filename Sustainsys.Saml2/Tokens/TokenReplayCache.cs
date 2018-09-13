using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;
using System;

namespace Sustainsys.Saml2.Tokens
{
	class TokenReplayCache : ITokenReplayCache
	{
		MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
		static readonly object cacheObject = new object();

		public TimeSpan? ExpiryTime { get; set; }

		public TokenReplayCache()
		{
		}

		public TokenReplayCache(TimeSpan? expiryTime)
		{
			ExpiryTime = expiryTime;
		}

		public bool TryAdd(string securityToken, DateTime expiresOn)
		{
			object entry;
			if (cache.TryGetValue(securityToken, out entry))
			{
				return false;
			}
			
			if (ExpiryTime.HasValue)
			{
				DateTime expiryOverride = DateTime.UtcNow.Add(ExpiryTime.Value);
				expiresOn = expiresOn < expiryOverride ? expiresOn : expiryOverride;
			}

			cache.Set<object>(securityToken, cacheObject, expiresOn);
			return true;
		}

		public bool TryFind(string securityToken)
		{
			return cache.Get(securityToken) != null;
		}
	}
}
