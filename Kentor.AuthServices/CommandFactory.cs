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
    static class CommandFactory
    {
        private static readonly ICommand notFoundCommand = new NotFoundCommand();

        private static readonly IDictionary<string, ICommand> commands =
        new Dictionary<string, ICommand>() 
        { 
            { "SignIn", new SignInCommand() },
            { "Acs", new AcsCommand() }
        };

        public static ICommand GetCommand(string path)
        {
            ICommand command;
            if (commands.TryGetValue(path, out command))
            {
                return command;
            }

            return notFoundCommand;
        }
    }
}
