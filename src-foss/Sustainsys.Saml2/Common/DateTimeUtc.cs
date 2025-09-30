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

    /// <summary>
    /// Construct a DateTimeUtc from ticks
    /// </summary>
    /// <param name="ticks"></param>
    public DateTimeUtc(long ticks)
    {
        Ticks = ticks;
    }

    /// <summary>
    /// Construct a DateTimeUtc
    /// </summary>
    /// <param name="year">Year</param>
    /// <param name="month">Month</param>
    /// <param name="day">Day</param>
    /// <param name="hour">Hour</param>
    /// <param name="minute">Minute</param>
    /// <param name="second">Second</param>
    public DateTimeUtc(int year, int month, int day, int hour, int minute, int second)
    {
        DateTime dt = new(year, month, day, hour, minute, second, DateTimeKind.Utc);

        Ticks = dt.Ticks;
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

    /// <summary>
    /// Implicit conversion to DateTimeOffset
    /// </summary>
    /// <param name="source">Source DateTimeUtc</param>
    public static implicit operator DateTimeOffset(DateTimeUtc source)
        => new(source);

    /// <summary>
    /// Operator greater than
    /// </summary>
    /// <param name="dto">DateTimeOffset</param>
    /// <param name="dtu">DateTimeUtc</param>
    /// <returns>Bool result</returns>
    public static bool operator <(DateTimeOffset dto, DateTimeUtc dtu) =>
        dto < (DateTime)dtu;

    /// <summary>
    /// Operator less than
    /// </summary>
    /// <param name="dto">DateTimeOffset</param>
    /// <param name="dtu">DateTimeUtc</param>
    /// <returns>Bool result</returns>
    /// <exception cref="NotImplementedException">Method is not implemented</exception>
    public static bool operator >(DateTimeOffset dto, DateTimeUtc dtu) =>
        throw new NotImplementedException();

    /// <summary>
    /// Operator greater or equal than
    /// </summary>
    /// <param name="dto">DateTimeOffset</param>
    /// <param name="dtu">DateTimeUtc</param>
    /// <returns>Bool result</returns>
    public static bool operator >=(DateTimeOffset dto, DateTimeUtc dtu) =>
        dto >= (DateTime)dtu;

    /// <summary>
    /// Operator less or equal than
    /// </summary>
    /// <param name="dto">DateTimeOffset</param>
    /// <param name="dtu">DateTimeUtc</param>
    /// <returns>Bool result</returns>
    /// <exception cref="NotImplementedException">Method is not implemented</exception>
    public static bool operator <=(DateTimeOffset dto, DateTimeUtc dtu) =>
        throw new NotImplementedException();

    /// <summary>
    /// ToString
    /// </summary>
    /// <returns>String</returns>
    public override string ToString() => ((DateTime)this).ToString();
}