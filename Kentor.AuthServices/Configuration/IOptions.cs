namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Root interface for the options objects, handling all configuration of
    /// AuthServices.
    /// </summary>
    public interface IOptions
    {
        /// <summary>
        /// Options for the service provider's behaviour; i.e. everything except
        /// the idp list and the notifications.
        /// </summary>
        SPOptions SPOptions { get; }

        /// <summary>
        /// Information about known identity providers.
        /// </summary>
        IdentityProviderDictionary IdentityProviders { get; }

        /// <summary>
        /// Set of callbacks that can be used as extension points for various
        /// events.
        /// </summary>
        KentorAuthServicesNotifications Notifications { get; }
    }
}
