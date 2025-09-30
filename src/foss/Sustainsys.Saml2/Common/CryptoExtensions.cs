// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using System.Security.Cryptography;
using System.Text;

namespace Sustainsys.Saml2.Common;

/// <summary>
/// Extension convience methods for crypto
/// </summary>
public static class CryptoExtensions
{
    /// <summary>
    /// Hash input using Sha256
    /// </summary>
    /// <param name="input">Input data</param>
    /// <param name="truncateLength">
    /// Requested length of result. The result istruncated to this length. Note!
    /// the hash is no longer cryptographically secure when truncated.
    /// </param>
    /// <returns>Base64UrlEncode(Sha256(input))</returns>
    public static string Sha256(this string? input, int? truncateLength = null)
    {
        if (string.IsNullOrEmpty(input))
        {
            return "";
        }

        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

        var result = Base64UrlTextEncoder.Encode(hash);

        if (truncateLength.HasValue)
        {
            return result.Substring(0, truncateLength.Value);
        }

        return result;
    }
}