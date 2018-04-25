using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Collections.Generic;
using System.Security.Claims;

namespace SampleIdentityServer4AspNetIdentity
{
	public class Config
	{
		public static IEnumerable<ApiResource> GetApiResources()
		{
			return new List<ApiResource>
			{
				new ApiResource("api1", "My API")
			};
		}

		public static IEnumerable<Client> GetClients()
		{
			return new List<Client>()
			{
				new Client
				{
					ClientId = "client",

					AllowedGrantTypes = GrantTypes.ClientCredentials,

					ClientSecrets = { new Secret("secret".Sha256()) },

					AllowedScopes = { "api1" }
				},

				new Client
				{
					ClientId = "ro.client",

					AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,

					ClientSecrets = { new Secret("secret".Sha256()) },

					AllowedScopes = { "api1" }
				},

				new Client
				{
					ClientId = "mvc",
					ClientName = "MVC Client",
					AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

					RequireConsent = false,

					ClientSecrets = { new Secret("secret".Sha256()) },

					RedirectUris = { "http://localhost:5002/signin-oidc" },
					PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"api1"
					},
					AllowOfflineAccess = true
				},

				new Client
				{
					ClientId = "electrode",
					ClientName = "Electrode Client App",
					AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,

					RequireConsent = true,

					ClientSecrets = { new Secret("secret".Sha256()) },

					RedirectUris = { "http://localhost:3000/login/oidc-callback" },
					PostLogoutRedirectUris = { "http://localhost:3000/login/signout-oidc-callback" },
					
					// AllowedGrantTypes = GrantTypes.Implicit,
					// AllowAccessTokensViaBrowser = true,
					// AllowedCorsOrigins = { "http://localhost:3000" },

					AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"api1"
					},
					AllowOfflineAccess = true
				}
			};
		}

		public static List<TestUser> GetUsers()
		{
			return new List<TestUser>()
			{
				new TestUser
				{
					SubjectId = "1",
					Username = "alice",
					Password = "password",

					Claims = new []
					{
						new Claim("name", "Alice"),
						new Claim("website", "https://alice.com")
					}
				},
				new TestUser
				{
					SubjectId = "2",
					Username = "bob",
					Password = "password",

					Claims = new []
					{
						new Claim("name", "Bob"),
						new Claim("website", "https://bob.com")
					}
				}
			};
		}

		public static IEnumerable<IdentityResource> GetIdentityResources()
		{
			return new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile()
			};
		}
	}
}
