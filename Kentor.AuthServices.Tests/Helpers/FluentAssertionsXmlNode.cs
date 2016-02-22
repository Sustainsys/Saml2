// The code in this file is intended to be included in the FluentAssertions
// library when done, see https://github.com/dennisdoomen/FluentAssertions/issues/354.

using FluentAssertions;
using FluentAssertions.Execution;
using FluentAssertions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Kentor.AuthServices.Tests.Helpers
{
    class XmlReaderValidator
    {
        AssertionScope assertion;
        XmlReader subjectReader, expectedReader;

        string CurrentLocation
        {
            get { return "/" + string.Join("/", locationStack.Reverse()); }
        }

        Stack<string> locationStack = new Stack<string>();

        public XmlReaderValidator(XmlReader subjectReader, XmlReader expectedReader, string because, object[] reasonArgs)
        {
            assertion = Execute.Assertion.BecauseOf(because, reasonArgs);
            this.subjectReader = subjectReader;
            this.expectedReader = expectedReader;
        }

        public void Validate()
        {
            subjectReader.MoveToContent();
            expectedReader.MoveToContent();
            while (!subjectReader.EOF && !expectedReader.EOF)
            {
                if(subjectReader.NodeType != expectedReader.NodeType)
                {
                    assertion.FailWith("Expected node of type {0} at {1}{reason} but found {2}.");
                }

                switch(subjectReader.NodeType)
                {
                    case XmlNodeType.Element:
                        locationStack.Push(subjectReader.LocalName);
                        ValidateStartElement();
                        ValidateAttributes();
                        break;
                    case XmlNodeType.EndElement:
                        // No need to verify end element, if it doesn't match
                        // the start element it isn't valid XML, so the parser
                        // would handle that.
                        locationStack.Pop();
                        break;
                    case XmlNodeType.Text:
                        ValidateText();
                        break;
                    default:
                        throw new NotImplementedException();
                }

                subjectReader.Read();
                expectedReader.Read();

                subjectReader.MoveToContent();
                expectedReader.MoveToContent();
            }

            if(!expectedReader.EOF)
            {
                assertion.FailWith("Expected end of document{reason} but found {0}.",
                    expectedReader.LocalName);
            }

            if (!subjectReader.EOF)
            {
                assertion.FailWith("Expected {0}{reason}, but found end of document",
                    subjectReader.LocalName);
            }
        }

        class AttributeData
        {
            public AttributeData(string namespaceUri, string localName, string value)
            {
                NamespaceUri = namespaceUri;
                LocalName = localName;
                Value = value;
            }

            public string NamespaceUri { get; }
            public string LocalName { get; }
            public string Value { get; }
        }

        private void ValidateAttributes()
        {
            var expectedAttributes = GetAttributes(expectedReader);
            var subjectAttributes = GetAttributes(subjectReader);

            foreach(var subjectAttribute in subjectAttributes)
            {
                var expectedAttribute = expectedAttributes.SingleOrDefault(
                    ea => ea.NamespaceUri == subjectAttribute.NamespaceUri
                    && ea.LocalName == subjectAttribute.LocalName);

                if(expectedAttribute == null)
                {
                    assertion.FailWith("Didn't expect to find attribute {0} at {1}{reason}.",
                        subjectAttribute.LocalName, CurrentLocation);
                }

                if(subjectAttribute.Value != expectedAttribute.Value)
                {
                    assertion.FailWith("Expected attribute {0} at {1} to have a value of {2}{reason}, but found {3}",
                        subjectAttribute.LocalName, CurrentLocation, expectedAttribute.Value, subjectAttribute.Value);
                }
            }

            if(subjectAttributes.Count != expectedAttributes.Count)
            {
                var missingAttribute = expectedAttributes.First(ea =>
                    !subjectAttributes.Any(sa =>
                        ea.NamespaceUri == sa.NamespaceUri
                        && sa.LocalName == ea.LocalName));

                assertion.FailWith("Expected attribute {0} at {1}{reason}, but found none.",
                    missingAttribute.LocalName, CurrentLocation);
            }
        }

        private IList<AttributeData> GetAttributes(XmlReader reader)
        {
            var attributes = new List<AttributeData>();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (reader.NamespaceURI != "http://www.w3.org/2000/xmlns/")
                    {
                        attributes.Add(new AttributeData(reader.NamespaceURI, reader.LocalName, reader.Value));
                    }
                } while (reader.MoveToNextAttribute());
            }

            return attributes;
        }

        private void ValidateStartElement()
        {
            if (subjectReader.LocalName != expectedReader.LocalName)
            {
                assertion.FailWith("Expected local name of element at {0} to be {1}{reason}, but found {2}.",
                    CurrentLocation, expectedReader.LocalName, subjectReader.LocalName);
            }

            if (subjectReader.NamespaceURI != expectedReader.NamespaceURI)
            {
                assertion.FailWith("Expected namespace of element {0} at {1} to be {2}{reason}, but found {3}.",
                    subjectReader.LocalName, CurrentLocation, expectedReader.NamespaceURI, subjectReader.NamespaceURI);
            }
        }

        private void ValidateText()
        {
            var subject = subjectReader.Value;
            var expected = expectedReader.Value;

            if(subject != expected)
            {
                assertion.FailWith("Expected content to be {0}{reason} at {1} but found {2}.",
                    expected, CurrentLocation, subject);
            }
        }
    }

    public class XmlNodeAssertions : ReferenceTypeAssertions<XmlNode, XmlNodeAssertions>
    {
        public XmlNodeAssertions(XmlNode xmlNode)
        {
            Subject = xmlNode;
        }

        protected override string Context
        {
            get { return "Xml Node"; }
        }

        public AndConstraint<XmlNodeAssertions> BeEquivalentTo(XmlNode expected)
        {
            return BeEquivalentTo(expected, string.Empty);
        }

        public AndConstraint<XmlNodeAssertions> BeEquivalentTo(XmlNode expected, string because, params object[] reasonArgs)
        {
            new XmlReaderValidator(new XmlNodeReader(Subject), new XmlNodeReader(expected), because, reasonArgs).Validate();

            return new AndConstraint<XmlNodeAssertions>(this);
        }

    }

    public static class AssertionExtension
    {
        public static XmlNodeAssertions Should(this XmlNode actualValue)
        {
            return new XmlNodeAssertions(actualValue);
        }
    }
}
