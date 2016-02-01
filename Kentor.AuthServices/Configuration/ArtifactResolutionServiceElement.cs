using System;
using System.Configuration;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Configuration of an artifact resolution service endpoint on an idp.
    /// </summary>
    public class ArtifactResolutionServiceElement : ConfigurationElement
    {
        const string index = nameof(index);
        /// <summary>
        /// Index of the artifact resolution service endpoint.
        /// </summary>
        [ConfigurationProperty(index, DefaultValue = 0)]
        public int Index
        {
            get
            {
                return (int)base[index];
            }
        }

        const string location = nameof(location);
        /// <summary>
        /// Location of the endpoint.
        /// </summary>
        [ConfigurationProperty(location, IsRequired = true)]
        public Uri Location
        {
            get
            {
                return (Uri)base["location"];
            }
        }
    }
}