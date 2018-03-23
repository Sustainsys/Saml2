using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

#if FALSE
// don't need this, there is one in Microsoft.IdentityModel.Xml. sigh.
namespace Sustainsys.Saml2.Metadata
{
    public class DelegatingXmlDictionaryReader : XmlDictionaryReader
    {
		public XmlDictionaryReader InnerReader { get; private set; }

		protected DelegatingXmlDictionaryReader()
		{
		}

		protected void InitializeInnerReader(XmlDictionaryReader innerReader)
		{
			if (innerReader == null)
			{
				throw new ArgumentNullException(nameof(innerReader));
			}
			InnerReader = innerReader;
		}

		public override string this[int i] => InnerReader[i];
		public override string this[string name] => InnerReader[name];
		public override string this[string name, string namespaceURI] => InnerReader[name, namespaceURI];
		public override int AttributeCount => InnerReader.AttributeCount;
		public override string BaseURI => InnerReader.BaseURI;
		public override int Depth => InnerReader.Depth;
		public override bool EOF => InnerReader.EOF;
		public override bool HasValue => InnerReader.HasValue;
		public override bool IsDefault => InnerReader.IsDefault;
		public override bool IsEmptyElement => InnerReader.IsEmptyElement;
		public override string LocalName => InnerReader.LocalName;
		public override string Name => InnerReader.Name;
		public override string NamespaceURI => InnerReader.NamespaceURI;
		public override XmlNameTable NameTable => InnerReader.NameTable;
		public override XmlNodeType NodeType => InnerReader.NodeType;
		public override string Prefix => InnerReader.Prefix;
		public override char QuoteChar => InnerReader.QuoteChar;
		public override ReadState ReadState => InnerReader.ReadState;
		public override string Value => InnerReader.Value;
		public override Type ValueType => InnerReader.ValueType;
		public override string XmlLang => InnerReader.XmlLang;
		public override XmlSpace XmlSpace => InnerReader.XmlSpace;

		public override void Close()
		{
			InnerReader.Close();
		}

		public override string GetAttribute(int i)
		{
			return InnerReader.GetAttribute(i);
		}

		public override string GetAttribute(string name)
		{
			return InnerReader.GetAttribute(name);
		}

		public override string GetAttribute(string name, string namespaceURI)
		{
			return InnerReader.GetAttribute(name, namespaceURI);
		}

		public override string LookupNamespace(string prefix)
		{
			return InnerReader.LookupNamespace(prefix);
		}

		public override void MoveToAttribute(int i)
		{
			InnerReader.MoveToAttribute(i);
		}

		public override bool MoveToAttribute(string name)
		{
			return InnerReader.MoveToAttribute(name);
		}

		public override bool MoveToAttribute(string name, string ns)
		{
			return InnerReader.MoveToAttribute(name, ns);
		}

		public override bool MoveToElement()
		{
			return InnerReader.MoveToElement();
		}

		public override bool MoveToFirstAttribute()
		{
			return InnerReader.MoveToFirstAttribute();
		}

		public override bool MoveToNextAttribute()
		{
			return InnerReader.MoveToNextAttribute();
		}

		public override bool Read()
		{
			return InnerReader.Read();
		}

		public override bool ReadAttributeValue()
		{
			return InnerReader.ReadAttributeValue();
		}

		public override int ReadContentAsBase64(byte[] buffer, int index, int count)
		{
			return InnerReader.ReadContentAsBase64(buffer, index, count);
		}

		public override int ReadContentAsBinHex(byte[] buffer, int index, int count)
		{
			return InnerReader.ReadContentAsBinHex(buffer, index, count);
		}

		public override UniqueId ReadContentAsUniqueId()
		{
			return InnerReader.ReadContentAsUniqueId();
		}

		public override int ReadValueChunk(char[] buffer, int index, int count)
		{
			return InnerReader.ReadValueChunk(buffer, index, count);
		}

		public override void ResolveEntity()
		{
			InnerReader.ResolveEntity();
		}
	}
}
#endif