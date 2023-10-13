using Microsoft.AspNetCore.Http;

namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// A SAML2 Binding that operates on the front channel, i.e. browser.
/// </summary>
public interface IFrontChannelBinding
{
    /// <summary>
    /// Binds a Saml2 message to the http response.
    /// </summary>
    /// <param name="message">Saml2 message</param>
    /// <param name="httpResponse">Http response to bind message to</param>
    /// <returns>Task</returns>
    /// <exception cref="System.ArgumentException">If message properties not properly set</exception>
    Task BindAsync(HttpResponse httpResponse, Saml2Message message);

    /// <summary>
    /// Unbinds a Saml2 message from an http request.
    /// </summary>
    /// <param name="httpRequest">HttpRequest to unbind from</param>
    /// <param name="getSaml2Entity">Func that returns a Saml2 entity from an entity id</param>
    /// <returns></returns>
    Task<Saml2Message> UnbindAsync(HttpRequest httpRequest,
        Func<string, Task<Saml2Entity>> getSaml2Entity);

    /// <summary>
    /// Can the current binding unbind a Saml message from the request?
    /// </summary>
    /// <param name="httpRequest"></param>
    /// <returns></returns>
    bool CanUnbind(HttpRequest httpRequest);
}

/// <summary>
/// A SAML2 Binding that operates on the front channel, i.e. browser.
/// </summary>
public abstract class FrontChannelBinding : IFrontChannelBinding
{
    /// <inheritdoc/>
    public abstract bool CanUnbind(HttpRequest httpRequest);

    /// <inheritdoc/>
    public Task BindAsync(HttpResponse httpResponse, Saml2Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Name))
        {
            throw new ArgumentException("Name property must have value", nameof(message));
        }

        if (message.Xml == null)
        {
            throw new ArgumentException("Xml property must have value", nameof(message));
        }

        return DoBindAsync(httpResponse, message);
    }

    /// <summary>
    /// Binds a Saml2 message to the http response.
    /// </summary>
    /// <param name="message">Saml2 message</param>
    /// <param name="httpResponse">Http response to bind message to</param>
    /// <returns>Task</returns>
    protected abstract Task DoBindAsync(HttpResponse httpResponse, Saml2Message message);

    /// <inheritdoc />
    public abstract Task<Saml2Message> UnbindAsync(
        HttpRequest httpRequest,
        Func<string, Task<Saml2Entity>> getSaml2Entity);
}
