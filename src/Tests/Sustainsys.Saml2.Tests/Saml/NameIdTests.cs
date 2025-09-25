using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Tests.Saml;
public class NameIdTests
{
    private const string value1 = "https://idp1.example.com";
    private const string value2 = "https://idp2.example.com";
    private const string format1 = "urn:format1";
    private const string format2 = "urn:format2";
    [Test]
    public void ImplicitConversion()
    {
        NameId subject = value1;
        subject.Value.Should().Be(value1);
    }

    [Test]
    public void Equals_Object()
    {
        NameId subject = value1;
        var other = new object ();
        subject.Equals(other).Should().BeFalse();
    }

    [Test]
    [Arguments(value1, null, value1, null, true)]
    [Arguments(value1, null, value2, null, false)]
    [Arguments(value1, format1, value1, format1, true)]
    [Arguments(value1, format1, value1, format2, false)]
    [Arguments(value1, format1, value2, format1, false)]
    [Arguments(value1, format1, value2, format2, false)]
    public void Equals_NameId(string v1, string? f1, string v2, string? f2, bool expected)
    {
        NameId subject = new(v1, f1);
        NameId other = new(v2, f2);
        subject.Equals(other).Should().Be(expected);
    }

    [Test]
    public void Equals_NameId_Null()
    {
        NameId subject = value1;
        NameId? other = null;
        subject.Equals(other).Should().BeFalse();
    }

    [Test]
    [Arguments(value1, null, value1, null, true)]
    [Arguments(value1, null, value2, null, false)]
    [Arguments(value1, format1, value1, format1, true)]
    [Arguments(value1, format1, value1, format2, false)]
    [Arguments(value1, format1, value2, format1, false)]
    [Arguments(value1, format1, value2, format2, false)]
    public void Operator_Equal(string v1, string? f1, string v2, string? f2, bool expected)
    {
        NameId subject = new(v1, f1);
        NameId other = new(v2, f2);
        (subject == other).Should().Be(expected);
    }

    [Test]
    public void Operator_Equal_Null()
    {
        NameId? subject = null;
        NameId? other = null;
        (subject == other).Should().BeTrue();
    }

    [Test]
    public void Operator_Equal_Null_vs_NullProperties()
    {
        NameId? subject = new();
        NameId? other = null;
        (subject == other).Should().BeFalse();
    }

    [Test]
    [Arguments(value1, null, value1, true)]
    [Arguments(value1, null, "xyz", false)]
    [Arguments(value1, "urn:format", value1, false)]
    public void Equals_String(string nameIdValue, string? nameIdFormat, string str, bool expected)
    {
        NameId subject = new(nameIdValue, nameIdFormat);
        subject.Equals(str).Should().Be(expected);
    }

    [Test]
    [Arguments(value1, null, value1, true)]
    [Arguments(value1, null, "xyz", false)]
    [Arguments(value1, "urn:format", value1, false)]
    public void Operator_Equals_String(string nameIdValue, string? nameIdFormat, string str, bool expected)
    {
        NameId subject = new(nameIdValue, nameIdFormat);
        (subject == str).Should().Be(expected);
    }

    [Test]
    public void GetHashCode_NullFormat_ShouldNotThrow()
    {
        NameId? subject = new("xyz", null);
        // Should not throw
        subject.GetHashCode();
    }
}