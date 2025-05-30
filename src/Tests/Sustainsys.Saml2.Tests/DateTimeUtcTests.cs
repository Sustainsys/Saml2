using Sustainsys.Saml2.Common;

namespace Sustainsys.Saml2.Tests;

public class DateTimeUtcTests
{
    [Theory]
    [InlineData(DateTimeKind.Utc, false)]
    [InlineData(DateTimeKind.Unspecified, true)]
    [InlineData(DateTimeKind.Local, true)]
    public void DateTimeUtc_FromDateTime(DateTimeKind kind, bool shouldThrow)
    {
        DateTime source = new(2025, 05, 28, 21, 49, 43, kind);

        if (shouldThrow)
        {
            Action a = () => { DateTimeUtc destination = source; };
            a.Should().Throw<ArgumentException>();
        }
        else
        {
            // Should not throw
            DateTimeUtc destination = source;
            destination.Ticks.Should().Be(source.Ticks);
        }
    }

    [Fact]
    public void DateTimeUtc_ConvertsToDateTime()
    {
        var utcNow = DateTime.UtcNow;
        DateTimeUtc source = utcNow;

        DateTime destination = source;

        destination.Kind.Should().Be(DateTimeKind.Utc);
        destination.Ticks.Should().Be(source.Ticks);
    }

    [Fact]
    public void DateTimeUtc_FromDefaultDatetime()
    {
        DateTime source = default;

        DateTimeUtc destination = source;

        destination.Ticks.Should().Be(0);
    }

    [Fact]
    public void DateTimeUtc_NotFrom0TicksLocal()
    {
        DateTime source = new(0, DateTimeKind.Local);

        Action a = () => { DateTimeUtc destination = source; };

        a.Should().Throw<ArgumentException>();
    }

    public static TheoryData<DateTimeOffset, DateTimeUtc, bool> DateTimeUtcOffset_Less_than_DateTimeUtc_Data =>
        new()
        {
            // Same time is not less
            { new(2025, 01, 02, 03, 04, 05, TimeSpan.Zero), new(2025, 01, 02, 03, 04, 05), false },
            // Less time is less
            { new(2025, 01, 02, 03, 04, 04, TimeSpan.Zero), new(2025, 01, 02, 03, 04, 05), true },
            // Time in earlier timezone is less
            { new(2025, 01, 02, 03, 04, 05, TimeSpan.FromHours(1)), new(2025, 01, 02, 03, 04, 05), true },
            // Time in later timezone is not less
            { new(2025, 01, 02, 03, 04, 05, TimeSpan.FromHours(-1)), new(2025, 01, 02, 03, 04, 05), false },
            // Earlier time in ealier time zone is not less (it's equal)
            { new(2025, 01, 02, 04, 04, 05, TimeSpan.FromHours(1)), new(2025, 01, 02, 03, 04, 05), false }
        };

    [Theory]
    [MemberData(nameof(DateTimeUtcOffset_Less_than_DateTimeUtc_Data))]
    public void DateTimeUtcOffset_Less_than_DateTimeUtc(DateTimeOffset dto, DateTimeUtc dtu, bool expected)
    {
        (dto < dtu).Should().Be(expected);
    }

    public static TheoryData<DateTimeOffset, DateTimeUtc, bool> DateTimeUtcOffset_GreaterOrEqual_than_DateTimeUtc_Data =>
        new()
        {
            // Same time is greater or equal
            { new(2025, 01, 02, 03, 04, 05, TimeSpan.Zero), new(2025, 01, 02, 03, 04, 05), true },
            // Less time is not greater or equal
            { new(2025, 01, 02, 03, 04, 04, TimeSpan.Zero), new(2025, 01, 02, 03, 04, 05), false },
            // Later time is greater or equal
            { new(2025, 01, 02, 03, 04, 06, TimeSpan.Zero), new(2025, 01, 02, 03, 04, 05), true },
            // Time in earlier timezone is less
            { new(2025, 01, 02, 03, 04, 05, TimeSpan.FromHours(1)), new(2025, 01, 02, 03, 04, 05), false },
            // Time in later timezone is greater
            { new(2025, 01, 02, 03, 04, 05, TimeSpan.FromHours(-1)), new(2025, 01, 02, 03, 04, 05), true },
            // Earlier time in ealier time zone is equal)
            { new(2025, 01, 02, 04, 04, 05, TimeSpan.FromHours(1)), new(2025, 01, 02, 03, 04, 05), true }
        };

    [Theory]
    [MemberData(nameof(DateTimeUtcOffset_GreaterOrEqual_than_DateTimeUtc_Data))]
    public void DateTimeUtcOffset_GreaterEqual_than_DateTimeUtc(DateTimeOffset dto, DateTimeUtc dtu, bool expected)
    {
        (dto >= dtu).Should().Be(expected);
    }
}