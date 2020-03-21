using Microsoft.Owin;
using Microsoft.Owin.Infrastructure;
using System;
using System.Globalization;
using System.Linq;

namespace Sustainsys.Saml2.Owin
{
    internal class LimitingCookieManager : ICookieManager
    {
        private readonly int maxCookies;

        public LimitingCookieManager( int maxCookies = 10 )
        {
            this.maxCookies = maxCookies;
        }

        public void AppendResponseCookie( IOwinContext context, string key, string value, CookieOptions options )
        {
            if ( context == null ) throw new ArgumentNullException( nameof( context ) );

            var excessCookies = context.Request.Cookies
                .Where( c => c.Key.StartsWith( StoredRequestState.CookieNameBase, StringComparison.OrdinalIgnoreCase ) )
                .OrderByDescending( c => c.Value ).Skip( maxCookies - 1 ).ToList();

            excessCookies.ForEach( c => context.Response.Cookies.Delete( c.Key, options ) );

            var elapsed = Convert.ToInt64( ( DateTime.UtcNow - new DateTime( 1970, 1, 1 ) ).TotalMilliseconds );
            context.Response.Cookies.Append( key, elapsed.ToString( CultureInfo.InvariantCulture ) + Separator + value, options );
        }

        public void DeleteCookie( IOwinContext context, string key, CookieOptions options )
        {
            if ( context == null ) throw new ArgumentNullException( nameof( context ) );
            context.Response.Cookies.Delete( key, options );
        }

        public string GetRequestCookie( IOwinContext context, string key )
        {
            if ( context == null ) throw new ArgumentNullException( nameof( context ) );

            var cookieValue = context.Request.Cookies[ key ];
            if ( cookieValue == null ) return null;

            var cookieParts = cookieValue.Split( Separator );
            // for backward compatibility of existing cookies during first upgrade that includes this feature
            return cookieParts.Length > 1 ? cookieParts[ 1 ] : cookieParts[ 0 ];
        }

        public const char Separator = '~';
    }
}