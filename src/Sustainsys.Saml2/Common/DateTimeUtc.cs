namespace Sustainsys.Saml2.Common;

/// <summary>
/// DateTime that only allows DateTimeKind UTC.
/// </summary>
public struct DateTimeUtc
{
    /// <summary>
    /// Ticks of the DateTime
    /// </summary>
    public long Ticks { get; init; }

    private DateTimeUtc(long ticks)
    {
        Ticks = ticks;
    }

    /// <summary>
    /// Implicit conversion from DateTime, validates that the
    /// source DateTimeKind is Utc.
    /// </summary>
    /// <param name="source">Source DateTime</param>
    public static implicit operator DateTimeUtc(DateTime source)
    {
        if (source.Kind != DateTimeKind.Utc
            && (source.Ticks != 0 || source.Kind != DateTimeKind.Unspecified))
        {
            throw new ArgumentException("DateTime must be of Utc kind");
        }
        return new DateTimeUtc(source.Ticks);
    }

    /// <summary>
    /// Implicit conversion to DateTime.
    /// </summary>
    /// <param name="source">Source DateTimeUtc</param>
    public static implicit operator DateTime(DateTimeUtc source)
    {
        return new(source.Ticks, DateTimeKind.Utc);
    }
}