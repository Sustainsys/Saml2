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
        private static readonly Command notFoundCommand = new NotFoundCommand();

        public static Command GetCommand(string path)
        {
            switch (path)
            {
                default:
                    return notFoundCommand;
            }
        }
    }
}
