using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetCore2.Tests;

class StubResponseCookies : IResponseCookies
{
    public void Append(string key, string value) => throw new NotImplementedException();
    public void Append(string key, string value, CookieOptions options) { }
    public void Delete(string key) => throw new NotImplementedException();
    public void Delete(string key, CookieOptions options) => throw new NotImplementedException();
}
