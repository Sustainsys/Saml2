using IdentityServer3.Core.Services.InMemory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SampleIdentityServer3
{
    static class Users
    {
        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>
            {
                new InMemoryUser
                {
                    Username = "JohnDoe",
                    Password = "password",
                    Subject = Guid.NewGuid().ToString()
                }
            };
        }
    }
}