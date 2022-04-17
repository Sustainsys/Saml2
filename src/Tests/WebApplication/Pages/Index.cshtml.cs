using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sustainsys.Saml2.AspNetCore;

namespace WebApplication.Pages;

[IgnoreAntiforgeryToken]
public class IndexModel : PageModel
{
    public void OnGet()
    {
    }

    [BindProperty]
    public string? Action { get; set; }

    public IActionResult OnPost()
    {
        switch(Action)
        {
            case "SignOut":
                return SignOut(CookieAuthenticationDefaults.AuthenticationScheme, Saml2Defaults.AuthenticationScheme);
            case "SignIn":
                return Challenge();
        }

        throw new NotImplementedException();
    }
}
