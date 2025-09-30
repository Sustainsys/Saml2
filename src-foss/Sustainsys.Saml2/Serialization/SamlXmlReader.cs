// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Xml;
using System.Collections.ObjectModel;
using System.Security.Cryptography.Xml;
using static Sustainsys.Saml2.Constants;

namespace Sustainsys.Saml2.Serialization;

/// <summary>
/// Reader for data from an Xml Document.
/// </summary>
public partial class SamlXmlReader : ISamlXmlReader
{
    /// <inheritdoc/>
    public virtual IEnumerable<string>? AllowedHashAlgorithms { get; set; } =
        defaultAllowedHashAlgorithms;

    private static readonly IEnumerable<string> defaultAllowedHashAlgorithms =
        new ReadOnlyCollection<string>(
        [
            "sha256",
            "sha384",
            "sha512",
            SignedXml.XmlDsigRSASHA256Url
        ]);

    /// <inheritdoc/>
    public virtual IEnumerable<SigningKey>? TrustedSigningKeys { get; set; }

    /// <inheritdoc/>
    public virtual Func<string, Saml2Entity>? EntityResolver { get; set; }

    /// <summary>
    /// Helper method that calls ThrowOnErrors. To supress errors and prevent 
    /// throwing, this is the last chance method to override.
    /// </summary>
    protected virtual void ThrowOnErrors(XmlTraverser source)
        => source.ThrowOnErrors();

    /// <summary>
    /// Default factory for read types is just to new it up. Override this method
    /// to create a derived/specialized type instead.
    /// </summary>
    /// <typeparam name="T">Type to create</typeparam>
    /// <returns>New instance of <typeparamref name="T"/></returns>
    protected virtual T Create<T>() where T : new() => new();

    /// <summary>
    /// Helper method to get the signing keys and allowed signature algorithms for
    /// an issuer.
    /// </summary>
    /// <param name="source">Xml Travers source - used to report errors</param>
    /// <param name="issuer">The issuer to find parameters for</param>
    /// <returns>
    /// Trusted signig keys and allowedHashAlgorithms. Hash algorithms uses the <see cref="AllowedHashAlgorithms"/>
    /// if there is no specific configuration on the Issuer.
    /// </returns>
    protected (IEnumerable<SigningKey>? trustedSigningKeys, IEnumerable<string>? allowedHashAlgorithms)
    GetSignatureValidationParametersFromIssuer(XmlTraverser source, NameId? issuer)
    {
        var trustedSigningKeys = TrustedSigningKeys;
        var allowedHashAlgorithms = AllowedHashAlgorithms;
        if (source.HasName(Elements.Signature, SignedXml.XmlDsigNamespaceUrl))
        {
            if (issuer == null)
            {
                source.Errors.Add(new(ErrorReason.MissingElement, Elements.Issuer, source.CurrentNode,
                    "A signature was found, but there was no Issuer specified. See profile spec 4.1.4.1, 4.1.4.2, 4.4.4.2"));
            }
            else
            {
                if (EntityResolver != null)
                {
                    var entity = EntityResolver(issuer.Value);
                    trustedSigningKeys = entity.SigningKeys;
                    allowedHashAlgorithms = entity.AllowedHashAlgorithms ?? AllowedHashAlgorithms;
                }
            }
        }

        return (trustedSigningKeys, allowedHashAlgorithms);
    }

    private static void CallErrorInspector<TData>(
        Action<ReadErrorInspectorContext<TData>>? errorInspector,
        TData data,
        XmlTraverser source)
    {
        if (errorInspector != null && source.Errors.Count != 0)
        {
            var context = new ReadErrorInspectorContext<TData>()
            {
                Data = data,
                Errors = source.Errors,
                XmlSource = source.RootNode
            };

            errorInspector(context);
        }
    }
}