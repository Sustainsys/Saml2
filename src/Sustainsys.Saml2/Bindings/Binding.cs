namespace Sustainsys.Saml2.Bindings;

/// <summary>
/// A SAML2 Binding that operates on the front channel, i.e. browser.
/// </summary>
public abstract class Binding
{
    /// <summary>
    /// The Uri identifying the binding
    /// </summary>
    public abstract string Identification { get; }

    /// <summary>
    /// Binds a Saml2 message to the http response.
    /// </summary>
    /// <param name="message">Saml2 message</param>
    /// <returns>Task</returns>
    /// <exception cref="System.ArgumentException">If message properties not properly set</exception>
    public BoundMessage Bind(Saml2Message message)
    {
        if (string.IsNullOrWhiteSpace(message.Name))
        {
            throw new ArgumentException("Name property must have value", nameof(message));
        }

        if(message.Xml == null)
        {
            throw new ArgumentException("Xml property must have value", nameof(message));
        }

        return DoBind(message);
    }

    /// <summary>
    /// Binds a Saml2 message to the http response.
    /// </summary>
    /// <param name="message">Saml2 message</param>
    /// <returns>Task</returns>
    protected abstract BoundMessage DoBind(Saml2Message message);
}
