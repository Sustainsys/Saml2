namespace Sustainsys.Saml2.WebSso
{
    /// <summary>
    /// Interface for a factory that creates the command objects that handle the incoming http requests.
    /// </summary>
    public interface ICommandFactory
    {
        ICommand GetCommand(string commandName);
    }
}