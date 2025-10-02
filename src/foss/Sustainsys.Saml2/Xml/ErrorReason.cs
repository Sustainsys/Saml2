// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

namespace Sustainsys.Saml2.Xml;

/// <summary>
/// Error reasons in the error reporting.
/// </summary>
public enum ErrorReason
{
    /// <summary>
    /// The local of the node name was not the expected.
    /// </summary>
    UnexpectedLocalName = 1,

    /// <summary>
    /// The namesapce of the node was not the expected.
    /// </summary>
    UnexpectedNamespace = 2,

    /// <summary>
    /// A required attribute was missing.
    /// </summary>
    MissingAttribute = 3,

    /// <summary>
    /// Value conversion failed for the attribute. The string
    /// representation is stored as <see cref="Error.StringValue"/>.
    /// </summary>
    ConversionFailed = 4,

    /// <summary>
    /// A string value that should be an absolute Uri wasn't that.
    /// </summary>
    NotAbsoluteUri = 5,

    /// <summary>
    /// When traversing child elements, an unsupported node type was encountered.
    /// </summary>
    UnsupportedNodeType = 6,

    /// <summary>
    /// Tried to move to next child element, but there was none as it should be.
    /// </summary>
    MissingElement = 7,

    /// <summary>
    /// A signature failed validation.
    /// </summary>
    SignatureFailure = 8,

    /// <summary>
    /// There are extra elements that were neither processed nor ignored.
    /// </summary>
    ExtraElements = 9,

    /// <summary>
    /// The element is present, but contains nothing.
    /// </summary>
    EmptyElement = 10,
}