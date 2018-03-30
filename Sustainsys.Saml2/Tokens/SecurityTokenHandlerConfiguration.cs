using Microsoft.IdentityModel.Tokens;
using Sustainsys.Saml2.Tokens;
using System;
using System.Security.Cryptography.X509Certificates;

namespace Sustainsys.Saml2.Configuration
{
	public class SecurityTokenHandlerConfiguration
    {
		public static readonly bool DefaultDetectReplayedTokens = true;
		public static readonly IssuerNameRegistry DefaultIssuerNameRegistry = new ConfigurationBasedIssuerNameRegistry();
		public static readonly TimeSpan DefaultMaxClockSkew = new TimeSpan(0, 5, 0);
		public static readonly bool DefaultSaveBootstrapContext = false;
		public static readonly TimeSpan DefaultTokenReplayCacheExpirationPeriod = TimeSpan.MaxValue;
		public static readonly X509RevocationMode DefaultRevocationMode = IdentityConfiguration.DefaultRevocationMode;

		public bool DetectReplayedTokens { get; set; } = DefaultDetectReplayedTokens;

		private IssuerNameRegistry issuerNameRegistry = DefaultIssuerNameRegistry;

		public IssuerNameRegistry IssuerNameRegistry
		{
			get { return issuerNameRegistry; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException(nameof(value));
				}
				issuerNameRegistry = value;
			}
		}

		public AudienceRestriction AudienceRestriction { get; set; } = new AudienceRestriction();

		private TimeSpan maxClockSkew = DefaultMaxClockSkew;
		public TimeSpan MaxClockSkew
		{
			get { return maxClockSkew; }
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				maxClockSkew = value;
			}
		}

		public bool SaveBootstrapContext { get; set; } = DefaultSaveBootstrapContext;
		public X509RevocationMode RevocationMode { get; set; } = DefaultRevocationMode;

		private TimeSpan tokenReplayCacheExpirationPeriod = DefaultTokenReplayCacheExpirationPeriod;

		public TimeSpan TokenReplayCacheExpirationPeriod
		{
			get { return tokenReplayCacheExpirationPeriod; }
			set
			{
				if (value <= TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value");
				}
				tokenReplayCacheExpirationPeriod = value;
			}
		}

		public ITokenReplayCache TokenReplayCache { get; set; } = new TokenReplayCache();

		public SecurityTokenHandlerConfiguration()
		{
		}
	}
}
