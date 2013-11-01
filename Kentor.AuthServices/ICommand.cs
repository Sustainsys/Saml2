using System.Web;

namespace Kentor.AuthServices
{
    /// <summary>
    /// A command - corresponds to an action in Mvc.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Run the command and return a result.
        /// </summary>
        /// <param name="request">The current http request that the input
        /// data can be read from.</param>
        /// <returns>The results of the command, as a DTO.</returns>
        CommandResult Run(HttpRequestBase request);
    }
}
