using Sustainsys.Saml2.WebSso;
using System;
using System.Net;
using System.Web;

namespace Sustainsys.Saml2.HttpModule
{
    public static partial class CommandResultHttpExtensions
    {
        /// <summary>
        /// Apply the command result to a bare HttpResponse.
        /// </summary>
        /// <param name="commandResult">The CommandResult that will update the HttpResponse.</param>
        /// <param name="response">Http Response to write the result to.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "HttpStatusCode")]
        public static void Apply(this CommandResult commandResult, HttpResponseBase response)
        {
            if (commandResult == null)
            {
                throw new ArgumentNullException(nameof(commandResult));
            }

            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            response.Cache.SetCacheability((HttpCacheability)commandResult.Cacheability);

            ApplyCookies(commandResult, response);
            ApplyHeaders(commandResult, response);

            if (commandResult.HttpStatusCode == HttpStatusCode.SeeOther || commandResult.Location != null)
            {
                if (commandResult.Location == null)
                {
                    throw new InvalidOperationException("Missing Location on redirect.");
                }
                if (commandResult.HttpStatusCode != HttpStatusCode.SeeOther)
                {
                    throw new InvalidOperationException("Invalid HttpStatusCode for redirect, but Location is specified");
                }

                response.Redirect(commandResult.Location.OriginalString);
            }
            else
            {
                response.StatusCode = (int)commandResult.HttpStatusCode;
                response.ContentType = commandResult.ContentType;
                response.Write(commandResult.Content);

                response.End();
            }
        }
    }
}
