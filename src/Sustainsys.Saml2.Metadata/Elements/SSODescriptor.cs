using Sustainsys.Saml2.Metadata.Xml;

namespace Sustainsys.Saml2.Metadata.Elements;


/// <summary>
/// Abstract SSODescriptor type.
/// </summary>
public abstract class SSODescriptor : RoleDescriptor
{
    /// <summary>
    /// Artifact resolution services.
    /// </summary>
    public List<IndexedEndpoint> ArtifactResolutionServices { get; } = new();

    /// <summary>
    /// Single logout services.
    /// </summary>
    public List<Endpoint> SingleLogoutServices { get; } = new();
}