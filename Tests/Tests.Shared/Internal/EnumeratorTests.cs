using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using FluentAssertions;
using Sustainsys.Saml2.Internal;

namespace Sustainsys.Saml2.Tests.Internal
{
    [TestClass]
    public class EnumeratorTests
    {
        [TestMethod]
        public void Enumerator_AsGeneric()
        {
            IEnumerable src = new int[] { 1, 2 };

            var subject = src.GetEnumerator().AsGeneric<int>();

            subject.MoveNext().Should().BeTrue();
            subject.Current.Should().Be(1);
            subject.MoveNext().Should().BeTrue();
            subject.Current.Should().Be(2);
            subject.MoveNext().Should().BeFalse();

            subject.Reset();
            subject.MoveNext().Should().BeTrue();
            subject.Current.Should().Be(1);

            ((IEnumerator)subject).Current.Should().Be(1);
        }
    }
}
