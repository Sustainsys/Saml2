using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kentor.AuthServices.TestHelpers
{
    public static class SpinWaiter
    {
        public readonly static TimeSpan MaxWait = new TimeSpan(0, 0, 0, 0, 500);
        public static void While(Func<bool> condition, string failMessage = "Timeout passed without condition becoming false.")
        {
            var waitStart = DateTime.UtcNow;
            while (condition())
            {
                if (DateTime.UtcNow - waitStart > MaxWait)
                {
                    Assert.Fail(failMessage);
                }
            }
        }

    }
}
