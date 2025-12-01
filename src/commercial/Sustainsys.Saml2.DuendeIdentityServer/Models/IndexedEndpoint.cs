// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Sustainsys.Saml2.DuendeIdentityServer.Models;

/// <summary>
/// An indexed endpoint.
/// </summary>
public record struct IndexedEndpoint
{
    private const char separator = '|';
    private const char signature = '§';

    /// <summary>
    /// Deserializes a RedirectUri representation of the IndexedEndpoint
    /// </summary>
    /// <param name="serializedIndexedEndpoint">serialized format</param>

    public static IndexedEndpoint FromSerializedString(string serializedIndexedEndpoint)
    {
        var segments = serializedIndexedEndpoint.Split(separator);

        if (segments.Length != 5 || segments[0].Length != 1 || segments[0][0] != signature)
        {
            throw new InvalidOperationException("String is not a serialized IndexedEndpoint in § format");
        }

        return new()
        {
            Index = int.Parse(segments[1]),
            IsDefault = bool.Parse(segments[2]),
            Binding = segments[3],
            Location = segments[4]
        };
    }

    /// <summary>
    /// Creates an IndexedEndpoint with the given string as location and the HttpPOST binding.
    /// </summary>
    /// <param name="location">Location</param>
    public static implicit operator IndexedEndpoint(string location) =>
        new()
        {
            Binding = Constants.BindingUris.HttpPOST,
            Location = location
        };

    /// <summary>
    /// Uri identifier of binding used.
    /// </summary>
    public required string Binding { get; init; }

    /// <summary>
    /// URL specifying the location of the endpoint.
    /// </summary>
    public required string Location { get; init; }

    /// <summary>
    /// Index of the endpoint.
    /// </summary>
    public int Index { get; init; }

    /// <summary>
    /// Indicates this is the default endpoint.
    /// </summary>
    public bool IsDefault { get; init; }

    /// <summary>
    /// Returns the serialized string format.
    /// </summary>
    /// <returns>String</returns>
    public string Serialize()
    {
        return $"{signature}{separator}{Index}{separator}{IsDefault}{separator}{Binding}{separator}{Location}";
    }
}
