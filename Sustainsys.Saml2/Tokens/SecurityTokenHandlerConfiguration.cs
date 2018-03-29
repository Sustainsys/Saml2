using System;
using System.Security.Cryptography.X509Certificates;
using Sustainsys.Saml2.Tokens;

using Microsoft.IdentityModel.Tokens;

namespace Sustainsys.Saml2.Configuration
{
    public class SecurityTokenHandlerConfiguration
    {
		public static readonly bool DefaultDetectReplayedTokens = true;
		public static readonly IssuerNameRegistry DefaultIssuerNameRegistry = new ConfigurationBasedIssuerNameRegistry();
		// TODO
		// public static readonly SecurityTokenResolver DefaultIssuerTokenResolver
		public static readonly TimeSpan DefaultMaxClockSkew = new TimeSpan(0, 5, 0);
		public static readonly bool DefaultSaveBootstrapContext = false;
		public static readonly TimeSpan DefaultTokenReplayCacheExpirationPeriod = TimeSpan.MaxValue;
		// TODO:
		// public static readonly X509CertificateValidationMode DefaultCertificateValidationMode = null; // IdentityConfiguration.DefaultCertificateValidationMode;
		public static readonly X509RevocationMode DefaultRevocationMode = IdentityConfiguration.DefaultRevocationMode;
		// public static readonly StoreLocation DefaultTrustedStoreLocation = IdentityConfiguration.DefaultTrustedStoreLocation;
		// public static readonly X509CertificateValidator DefaultCertificateValidator = X509Util.CreateCertificateValidator(DefaultCertificateValidationMode, DefaultRevocationMode, DefaultTrustedStoreLocation);

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

		// TODO:
		public AudienceRestriction AudienceRestriction { get; set; } = new AudienceRestriction();

		// private X509CertificateValidator certificateValidator = DefaultCertificateValidator;
		// private SecurityTokenResolver issuerTokenResolver = DefaultIssuerTokenResolver;

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

		// TODO:
		// private SecurityTokenResolver serviceTokenResolver = EmptySecurityTokenResolver.Instance;
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

		/// <summary>
		/// Creates an instance of <see cref="SecurityTokenHandlerConfiguration"/>
		/// </summary>
		public SecurityTokenHandlerConfiguration()
		{
		}
	}
}
