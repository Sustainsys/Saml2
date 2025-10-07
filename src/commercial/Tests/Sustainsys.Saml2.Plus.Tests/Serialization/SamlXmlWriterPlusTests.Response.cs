// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using FluentAssertions;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;

namespace Sustainsys.Saml2.Plus.Tests.Serialization;
partial class SamlXmlWriterPlusTests
{
    [Fact]
    public void WriteSamlResponse_MinimalErrorRequester()
    {
        Response response = new()
        {
            Id = "x123",
            IssueInstant = new(2025, 10, 07, 11, 13, 32),
            Status = new()
            {
                StatusCode = new()
                {
                    Value = Constants.StatusCodes.Requester
                }
            }
        };

        var subject = new SamlXmlWriterPlus();

        var actual = subject.Write(response);
        var expected = GetXmlDocument();

        actual.Should().BeEquivalentTo(expected);
    }
};