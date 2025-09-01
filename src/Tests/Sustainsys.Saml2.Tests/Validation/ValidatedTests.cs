using Sustainsys.Saml2.Tests.Helpers;
using Sustainsys.Saml2.Validation;

namespace Sustainsys.Saml2.Tests.Validation;
public class ValidatedTests
{
    private class MyClass
    {
        public object Property { get; set; }
    }

    private static Validated<MyClass> CreateSubject(out MyClass data)
    {
        data = new()
        {
            Property = new()
        };
        object pars = new();
        NoOpValidator<MyClass> validator = new();

        return data.Validate(validator, pars);
    }

    [Fact]
    public void Validate()
    {
        var validated = CreateSubject(out var data);

        validated.Value.Should().BeSameAs(data);
        MyClass implicitConvert = validated;
        implicitConvert.Should().BeSameAs(data);
    }


    [Fact]
    public void GetValidated()
    {
        var validated = CreateSubject(out MyClass data);

        var validatedProperty = validated.GetValidated(c => c.Property);

        validatedProperty.Value.Should().BeSameAs(data.Property);
    }
}