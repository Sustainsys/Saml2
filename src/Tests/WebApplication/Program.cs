using Microsoft.AspNetCore.Authentication.Cookies;
using Sustainsys.Saml2.AspNetCore;

var builder = Microsoft.AspNetCore.Builder.WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(opt =>
    {
        opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        opt.DefaultChallengeScheme = Saml2Defaults.AuthenticationScheme;
    })
    .AddCookie()
    .AddSaml2();

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseRouting();

app.MapRazorPages();

app.Run();
