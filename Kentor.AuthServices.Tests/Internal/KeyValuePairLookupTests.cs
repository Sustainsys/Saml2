using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Internal;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class KeyValuePairLookupTests
    {
        KeyValuePair<string, string[]>[] testDataSingleKeySingleValue = {
            new KeyValuePair<string, string[]>("a", new[] {"b"}),
        };
        KeyValuePair<string, string[]>[] testDataSingleKeyMultipleValues = {
            new KeyValuePair<string, string[]>("a", new[] {"b", "c"}),
        };
        KeyValuePair<string, string[]>[] testDataMultipleKeySingleValue = {
            new KeyValuePair<string, string[]>("a", new[] {"b"}),
            new KeyValuePair<string, string[]>("c", new[] {"d"}),
        };
        KeyValuePair<string, string[]>[] testDataDuplicateSecondsKeySingleValue = {
            new KeyValuePair<string, string[]>("a", new[] {"b"}),
            new KeyValuePair<string, string[]>("c", new[] {"d"}),
            new KeyValuePair<string, string[]>("c", new[] {"e"}),
        };

        [TestMethod]
        public void SingleKeySingleValueShouldRetrieveOkByKey()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataSingleKeySingleValue);
            lookup["a"].Should().Be("b");
        }

        [TestMethod]
        public void SingleKeySingleValueShouldThrowKeyNotFoundExceptionWhenNoKeyFound()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataSingleKeySingleValue);
            Action action = () =>
            {
                var x = lookup["x"];
            };
            action.ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod]
        public void LookupCountShouldReturnCorrectValue()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataMultipleKeySingleValue);
            lookup.Count.Should().Be(2);
        }

        [TestMethod]
        public void SingleKeySingleValueContainsKeyShouldReturnTrueWhenFound()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataSingleKeySingleValue);
            lookup.ContainsKey("a").Should().BeTrue();
        }

        [TestMethod]
        public void SingleKeySingleValueContainsKeyShouldReturnFalseWhenNotFound()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataSingleKeySingleValue);
            lookup.ContainsKey("x").Should().BeFalse();
        }

        [TestMethod]
        public void SingleKeySingleValueTryGetValueKeyShouldReturnTrueWhenFound()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataSingleKeySingleValue);
            string item;
            lookup.TryGetValue("a", out item).Should().BeTrue();
            item.Should().Be("b");
        }

        [TestMethod]
        public void SingleKeySingleValueTryGetValueShouldReturnFalseWhenNotFound()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataSingleKeySingleValue);
            string item;
            lookup.TryGetValue("b", out item).Should().BeFalse();
            item.Should().BeNull();
        }

        [TestMethod]
        public void SingleKeySingleValueInLookupWithOtherMultipleKeysTryGetValueKeyShouldReturnTrueWhenFound()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataDuplicateSecondsKeySingleValue);
            string item;
            lookup.TryGetValue("a", out item).Should().BeTrue();
        }

        [TestMethod]
        public void MultipleKeyShouldThrowException()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataDuplicateSecondsKeySingleValue);
            string item;
            Action action = () =>
            {
                var x = lookup.TryGetValue("c", out item);
            };
            action.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void SingleKeySingleValueInLookupWithOtherMultipleKeyShouldRetrieveOk()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataDuplicateSecondsKeySingleValue);
            lookup["a"].Should().Be("b");
        }

        [TestMethod]
        public void MultipleKeyShouldThrowExceptionWhenRetrievedByIndexer()
        {
            var lookup = new KeyValuePairLookup<string, string>(testDataDuplicateSecondsKeySingleValue);
            Action action = () =>
            {
                var x = lookup["c"];
            };
            action.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void NullCollectionShouldThrowException()
        {
            Action action = () =>
            {
                var lookup = new KeyValuePairLookup<string, string>(null);
            };
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("valueData");
        }
    }
}
