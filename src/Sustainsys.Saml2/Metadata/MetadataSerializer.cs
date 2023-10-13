using Sustainsys.Saml2.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Serializer for Saml2 Metadata
/// </summary>
public partial class MetadataSerializer : SerializerBase
{
    /// <summary>
    /// Ctor
    /// </summary>
    public MetadataSerializer()
    {
        NamespaceUri = Constants.Namespaces.Metadata;
        Prefix = "md";
    }
}
