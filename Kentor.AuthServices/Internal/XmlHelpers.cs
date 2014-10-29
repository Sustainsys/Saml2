using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Kentor.AuthServices.Internal
{
    static class XmlHelpers
    {
        public static XElement AddAttributeIfNotNullOrEmpty(this XElement xElement, XName attribute, object value)
        {
            if (value != null && !string.IsNullOrEmpty(value.ToString()))
            {
                xElement.Add(new XAttribute(attribute, value));
            }
            return xElement;
        }

        public static string GetValueIfNotNull(this XmlAttribute xmlAttribute)
        {
            if (xmlAttribute == null)
            {
                return null;
            }
            return xmlAttribute.Value;
        }

        public static string GetTrimmedTextIfNotNull(this XmlElement xmlElement)
        {
            if (xmlElement == null)
            {
                return null;
            }

            return xmlElement.InnerText.Trim();
        }
    }
}
