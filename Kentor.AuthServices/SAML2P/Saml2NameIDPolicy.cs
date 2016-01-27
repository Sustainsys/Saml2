namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// The NameId policy.
    /// </summary>
    public class Saml2NameIdPolicy
    {
        /// <summary>
        /// Value of AllowCreate attribute. Set to null to omit.
        /// </summary>
        public bool? AllowCreate { get; set; }

        /// <summary>
        /// The NameId format.
        /// </summary>
        public NameIdFormat Format { get; set; }
    }
}