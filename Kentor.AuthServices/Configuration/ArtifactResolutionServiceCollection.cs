using Kentor.AuthServices.Internal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.Configuration
{
    /// <summary>
    /// Config collection of ArtifactResolutionElements.
    /// </summary>
    public class ArtifactResolutionServiceCollection : ConfigurationElementCollection, IEnumerable<ArtifactResolutionServiceElement>
    {
        /// <summary>
        /// Factory for element type.
        /// </summary>
        /// <returns></returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ArtifactResolutionServiceElement();
        }

        /// <summary>
        /// Get an identifying key of the element.
        /// </summary>
        /// <param name="element">Element</param>
        /// <returns>Index of endpoint</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ArtifactResolutionServiceElement)element).Index;
        }

        /// <summary>
        /// Generic enumerator.
        /// </summary>
        /// <returns>Generic enumerator</returns>
        public new IEnumerator<ArtifactResolutionServiceElement> GetEnumerator()
        {
            return base.GetEnumerator().AsGeneric<ArtifactResolutionServiceElement>();
        }
    }
}
