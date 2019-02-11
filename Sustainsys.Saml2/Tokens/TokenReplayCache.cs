#if NETSTANDARD2_0
using Microsoft.Extensions.Caching.Memory;
#else
using System.Runtime.Caching;
#endif
using Microsoft.IdentityModel.Tokens;
using System;

namespace Sustainsys.Saml2.Tokens
{
	class TokenReplayCache : ITokenReplayCache
	{
#if NETSTANDARD2_0
        MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
#else
        MemoryCache cache = new MemoryCache(nameof(TokenReplayCache));
#endif

        // Dummy object to store in cache.
        private static object cacheObject = new object();

		public bool TryAdd(string securityToken, DateTime expiresOn)
		{
#if NETSTANDARD2_0
            cache.Set(securityToken, cacheObject, new DateTimeOffset(expiresOn));
#else
            cache.Add(securityToken, cache, new DateTimeOffset(expiresOn));
#endif
            return true;
        }

        public bool TryFind(string securityToken)
		{
            return cache.Get(securityToken) != null;
		}
	}
}
