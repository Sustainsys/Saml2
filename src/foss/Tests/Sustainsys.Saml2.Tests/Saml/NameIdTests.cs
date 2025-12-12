// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Tests.Saml;

public class NameIdTests
{
    private const string value1 = "https://idp1.example.com";
    private const string value2 = "https://idp2.example.com";
    private const string format1 = "urn:format1";
    private const string format2 = "urn:format2";

    [Fact]
    public void ImplicitConversion()
    {
        NameId subject = value1;

        subject.Value.Should().Be(value1);
    }

    [Fact]
    public void Equals_Object()
    {
        NameId subject = value1;
        var other = new object();

        subject.Equals(other).Should().BeFalse();
    }

    [Theory]
    [InlineData(value1, null, value1, null, true)]
    [InlineData(value1, null, value2, null, false)]
    [InlineData(value1, format1, value1, format1, true)]
    [InlineData(value1, format1, value1, format2, false)]
    [InlineData(value1, format1, value2, format1, false)]
    [InlineData(value1, format1, value2, format2, false)]
    public void Equals_NameId(string v1, string? f1, string v2, string? f2, bool expected)
    {
        NameId subject = new(v1, f1);
        NameId other = new(v2, f2);

        subject.Equals(other).Should().Be(expected);
    }

    [Fact]
    public void Equals_NameId_Null()
    {
        NameId subject = value1;
        NameId? other = null;

        subject.Equals(other).Should().BeFalse();
    }

    [Theory]
    [InlineData(value1, null, value1, null, true)]
    [InlineData(value1, null, value2, null, false)]
    [InlineData(value1, format1, value1, format1, true)]
    [InlineData(value1, format1, value1, format2, false)]
    [InlineData(value1, format1, value2, format1, false)]
    [InlineData(value1, format1, value2, format2, false)]
    public void Operator_Equal(
        string v1, string? f1, string v2, string? f2, bool expected)
    {
        NameId subject = new(v1, f1);
        NameId other = new(v2, f2);

        (subject == other).Should().Be(expected);
    }

    [Fact]
    public void Operator_Equal_Null()
    {
        NameId? subject = null;
        NameId? other = null;

        (subject == other).Should().BeTrue();
    }

    [Fact]
    public void Operator_Equal_Null_vs_NullProperties()
    {
        NameId? subject = new();
        NameId? other = null;

        (subject == other).Should().BeFalse();
    }

    [Theory]
    [InlineData(value1, null, value1, true)]
    [InlineData(value1, null, "xyz", false)]
    [InlineData(value1, "urn:format", value1, false)]
    public void Equals_String(
        string nameIdValue, string? nameIdFormat, string str, bool expected)
    {
        NameId subject = new(nameIdValue, nameIdFormat);

        subject.Equals(str).Should().Be(expected);
    }

    [Theory]
    [InlineData(value1, null, value1, true)]
    [InlineData(value1, null, "xyz", false)]
    [InlineData(value1, "urn:format", value1, false)]
    public void Operator_Equals_String(string nameIdValue, string? nameIdFormat, string str, bool expected)
    {
        NameId subject = new(nameIdValue, nameIdFormat);

        (subject == str).Should().Be(expected);
    }

    [Fact]
    public void GetHashCode_NullFormat_ShouldNotThrow()
    {
        NameId? subject = new("xyz", null);

        // Should not throw
        subject.GetHashCode();
    }
}