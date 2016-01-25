namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// The NameId policy.
    /// </summary>
    public class NameIDPolicy
    {
        /// <summary>
        /// True to set the AllowCreate attribute to 1, false to set it to 0.
        /// </summary>
        public bool AllowCreate { get; set; } = false;

        /// <summary>
        /// The NameId format.
        /// </summary>
        public NameIdFormat NameIdFormat { get; set; } = NameIdFormat.Transient;
    }
}