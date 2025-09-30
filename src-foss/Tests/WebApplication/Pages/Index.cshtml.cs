// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    public async Task OnGet()
    {
        var authResult = await HttpContext.AuthenticateAsync();

        Items = authResult?.Properties?.Items;
    }

    public IDictionary<string, string?>? Items { get; set; }

    [BindProperty]
    public string? Action { get; set; }

    public async Task<IActionResult> OnPost()
    {
        switch (Action)
        {
            case "SignIn":
                {
                    AuthenticationProperties properties = new()
                    {
                        Items =
                        {
                            { "TestKey", "TestValue" }
                        }
                    };
                    return Challenge(properties);
                }
            case "SignOut":
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/");
            default:
                throw new NotImplementedException();

        }
    }
}