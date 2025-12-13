// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace OidcClient.Pages;

[Authorize]
public class SecureModel : PageModel
{
    public IEnumerable<Claim> Claims { get; private set; } = default!;

    public IEnumerable<KeyValuePair<string, string>> Properties { get; private set; } = default!;

    public async Task OnGet()
    {
        var authResult = await HttpContext.AuthenticateAsync();

        Claims = authResult.Principal!.Claims;
        Properties = authResult.Properties!.Items!;
    }
}