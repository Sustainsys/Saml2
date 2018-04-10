using FluentAssertions;
using Sustainsys.Saml2.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class DateTimeHelperTests
    {
        [TestMethod]
        public void DateTimeHelper_EarliestTime_NullOnBothNull()
        {
            DateTimeHelper.EarliestTime(null, null)
                .Should().NotHaveValue("earliest of null and null is null");
        }

        [TestMethod]
        public void DateTimeHelper_EarliestTime_Value1OnValue2Null()
        {
            var dt = new DateTime(2016, 06, 30);

            DateTimeHelper.EarliestTime(dt, null)
                .Should().Be(dt, "earliest of null and value should be the value");
        }

        [TestMethod]
        public void DateTimeHelper_EarliestTime_Value2OnValue1Null()
        {
            var dt = new DateTime(2016, 06, 30);

            DateTimeHelper.EarliestTime(null, dt)
                .Should().Be(dt, "earliest of null and value should be the value");
        }

        [TestMethod]
        public void DateTimeHelper_EarliestTime_Value1OnValue2Later()
        {
            var dt = new DateTime(2016, 06, 30);

            DateTimeHelper.EarliestTime(dt, dt.AddDays(1))
                .Should().Be(dt, "earliest value should be returned");
        }

        [TestMethod]
        public void DateTimeHelper_EarliestTime_Value2OnVAlue1Later()
        {
            var dt = new DateTime(2016, 06, 30);

            DateTimeHelper.EarliestTime(dt.AddDays(1), dt)
                .Should().Be(dt, "earliest value should be returned");
        }
    }
}
