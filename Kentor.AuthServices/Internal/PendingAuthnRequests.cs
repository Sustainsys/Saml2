using System;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Text;
using System.Web;
using System.Web.Security;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.Internal
{
    static class PendingAuthnRequests
    {
      private const string CookieName = "AuthNRequest";
      private const string CookieIdAttribute = "id";
      private const string CookieIdpIdAttribute = "idpId";
      private const string Purpose = "Initiator Verification";

      internal static void Add(Saml2Id id, StoredRequestState idp)
      {
        var cookie = GetNewCookie();
        cookie.Values[CookieIdAttribute] = Encrypt(id.Value);
        cookie.Values[CookieIdpIdAttribute] = Encrypt(idp.Idp.Id);

        HttpContext.Current.Response.Cookies.Add(cookie);
      }

      internal static bool TryRemove(Saml2Id id, out StoredRequestState idp)
      {
        var returnValue = false;
        idp = null;

        var cookie = HttpContext.Current.Request.Cookies[CookieName];

        if (cookie != null)
        {
          var cookieId = Decrypt(cookie.Values[CookieIdAttribute]);
          var cookieIdpId = Decrypt(cookie.Values[CookieIdpIdAttribute]);

          if (id != null && cookieId == id.Value)
          {
            idp = new StoredRequestState(new EntityId(cookieIdpId), null); // TODO: returnUrl is left out here as it's not used. Possibly add it by also storing it into the cookie (or get it from config based in the id) or simply remove if from the stored state
            returnValue = true;
          }

          // Cleanup
          cookie = GetNewCookie();
          cookie.Expires = DateTime.Now.AddDays(-1d);
          HttpContext.Current.Response.Cookies.Add(cookie);
        }

        return returnValue;
      }

      private static HttpCookie GetNewCookie()
      {
        return new HttpCookie(CookieName)
        {
          Path = String.Concat(HttpContext.Current.Request.ApplicationPath, KentorAuthServicesSection.Current.ModulePath),
          HttpOnly = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.HideFromClientScript,
          Secure = FederatedAuthentication.SessionAuthenticationModule.CookieHandler.RequireSsl
        };
      }

      private static string Encrypt(string value)
      {
        var unprotectedBytes = Encoding.UTF8.GetBytes(value);
        var protectedBytes = MachineKey.Protect(unprotectedBytes, Purpose);

        return Convert.ToBase64String(protectedBytes);
      }

      private static string Decrypt(string value)
      {
        var returnValue = String.Empty;

        if (!String.IsNullOrEmpty(value))
        {
          try
          {
            var protectedBytes = Convert.FromBase64String(value);
            var unprotectedBytes = MachineKey.Unprotect(protectedBytes, Purpose);
            returnValue = unprotectedBytes != null ? Encoding.UTF8.GetString(unprotectedBytes) : String.Empty;
          }
          catch (Exception)
          {
            // TODO: logging / exception. Probable causes: tempering with cookie, or machine keys are not equal on each farm member
          }
        }

        return returnValue;
      }
    }
}
