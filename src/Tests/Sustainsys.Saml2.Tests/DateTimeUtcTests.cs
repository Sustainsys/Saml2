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
}

