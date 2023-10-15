using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Sustainsys.Saml2.Samlp.Attributes;
using Sustainsys.Saml2.Samlp.Elements;
using Sustainsys.Saml2.Xml;

namespace Sustainsys.Saml2.Samlp;
partial class SamlpSerializer
{
    /// <summary>
    /// Read attributes of a status response
    /// </summary>
    /// <param name="source">Xml traverser</param>
    /// <param name="response">StatusResponse</param>
    protected virtual void ReadAttributes(XmlTraverser source, StatusResponseType response)
    {
        response.Id = source.GetRequiredAttribute(AttributeNames.ID);
        response.Version = source.GetRequiredAttribute(AttributeNames.Version);
        response.IssueInstant = source.GetRequiredDateTimeAttribute(AttributeNames.IssueInstant);
    }

    /// <summary>
    /// Read elements of abstract StatusResponseType
    /// </summary>
    /// <param name="source">XML Traverser</param>
    /// <param name="response">Response to populate</param>
    protected virtual void ReadElements(XmlTraverser source, StatusResponseType response)
    {
        source.MoveNext();

        if(source.EnsureName(NamespaceUri, ElementNames.Status))
        {
            response.Status = ReadStatus(source);
        }
    }

    /// <summary>
    /// Factory for SamlStatus
    /// </summary>
    /// <returns>SamlStatus</returns>
    protected virtual SamlStatus CreateSamlStatus() => new();

    /// <summary>
    /// Reads Status
    /// </summary>
    /// <param name="source">Xml Traverser</param>
    /// <returns>Status</returns>
    protected virtual SamlStatus ReadStatus(XmlTraverser source)
    {
        var result = CreateSamlStatus();

        ReadElements(source.GetChildren(), result);

        return result;
    }

    /// <summary>
    /// Reads elements of SamlStatus
    /// </summary>
    /// <param name="source">Xml Traverser</param>
    /// <param name="status">Status to populate</param>
    protected virtual void ReadElements(XmlTraverser source, SamlStatus status)
    {
        source.MoveNext();

        if(source.EnsureName(NamespaceUri, ElementNames.StatusCode))
        {
            status.StatusCode = ReadStatusCode(source);
        }
    }

    /// <summary>
    /// Factory for StatusCode
    /// </summary>
    /// <returns>StatusCode</returns>
    protected virtual StatusCode CreateStatusCode() => new();

    /// <summary>
    /// Reads a status code
    /// </summary>
    /// <param name="source"></param>
    protected virtual StatusCode ReadStatusCode(XmlTraverser source)
    {
        var result = CreateStatusCode();

        ReadAttributes(source, result);

        return result;
    }

    /// <summary>
    /// Reads attributes of StatusCode
    /// </summary>
    /// <param name="source"></param>
    /// <param name="statusCode"></param>
    protected virtual void ReadAttributes(XmlTraverser source, StatusCode statusCode)
    {
        statusCode.Value = source.GetRequiredAbsoluteUriAttribute(AttributeNames.Value)!;
    }
}
