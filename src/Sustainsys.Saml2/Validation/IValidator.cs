namespace Sustainsys.Saml2.Validation;

/// <summary>
/// Validation exception
/// </summary>
/// <typeparam name="T">Type of data that is validated</typeparam>
/// <param name="message">Error message</param>
public class ValidationException<T>(string message) : Exception(message)
{ }

/// <summary>
/// Common interface for validators
/// </summary>
/// <typeparam name="TData">Type of data to validate</typeparam>
/// <typeparam name="TValidatorParams">Type of validation parameters</typeparam>
public interface IValidator<TData, TValidatorParams>
{
    /// <summary>
    /// Validate
    /// </summary>
    /// <param name="data"></param>
    /// <param name="validatorParams"></param>
    /// <exception cref="ValidationException{T}">Exception on failed validation</exception>
    void Validate(TData data, TValidatorParams validatorParams);
}

/// <summary>
/// Extension method helper
/// </summary>
public static class ValidationExtensions
{
    /// <summary>
    /// Performs validation and returns <see cref="Validated{TData}"/>
    /// </summary>
    /// <typeparam name="TData">Type of the validated data</typeparam>
    /// <typeparam name="TValidationParams">Validation parameters</typeparam>
    /// <param name="data">The data</param>
    /// <param name="validationParams">Validation params to use</param>
    /// <param name="validator">Validators to run</param>
    /// <returns>Validated wrapper around data</returns>
    /// <exception cref="ValidationException{T}">if validation fails</exception> 
    public static Validated<TData> Validate<TData, TValidationParams>(
        this TData data,
        IValidator<TData, TValidationParams> validator,
        TValidationParams validationParams)
        => new Validated<TData, TValidationParams>(data, validationParams, validator);
}

/// <summary>
/// Wrapper around a value, indicating that it is valid.
/// </summary>
/// <remarks>
/// The purpose of this class is to let security sensitive methods
/// mark that they require the passed argument to be validated before being used.
/// </remarks>
/// <typeparam name="TData">The type of the data</typeparam>
public class Validated<TData>
{
    /// <summary>
    /// Constructor. Caller must ensure that the data is validated as
    /// part of the construction.
    /// </summary>
    /// <param name="data">The data to wrap.</param>
    protected Validated(TData data) => Value = data;

    /// <summary>
    /// The validated value
    /// </summary>
    public TData Value { get; private set; }

    /// <summary>
    /// Implicit operator returning the validated value.
    /// </summary>
    /// <param name="valid">The Validated wrapper of the value</param>
    public static implicit operator TData(Validated<TData> valid)
        => valid.Value;

    /// <summary>
    /// Gets a property of the validated data, wrapped in another Validated instance.
    /// </summary>
    /// <typeparam name="TProperty">Type of the property</typeparam>
    /// <param name="selector">Property selector function</param>
    /// <returns>Validated property</returns>
    public Validated<TProperty> GetValidated<TProperty>(Func<TData, TProperty> selector) =>
        new(selector(Value));
}

/// <summary>
/// Derived helper for <see cref="Validated{TData}"/> that handles
/// the validation parameters type.
/// </summary>
/// <typeparam name="TData"></typeparam>
/// <typeparam name="TValidatorParams"></typeparam>
public class Validated<TData, TValidatorParams> : Validated<TData>
{
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="data"></param>
    /// <param name="validatorParams"></param>
    /// <param name="validator"></param>
    /// <exception cref="ArgumentException"></exception>
    public Validated(
        TData data,
        TValidatorParams validatorParams,
        IValidator<TData, TValidatorParams> validator)
        : base(data)
    {
        validator.Validate(data, validatorParams);
    }
}