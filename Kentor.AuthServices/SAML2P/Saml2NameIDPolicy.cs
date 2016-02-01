namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// The NameId policy.
    /// </summary>
    /// <remarks>The class is used in created AuthnRequests, so it is
    /// immutable to avoid unintended changes.</remarks>
    public class Saml2NameIdPolicy
    {
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="allowCreate"></param>
        /// <param name="format"></param>
        public Saml2NameIdPolicy(bool? allowCreate, NameIdFormat format)
        {
            AllowCreate = allowCreate;
            Format = format;
        }

        /// <summary>
        /// Value of AllowCreate attribute. Set to null to omit.
        /// </summary>
        public bool? AllowCreate { get; }

        /// <summary>
        /// The NameId format.
        /// </summary>
        public NameIdFormat Format { get; }
    }
}