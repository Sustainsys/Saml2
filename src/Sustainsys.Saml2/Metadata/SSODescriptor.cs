using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Metadata;


/// <summary>
/// Abstract SSODescriptor type.
/// </summary>
public abstract class SSODescriptor : RoleDescriptor
{
    /// <summary>
    /// Artifact resolution services.
    /// </summary>
    public List<IndexedEndpoint> ArtifactResolutionServices { get; } = [];

    /// <summary>
    /// Single logout services.
    /// </summary>
    public List<Endpoint> SingleLogoutServices { get; } = [];
}