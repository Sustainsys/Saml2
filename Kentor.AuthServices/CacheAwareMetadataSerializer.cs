using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Metadata;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices
{
    class CacheAwareMetadataSerializer : MetadataSerializer
    {
        TimeSpan cacheDuration;

        public CacheAwareMetadataSerializer(TimeSpan cacheDuration)
        {
            this.cacheDuration = cacheDuration;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Method is only called by base class no validation needed.")]
        protected override void WriteCustomAttributes<T>(XmlWriter writer, T source)
        {
            if (typeof(T) == typeof(EntityDescriptor))
            {
                writer.WriteAttributeString("cacheDuration", XmlConvert.ToString(cacheDuration));
            }
        }
    }
}
