using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata.Attributes;

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
    /// validUntil attribute name.
    /// </summary>
    public const string validUntil = nameof(validUntil);

    /// <summary>
    /// Binding attibute name.
    /// </summary>
    public const string Binding = nameof(Binding);

    /// <summary>
    /// Location attribute name.
    /// </summary>
    public const string Location = nameof(Location);

    /// <summary>
    /// protocolSupportEnumeration attribute name.
    /// </summary>
    public const string protocolSupportEnumeration = nameof(protocolSupportEnumeration);

    /// <summary>
    /// use attribute name.
    /// </summary>
    public const string use = nameof(use);

    /// <summary>
    /// index attribute name.
    /// </summary>
    public const string index = nameof(index);

    /// <summary>
    /// isDefault attribute name.
    /// </summary>
    public const string isDefault = nameof(isDefault);

    /// <summary>
    /// WantAuthnRequestsSigned attribute name.
    /// </summary>
    public const string WantAuthnRequestsSigned = nameof(WantAuthnRequestsSigned);
}
