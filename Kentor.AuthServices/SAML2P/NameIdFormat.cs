namespace Kentor.AuthServices.Saml2P
{
    /// <summary>
    /// The NameId Format.
    /// </summary>
    public enum NameIdFormat
    {
        /// <summary>
        /// 8.3.1 Unspecified
        /// </summary>
        Unspecified,

        /// <summary>
        /// 8.3.2 Email Address
        /// </summary>
        EmailAddress,

        /// <summary>
        /// 8.3.3 X.509 Subject Name
        /// </summary>
        X509SubjectName,

        /// <summary>
        /// 8.3.4 Windows Domain Qualified Name
        /// </summary>
        WindowsDomainQualifiedName,

        /// <summary>
        /// 8.3.5 Kerberos Principal Name
        /// </summary>
        KerberosPrincipalName,

        /// <summary>
        /// 8.3.6 Entity Identifier
        /// </summary>
        EntityIdentifier,

        /// <summary>
        /// 8.3.7 Persistent Identifier
        /// </summary>
        Persistent,

        /// <summary>
        /// 8.3.8 Transient Identifier
        /// </summary>
        Transient
    }
}