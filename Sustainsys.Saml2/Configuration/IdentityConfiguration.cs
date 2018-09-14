using Microsoft.IdentityModel.Tokens;
using Sustainsys.Saml2.Tokens;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.Configuration
{
	public class IdentityConfiguration
	{
		public const string DefaultServiceName = "";
		public static readonly TimeSpan DefaultMaxClockSkew = new TimeSpan(0, 5, 0);
		internal const string DefaultMaxClockSkewString = "00:05:00";
		public static readonly X509RevocationMode DefaultRevocationMode = X509RevocationMode.Online;
		public static readonly StoreLocation DefaultTrustedStoreLocation = StoreLocation.LocalMachine;

		string name;
		SecurityTokenHandlerConfiguration serviceHandlerConfiguration;

		public IdentityConfiguration(bool loadConfig)
		{
			if (loadConfig)
			{
				var section = SustainsysSaml2Section.Current;
				if (section == null)
				{
					throw new InvalidOperationException("No sustainsys.saml2 configuration element was found");
				}

				IdentityConfigurationElement element = section
					.IdentityConfigurations
					.IdentityConfigurationsCollection
					.GetElement(DefaultServiceName);
				LoadConfiguration(element);
			}
			else
			{
				LoadConfiguration(null);
			}

			if (serviceHandlerConfiguration == null)
			{
				serviceHandlerConfiguration = new SecurityTokenHandlerConfiguration();
				serviceHandlerConfiguration.MaxClockSkew = DefaultMaxClockSkew;
			}
		}

		public AudienceRestriction AudienceRestriction
		{
			get { return serviceHandlerConfiguration.AudienceRestriction; }
			set { serviceHandlerConfiguration.AudienceRestriction = value; }
		}

		protected void LoadConfiguration(IdentityConfigurationElement element)
		{
			if (element == null)
			{
				return;
			}

			name = element.Name;

			serviceHandlerConfiguration = LoadHandlerConfiguration(element);
		}

		protected SecurityTokenHandlerConfiguration LoadHandlerConfiguration(IdentityConfigurationElement element)
		{
			SecurityTokenHandlerConfiguration handlerConfiguration = new SecurityTokenHandlerConfiguration()
			{
				MaxClockSkew = element.MaximumClockSkew,
				SaveBootstrapContext = element.SaveBootstrapContext
			};

			if (element.AudienceUris != null)
			{
				handlerConfiguration.AudienceRestriction.AudienceMode = element.AudienceUris.Mode;
				foreach (AudienceUriElement audienceUriElement in element.AudienceUris)
				{
					handlerConfiguration.AudienceRestriction.AllowedAudienceUris.Add(
						new Uri(audienceUriElement.Value, UriKind.RelativeOrAbsolute));
				}
			}
			if (element.Caches != null)
			{
				if (element.Caches.TokenReplayCache != null &&
					element.Caches.TokenReplayCache.Type != null)
				{
					handlerConfiguration.TokenReplayCache = (ITokenReplayCache)
						Activator.CreateInstance(element.Caches.TokenReplayCache.Type);
				}
			}
			if (element.TokenReplayDetection != null)
			{
				handlerConfiguration.TokenReplayCacheExpirationPeriod =
					element.TokenReplayDetection.ExpirationPeriod;
				handlerConfiguration.DetectReplayedTokens =
					element.TokenReplayDetection.Enabled;

				if (handlerConfiguration.TokenReplayCache == null)
				{
					TimeSpan? expiryTime = null;
					if (handlerConfiguration.TokenReplayCacheExpirationPeriod > TimeSpan.Zero &&
						handlerConfiguration.TokenReplayCacheExpirationPeriod < TimeSpan.MaxValue)
					{
						expiryTime = handlerConfiguration.TokenReplayCacheExpirationPeriod;
					}
					handlerConfiguration.TokenReplayCache = new TokenReplayCache(expiryTime);
				}
			}

			return handlerConfiguration;
		}

		public TimeSpan MaxClockSkew
		{
			get { return serviceHandlerConfiguration.MaxClockSkew; }
			set { serviceHandlerConfiguration.MaxClockSkew = value; }
		}

		public string Name
		{
			get
			{
				return name;
			}
		}

		public X509RevocationMode RevocationMode
		{
			get { return serviceHandlerConfiguration.RevocationMode; }
			set { serviceHandlerConfiguration.RevocationMode = value; }
		}

		public bool SaveBootstrapContext
		{
			get { return serviceHandlerConfiguration.SaveBootstrapContext; }
			set { serviceHandlerConfiguration.SaveBootstrapContext = value; }
		}

		public bool DetectReplayedTokens
		{
			get { return serviceHandlerConfiguration.DetectReplayedTokens; }
			set { serviceHandlerConfiguration.DetectReplayedTokens = value; }
		}

		public TimeSpan TokenReplayCacheExpirationPeriod
		{
			get { return serviceHandlerConfiguration.TokenReplayCacheExpirationPeriod; }
			set { serviceHandlerConfiguration.TokenReplayCacheExpirationPeriod = value; }
		}

		public ITokenReplayCache TokenReplayCache
		{
			get { return serviceHandlerConfiguration.TokenReplayCache; }
			set { serviceHandlerConfiguration.TokenReplayCache = value; }
		}
	}
}
