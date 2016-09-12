using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Kentor.AuthServices.Internal;
using System.Collections.Generic;
using FluentAssertions;
using System.Linq;

namespace Kentor.AuthServices.Tests.Internal
{
    [TestClass]
    public class LazyDictionaryCollectionTests
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
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            dictionary["a"].Should().Be("b");
        }

        [TestMethod]
        public void SingleKeySingleValueShouldThrowKeyNotFoundExceptionWhenNoKeyFound()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            Action action = () =>
            {
                var x = dictionary["x"];
            };
            action.ShouldThrow<KeyNotFoundException>();
        }

        [TestMethod]
        public void DictionaryCountShouldReturnCorrectValue()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataMultipleKeySingleValue);
            dictionary.Count.Should().Be(2);
        }

        [TestMethod]
        public void KeysPropertyShouldReturnAllKeys()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataMultipleKeySingleValue);
            dictionary.Keys.Should().BeEquivalentTo(new[] { "a", "c" });
        }


        [TestMethod]
        public void ValuesPropertyShouldReturnAllValues()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataMultipleKeySingleValue);
            dictionary.Values.Should().BeEquivalentTo(new[] { "b", "d" });
        }

        [TestMethod]
        public void ValuesPropertyShouldThrowExceptionOnMultipleValues()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeyMultipleValues);
            Action action = () =>
            {
                var x = dictionary.Values.ToList();
            };
            action.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void SingleKeySingleValueContainsKeyShouldReturnTrueWhenFound()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            dictionary.ContainsKey("a").Should().BeTrue();
        }

        [TestMethod]
        public void SingleKeySingleValueContainsKeyShouldReturnFalseWhenNotFound()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            dictionary.ContainsKey("x").Should().BeFalse();
        }

        [TestMethod]
        public void SingleKeySingleValueTryGetValueKeyShouldReturnTrueWhenFound()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            string item;
            dictionary.TryGetValue("a", out item).Should().BeTrue();
            item.Should().Be("b");
        }

        [TestMethod]
        public void SingleKeySingleValueTryGetValueShouldReturnFalseWhenNotFound()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            string item;
            dictionary.TryGetValue("b", out item).Should().BeFalse();
            item.Should().BeNull();
        }

        [TestMethod]
        public void SingleKeySingleValueInDictionaryWithOtherMultipleKeysTryGetValueKeyShouldReturnTrueWhenFound()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataDuplicateSecondsKeySingleValue);
            string item;
            dictionary.TryGetValue("a", out item).Should().BeTrue();
        }

        [TestMethod]
        public void MultipleKeyShouldThrowException()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataDuplicateSecondsKeySingleValue);
            string item;
            Action action = () =>
            {
                var x = dictionary.TryGetValue("c", out item);
            };
            action.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void SingleKeySingleValueInDictionaryWithOtherMultipleKeyShouldRetrieveOk()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataDuplicateSecondsKeySingleValue);
            dictionary["a"].Should().Be("b");
        }

        [TestMethod]
        public void MultipleKeyShouldThrowExceptionWhenRetrievedByIndexer()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataDuplicateSecondsKeySingleValue);
            Action action = () =>
            {
                var x = dictionary["c"];
            };
            action.ShouldThrow<InvalidOperationException>();
        }

        [TestMethod]
        public void EnumeratorShouldReturnAllKeysAndValues()
        {
            var dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            var hits = 0;
            foreach (var kvp in dictionary)
            {
                kvp.Key.Should().Be("a");
                kvp.Value.Should().Be("b");

                hits++;
            }
            hits.Should().Be(1);
        }

        [TestMethod]
        public void PlainEnumeratorShouldReturnAllKeysAndValues()
        {
            System.Collections.IEnumerable dictionary = new LazyDictionaryCollection<string, string>(testDataSingleKeySingleValue);
            var hits = 0;
            foreach (KeyValuePair<string, string> kvp in dictionary)
            {
                kvp.Key.Should().Be("a");
                kvp.Value.Should().Be("b");

                hits++;
            }
            hits.Should().Be(1);
        }


        [TestMethod]
        public void NullCollectionShouldThrowException()
        {
            Action action = () =>
            {
                var dictionary = new LazyDictionaryCollection<string, string>(null);
            };
            action.ShouldThrow<ArgumentNullException>().And.ParamName.Should().Be("valueData");
        }
    }
}
