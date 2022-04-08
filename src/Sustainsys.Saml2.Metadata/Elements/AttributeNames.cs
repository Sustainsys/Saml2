using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata.Elements;

/// <summary>
/// Names of attributes.
/// </summary>
/// <remarks>The naming of the constants are deriberately not following
/// casing convention in order to be exactly the same as the contents.
/// </remarks>
public static class AttributeNames
{
    /// <summary>
    /// entityID attribute name.
    /// </summary>
    public const string entityID = nameof(entityID);

    /// <summary>
    /// ID attribute name.
    /// </summary>
    public const string ID = nameof(ID);

    /// <summary>
    /// cacheDuration attribute name.
    /// </summary>
    public const string cacheDuration = nameof(cacheDuration);

    /// <summary>
    /// validUntil attribute Name.
    /// </summary>
    public const string validUntil = nameof(validUntil);
}
