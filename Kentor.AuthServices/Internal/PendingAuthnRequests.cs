using System;
using System.IO;
using System.IdentityModel.Metadata;
using System.IdentityModel.Services;
using System.IdentityModel.Tokens;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Security;
using Kentor.AuthServices.Configuration;
using System.Collections.Generic;

namespace Kentor.AuthServices.Internal
{
    static class PendingAuthnRequests
    {
      private const string CookieName = "AuthNRequest";
      private const string Purpose = "Initiator Verification";

      internal static void Add(Saml2Id id, StoredRequestState idp)
      {
        var cookie = GetNewCookie();
        cookie.Value = Encrypt(SerializeRequestState(id, idp));

        var cookies = HttpContext.Current.Response.Cookies; 
        cookies.Add(cookie);
      }

      internal static bool TryRemove(Saml2Id id, out StoredRequestState idp)
      {
        var returnValue = false;
        idp = null;

        var cookies = HttpContext.Current.Request.Cookies;
        var cookie = cookies[CookieName];

        if (cookie != null)
        {
          var decrypted = Decrypt(cookie.Value);

          if (decrypted != null && decrypted.Length > 0)
          {
            Saml2Id stateId;
            DeserializeRequestState(decrypted, out stateId, out idp);

            if (id.Value == stateId.Value)
            {
              returnValue = true;
            }
          }

          // Cleanup
          cookie = GetNewCookie();
          cookie.Expires = DateTime.Now.AddDays(-1d);
          cookies.Add(cookie);
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

      private static byte[] SerializeRequestState(Saml2Id id, StoredRequestState idp)
      {
        // Note: all data is put in a dictionary, because 'System.IdentityModel.Metadata.EntityId' within StoredRequestState is not serializable. Preferably StoredRequestState is modified to support serializing
        var data = new Dictionary<string, object> 
        { 
          {"Id", id.Value},
          {"StoredRequestState.Id", idp.Idp.Id},
          {"StoredRequestState.ReturnUrl", idp.ReturnUrl.ToString()},
          {"StoredRequestState.RelayData", idp.RelayData} // TODO: use relaystate parameter to IDP for this. Size of relaydata can cause cookie size to increase above 4000 bytes limit
        };

        return Serialize(data);
      }

      private static void DeserializeRequestState(byte[] serialized, out Saml2Id id, out StoredRequestState idp)
      {
        id = null;
        idp = null;

        var data = Deserialize(serialized) as Dictionary<string, object>;

        if (data != null)
        {
          id = new Saml2Id(data["Id"].ToString());
          idp = new StoredRequestState(new EntityId(data["StoredRequestState.Id"].ToString()),
                                       new Uri(data["StoredRequestState.ReturnUrl"].ToString()),
                                       data["StoredRequestState.RelayData"]);
        }
      }

      private static string Encrypt(byte[] serialized)
      {
        var protectedBytes = MachineKey.Protect(serialized, Purpose);
        return Convert.ToBase64String(protectedBytes);
      }

      private static byte[] Decrypt(string value)
      {
        byte[] returnValue = null;

        if (!String.IsNullOrEmpty(value))
        {
          try
          {
            var protectedBytes = Convert.FromBase64String(value);
            returnValue = MachineKey.Unprotect(protectedBytes, Purpose);
          }
          catch (Exception)
          {
            const string msg = "The response could not be validated. Possible causes: cookie has been tempered with or the machine keys on each farm member are not the same.";
            throw new InvalidOperationException(msg);
          }
        }

        return returnValue;
      }

      private static byte[] Serialize(object obj)
      {
        using (var stream = new MemoryStream())
        {
          var formatter = new BinaryFormatter();
          formatter.Serialize(stream, obj);
          stream.Flush();

          return stream.ToArray();
        }
      }

      private static object Deserialize(byte[] value)
      {
        object returnValue = null;

        if (value != null && value.Length > 0)
        {
          var formatter = new BinaryFormatter();

          using (var stream = new MemoryStream(value))
          {
            returnValue = formatter.Deserialize(stream);
          }
        }

        return returnValue;
      }
    }
}
