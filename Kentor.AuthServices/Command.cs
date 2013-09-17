using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kentor.AuthServices
{
    abstract class Command
    {
        public abstract CommandResult Run();
    }
}
