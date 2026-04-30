// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Bindings;

internal class ReadLimitedDeflateStream(
    Stream stream,
    CompressionMode compressionMode,
    int maxBytes)
    : DeflateStream(stream, compressionMode)
{
    private int totalRead;

    public override int Read(byte[] buffer, int offset, int count)
    {
        var read = base.Read(buffer, offset, count);

        totalRead += read;

        if (totalRead > maxBytes)
        {
            throw new InvalidOperationException($"Maximum allowed decompressed size {maxBytes} exceeded. Change BindingOptions to allow larger messages.");
        }

        return read;
    }
}