using System;

namespace Kentor.AuthServices
{
    internal static class UrlExtensions
    {
        public static Uri AppendReturnUrl(this Uri url, string returnUrl)
        {
            if (url == null || string.IsNullOrWhiteSpace(returnUrl))
                return url;
            Uri val;
            if (!Uri.TryCreate(Uri.UnescapeDataString(returnUrl), UriKind.RelativeOrAbsolute, out val))
                return url;

            var queryToAppend = "returnUrl=" + val;
            var baseUri = new UriBuilder(url);
            if (baseUri.Query.Length > 1)
                baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
            else
                baseUri.Query = queryToAppend;

            return baseUri.Uri;
        }
    }
}