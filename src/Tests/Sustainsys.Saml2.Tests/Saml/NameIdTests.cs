using Sustainsys.Saml2.Saml;

namespace Sustainsys.Saml2.Tests.Saml;
public class NameIdTests
{
    private const string Value = "https://idp.example.com";

    [Fact]
    public void ImplicitConversion()
    {
        NameId subject = Value;

        subject.Value.Should().Be(Value); ;
    }

    [Fact]
    public void Equals_Object()
    {
        NameId subject = Value;

        subject.Equals(new object()).Should().BeFalse();
    }

    [Fact]
    public void Equals_NameId_Equal()
    {
        NameId subject = Value;
        NameId other = Value;

        subject.Equals(other).Should().BeTrue();
    }

    [Fact]
    public void Equals_NameId_Operator_Equal()
    {
        NameId subject = Value;
        NameId other = Value;

        (subject == other).Should().BeTrue();
    }

    [Fact]
    public void Equals_NameId_NotEqual()
    {
        NameId subject = Value;
        NameId other = "https://other.example.com";

        subject.Equals(other).Should().BeFalse();
    }

    [Fact]
    public void Equals_NameId_Operator_NotEqual()
    {
        NameId subject = Value;
        NameId other = "https://other.example.com";

        (subject == other).Should().BeFalse();
    }

    [Fact]
    public void Equals_String_Equal()
    {
        NameId subject = Value;

        subject.Equals(Value).Should().BeTrue();
    }

    [Fact]
    public void Equals_String_Operator_Equal()
    {
        NameId subject = Value;

        (subject == Value).Should().BeTrue();
    }

    [Fact]
    public void Equals_String_NotEqual()
    {
        NameId subject = Value;

        subject.Equals("xyz").Should().BeFalse();
    }

    [Fact]
    public void Equals_String_Operator_NotEqual()
    {
        NameId subject = Value;

        (subject == "xyz").Should().BeFalse();
    }

}