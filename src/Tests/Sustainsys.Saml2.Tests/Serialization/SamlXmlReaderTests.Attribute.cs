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
    public void ReadAttribute_EmptyStringAttribute_Short()
    {
        // Arrange
        var source = GetXmlTraverser();
        var subject = new SamlXmlReader();
        var expected = new SamlAttribute
        {
            Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/roles",
            Values = {""}
        };

        // Act
        var actual = subject.ReadAttribute(source);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ReadAttribute_EmptyStringAttribute_Long()
    {
        // Arrange
        var source = GetXmlTraverser();
        var subject = new SamlXmlReader();
        var expected = new SamlAttribute
        {
            Name = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/roles",
            Values = {""}
        };

        // Act
        var actual = subject.ReadAttribute(source);

        // Assert
        actual.Should().BeEquivalentTo(expected);
    }
    
    [Fact]
    public void ReadAttribute_NullAttribute_True()
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
    
    [Fact]
    public void ReadAttribute_NullAttribute_One()
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
