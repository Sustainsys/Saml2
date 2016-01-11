namespace Kentor.AuthServices
{
    /// <summary>
    /// How should we override the metadata publishing rules
    /// </summary>
    public enum MetadataPublishOverrideType
    {
        /// <summary>
        /// No override. Published according to the normal rules.
        /// </summary>
        None = 0,

        /// <summary>
        /// Publish as Unspecified
        /// </summary>
        PublishUnspecified = 1,

        /// <summary>
        /// Publish as Encryption
        /// </summary>
        PublishEncryption = 2,

        /// <summary>
        /// Publish as Signing
        /// </summary>
        PublishSigning = 3,

        /// <summary>
        /// Do not publish
        /// </summary>
        DoNotPublish = 4
    }
}