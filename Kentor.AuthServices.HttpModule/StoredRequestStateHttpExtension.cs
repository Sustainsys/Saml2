using System;
using System.IO;
using System.IdentityModel.Services;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using System.Web.Security;
using Kentor.AuthServices.Configuration;

namespace Kentor.AuthServices.HttpModule
{
    /// <summary>
    /// Extension methods to StoredRequestState to get it from the HttpRequestBase or set it to the HttpResponseBase.
    /// </summary>
    public static class StoredRequestStateHttpExtension
    {
        private const string CookieName = "AuthNRequest";
        private const string Purpose = "Request State";

        /// <summary>
        /// Transform the stored request state to a http cookie to be set to the HttpResponse.
        /// </summary>
        /// <param name="response">Http Response to write the result to.</param>
        /// <param name="storedRequestState">The stored request state to be saved in a HTTP cookie.</param>
        public static void SetStoredRequestState(this HttpResponseBase response, StoredRequestState storedRequestState)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            var cookieHandler = CookieHandler;

            if (storedRequestState != null)
            {
                byte[] value = Encrypt(Serialize(storedRequestState));
                cookieHandler.Write(value, false, DateTime.MinValue);
            }
            else
            {
                cookieHandler.Delete();
            }
        }

        /// <summary>
        /// Transform the received HTTP cookie to a stored request state instance.
        /// </summary>
        /// <param name="requestBase">Http Request to get the request state from.</param>
        public static StoredRequestState GetStoredRequestState(this HttpRequestBase requestBase)
        {
            if (requestBase == null)
            {
                throw new ArgumentNullException("requestBase");
            }

            return Deserialize(Decrypt(CookieHandler.Read()));
        }

        private static ChunkedCookieHandler CookieHandler
        {
            get
            {
                // Use native federation configuration for determening cookie settings
                CookieHandler config = FederatedAuthentication.SessionAuthenticationModule.CookieHandler;

                var appRoot = HttpContext.Current.Request.ApplicationPath;
                appRoot = (!String.IsNullOrEmpty(appRoot)) ? appRoot.TrimEnd('/') : String.Empty;
                var authServicesRoot = appRoot + KentorAuthServicesSection.Current.ModulePath + "/";

                return new ChunkedCookieHandler
                {
                    Name = CookieName,
                    Domain = config.Domain,
                    Path = authServicesRoot,
                    HideFromClientScript = config.HideFromClientScript,
                    RequireSsl = config.RequireSsl
                };
            }
        }

        private static byte[] Encrypt(byte[] serialized)
        {
            return MachineKey.Protect(serialized, Purpose);
        }

        private static byte[] Serialize(StoredRequestState obj)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Flush();

                return stream.ToArray();
            }
        }
        
        private static byte[] Decrypt(byte[] value)
        {
            byte[] returnValue = null;

            if (value != null && value.Length > 0)
            {
                try
                {
                    returnValue = MachineKey.Unprotect(value, Purpose);
                }
                catch (Exception)
                {
                    const string msg = "The response could not be validated. Possible causes: cookie has been tempered with or the machine keys on each farm member are not the same.";
                    throw new InvalidOperationException(msg);
                }
            }

            return returnValue;
        }

        private static StoredRequestState Deserialize(byte[] value)
        {
            StoredRequestState returnValue = null;

            if (value != null && value.Length > 0)
            {
                var formatter = new BinaryFormatter();

                using (var stream = new MemoryStream(value))
                {
                    returnValue = formatter.Deserialize(stream) as StoredRequestState;
                }
            }

            return returnValue;
        }
    }
}
