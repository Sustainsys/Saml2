using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices
{
    /// <summary>
    /// Factory to create the command objects thand handles the incoming http requests.
    /// </summary>
    public static class CommandFactory
    {
        private static readonly ICommand notFoundCommand = new NotFoundCommand();

        private static readonly IDictionary<string, ICommand> commands =
        new Dictionary<string, ICommand>(StringComparer.InvariantCultureIgnoreCase) 
        { 
            { "SignIn", new SignInCommand() },
            { "Acs", new AcsCommand() }
        };

        /// <summary>
        /// Gets a command for a command name.
        /// </summary>
        /// <param name="commandName">Name of a command. Probably a path.</param>
        /// <returns>A command implementation or notFoundCommand if invalid.</returns>
        public static ICommand GetCommand(string commandName)
        {
            ICommand command;
            if (commands.TryGetValue(commandName, out command))
            {
                return command;
            }

            return notFoundCommand;
        }
    }
}
