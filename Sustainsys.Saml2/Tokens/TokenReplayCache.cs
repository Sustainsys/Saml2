#if NETFRAMEWORK
using System.Runtime.Caching;
#else
using Microsoft.Extensions.Caching.Memory;
#endif
using Microsoft.IdentityModel.Tokens;
using System;

namespace Sustainsys.Saml2.Tokens
{
	class TokenReplayCache : ITokenReplayCache
	{
#if NETFRAMEWORK
        readonly MemoryCache cache = new MemoryCache(nameof(TokenReplayCache));
#else
        readonly MemoryCache cache = new MemoryCache(new MemoryCacheOptions());
#endif

        // Dummy object to store in cache.
        private static readonly object cacheObject = new object();

		public bool TryAdd(string securityToken, DateTime expiresOn)
		{
#if NETFRAMEWORK
            cache.Add(securityToken, cache, new DateTimeOffset(expiresOn));
#else
            cache.Set(securityToken, cacheObject, new DateTimeOffset(expiresOn));
#endif
            return true;
        }

        public bool TryFind(string securityToken)
		{
            return cache.Get(securityToken) != null;
		}
	}
}
