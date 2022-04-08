using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Metadata;

/// <summary>
/// Map helper for enum.
/// </summary>
public static class EnumMapper
{
    /// <summary>
    /// Gets the enum value corresponding to a uri
    /// </summary>
    /// <typeparam name="T">Enum type</typeparam>
    /// <param name="uri">Uri to look for</param>
    /// <returns>Enum or default() if not matched.</returns>
    public static T MapEnum<T>(this string? uri)
        where T : Enum
    {
        var fields = typeof(T).GetFields();

        var field = fields.SingleOrDefault(f => f.GetCustomAttributes(false).OfType<UriAttribute>().SingleOrDefault()?.Uri == uri);

        if(field == null)
        {
            return default!;
        }

        return (T)field.GetValue(null)!;
    }
}
