using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Serialization;

namespace Sustainsys.Saml2.Tests.Serialization;
public partial class SamlXmlReaderTests
{
    [Fact]
    public void ReadAttribute_Mandatory()
    {
        // Arrange
        var source = GetXmlTraverser();
        var subject = new SamlXmlReader();
        var expected = new SamlAttribute
        {
            Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/roles",
            Values = { "developer", "administrator" }
        };

        // Act
        var actual = subject.ReadAttribute(source);
        
        // Assert
        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void ReadAttribute_NullAttribute()
    {
        // Arrange
        var source = GetXmlTraverser();
        var subject = new SamlXmlReader();
        var expected = new SamlAttribute
        {
            Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/roles",
            Values = { }
        };

        // Act
        var actual = subject.ReadAttribute(source);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
}
