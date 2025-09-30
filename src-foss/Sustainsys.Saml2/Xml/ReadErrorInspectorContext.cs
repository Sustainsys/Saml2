// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System.Xml;

namespace Sustainsys.Saml2.Xml;

/// <summary>
/// Context for an error inspector.
/// </summary>
/// <typeparam name="TData">Type of the data read</typeparam>
public class ReadErrorInspectorContext<TData>
{
    /// <summary>
    /// The data read
    /// </summary>
    public required TData Data { get; set; }

    /// <summary>
    /// The XML source, if this was a parsing event.
    /// </summary>
    public required XmlNode? XmlSource { get; set; }

    /// <summary>
    /// The errors found
    /// </summary>
    public required IList<Error> Errors { get; set; }
}