using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

partial class SamlXmlReader

{
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

		if (source.EnsureName(Constants.Namespaces.SamlpUri, Constants.Elements.StatusCode))
		{
			status.StatusCode = ReadStatusCode(source);
			source.MoveNext(true);
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
		statusCode.Value = source.GetRequiredAbsoluteUriAttribute(Attributes.Value)!;
	}

}
