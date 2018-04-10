using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sustainsys.Saml2.Tests
{
	[TestClass]
    public class XsdDurationTests
    {
		[TestMethod]
		public void XsdDuration_TestComponentsCtor()
		{
			XsdDuration duration = new XsdDuration(true, 12, 10, 8, 6, 4, 2, 147);
			duration.Negative.Should().Be(true);
			duration.Years.Should().Be(12);
			duration.Months.Should().Be(10);
			duration.Days.Should().Be(8);
			duration.Hours.Should().Be(6);
			duration.Minutes.Should().Be(4);
			duration.Seconds.Should().Be(2);
			duration.Nanoseconds.Should().Be(147);
		}

		[TestMethod]
		public void XsdDuration_TestTimespanCtor()
		{
			XsdDuration duration = new XsdDuration(new TimeSpan(99, 13, 24, 11, 33));
			duration.Days.Should().Be(99);
			duration.Hours.Should().Be(13);
			duration.Minutes.Should().Be(24);
			duration.Seconds.Should().Be(11);
			duration.Nanoseconds.Should().Be(33000000);
		}

		[TestMethod]
		public void XsdDuration_TestParseNull()
		{
			Action a = () => XsdDuration.Parse(null);
			a.Should().Throw<ArgumentNullException>();
		}

		[TestMethod]
		public void XsdDuration_TestParseEmpty()
		{
			Action a = () => XsdDuration.Parse("");
			a.Should().Throw<FormatException>();
		}

		[TestMethod]
		public void XsdDuration_TestParseInvalidStart()
		{
			Action a = () => XsdDuration.Parse("X");
			a.Should().Throw<FormatException>();
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksNegative()
		{
			var x = new XsdDuration(negative: true);
			var y = new XsdDuration(negative: false);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksYears()
		{
			var x = new XsdDuration(years: 1);
			var y = new XsdDuration(years: 2);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksMonths()
		{
			var x = new XsdDuration(months: 3);
			var y = new XsdDuration(months: 2);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksDays()
		{
			var x = new XsdDuration(days: 1);
			var y = new XsdDuration(days: 2);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksHours()
		{
			var x = new XsdDuration(hours: 3);
			var y = new XsdDuration(hours: 2);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksMinutes()
		{
			var x = new XsdDuration(minutes: 1);
			var y = new XsdDuration(minutes: 3);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksSeconds()
		{
			var x = new XsdDuration(seconds: 5);
			var y = new XsdDuration(seconds: 6);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestEqualsChecksNanoseconds()
		{
			var x = new XsdDuration(nanoseconds: 31323);
			var y = new XsdDuration(nanoseconds: 99099);
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesNegative()
		{
			var x = new XsdDuration(negative: true).GetHashCode();
			var y = new XsdDuration(negative: false).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesYears()
		{
			var x = new XsdDuration(years: 3).GetHashCode();
			var y = new XsdDuration(years: 2).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesMonths()
		{
			var x = new XsdDuration(months: 3).GetHashCode();
			var y = new XsdDuration(months: 2).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesDays()
		{
			var x = new XsdDuration(days: 3).GetHashCode();
			var y = new XsdDuration(days: 2).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesHours()
		{
			var x = new XsdDuration(hours: 3).GetHashCode();
			var y = new XsdDuration(hours: 2).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesMinutes()
		{
			var x = new XsdDuration(minutes: 3).GetHashCode();
			var y = new XsdDuration(minutes: 2).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesSeconds()
		{
			var x = new XsdDuration(seconds: 3).GetHashCode();
			var y = new XsdDuration(seconds: 2).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestHashUsesNanoseconds()
		{
			var x = new XsdDuration(nanoseconds: 3).GetHashCode();
			var y = new XsdDuration(nanoseconds: 2).GetHashCode();
			x.Should().NotBe(y);
		}

		[TestMethod]
		public void XsdDuration_TestParseMinTimespanThrows()
		{
			Action a = () => new XsdDuration(new TimeSpan(Int64.MinValue));
			a.Should().Throw<OverflowException>();
		}

		[TestMethod]
		public void XsdDuration_TestParseIntOverflow()
		{
			Action a = () => XsdDuration.Parse("P8589934592Y");
			a.Should().Throw<OverflowException>();
		}

		[TestMethod]
		public void XsdDuration_TestParseEmptyTimeComponent()
		{
			XsdDuration duration;
			XsdDuration.TryParse("P1YT", out duration).Should().Be(false);
		}

		[TestMethod]
		public void XsdDuration_TestParseInvalidNumber()
		{
			XsdDuration duration;
			XsdDuration.TryParse("P-5YT", out duration).Should().Be(false);
		}

		[TestMethod]
		public void XsdDuration_TestParseDuplicateTimeSeparator()
		{
			XsdDuration duration;
			XsdDuration.TryParse("P5YTT42M", out duration).Should().Be(false);
		}

		[TestMethod]
		public void XsdDuration_TestParseYMD()
		{
			XsdDuration.Parse("P5Y3M14D").Should().Be(
				new XsdDuration(years: 5, months: 3, days: 14));
		}

		[TestMethod]
		public void XsdDuration_TestParseYMDNegative()
		{
			XsdDuration.Parse("-P5Y3M14D").Should().Be(
				new XsdDuration(negative: true, years: 5, months: 3, days: 14));
		}

		[TestMethod]
		public void XsdDuration_TestParseHMS()
		{
			XsdDuration.Parse("PT26H38M14S").Should().Be(
				new XsdDuration(hours: 26, minutes: 38, seconds: 14));
		}

		[TestMethod]
		public void XsdDuration_TestParseHMSNS()
		{
			XsdDuration.Parse("PT26H38M14.974S").Should().Be(
				new XsdDuration(hours: 26, minutes: 38, seconds: 14, nanoseconds: 974000000));
		}

		[TestMethod]
		public void XsdDuration_TestParseHMSNS_MissingNS()
		{
			Action a = () => XsdDuration.Parse("PT26H38M14.S");
			a.Should().Throw<FormatException>();
		}

		[TestMethod]
		public void XsdDuration_TestParseHMSNS_MissingS()
		{
			Action a = () => XsdDuration.Parse("PT26H38M14.393");
			a.Should().Throw<FormatException>();
		}

		[TestMethod]
		public void XsdDuration_TestToStringYMD()
		{
			new XsdDuration(years: 30, months: 4, days: 26).ToString()
				.Should().Be("P30Y4M26D");
		}

		[TestMethod]
		public void XsdDuration_TestToStringHMS()
		{
			new XsdDuration(hours: 12, minutes: 22, seconds: 13).ToString()
				.Should().Be("PT12H22M13S");
		}

		[TestMethod]
		public void XsdDuration_TestToStringNS100()
		{
			new XsdDuration(nanoseconds: 100).ToString()
				.Should().Be("PT0.0000001S");
		}

		[TestMethod]
		public void XsdDuration_TestToStringHMSNS()
		{
			new XsdDuration(hours: 12, minutes: 22, seconds: 13, nanoseconds: 123456000).ToString()
				.Should().Be("PT12H22M13.123456S");
		}

		[TestMethod]
		public void XsdDuration_TestToStringYMDHMSNS()
		{
			new XsdDuration(years: 24, months: 6, days: 12, 
				hours: 12, minutes: 22, seconds: 13, nanoseconds: 123456000).ToString()
				.Should().Be("P24Y6M12DT12H22M13.123456S");
		}

		[TestMethod]
		public void XsdDuration_TestToStringYMDHMSNSNegative()
		{
			new XsdDuration(negative: true, years: 24, months: 6, days: 12,
				hours: 12, minutes: 22, seconds: 13, nanoseconds: 123456000).ToString()
				.Should().Be("-P24Y6M12DT12H22M13.123456S");
		}

		[TestMethod]
		public void XsdDuration_TestToStringZero()
		{
			new XsdDuration().ToString().Should().Be("PT0S");
		}

		[TestMethod]
		public void XsdDuration_TestParseIgnoresLeadingAndTrailingWhitespace()
		{
			var result = XsdDuration.Parse("   PT1M   ");
			result.Should().Be(new XsdDuration(minutes: 1));
		}
    }
}
