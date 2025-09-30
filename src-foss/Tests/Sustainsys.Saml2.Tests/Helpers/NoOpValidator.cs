using Sustainsys.Saml2.Validation;

namespace Sustainsys.Saml2.Tests.Helpers;

/// <summary>
/// A no operations (i.e. dummy) validator. Used to make test data available as
/// <see cref="Valid{TData}"/>
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TValidatorParams"></typeparam>
class NoOpValidator<TData> : IValidator<TData, object>
{
    public void Validate(TData data, object _) { }
}


public static class NoOpValidatorExtensions
{
    public static Valid<TData> FakeValidate<TData>(this TData data)
    {
        var validator = new NoOpValidator<TData>();

        // The parameters is ignored, we just need an object to pass. Data will do.
        return data.Validate(validator, data!);
    }
}