using Sustainsys.Saml2.Metadata.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Sustainsys.Saml2.Metadata.Tests.Xml;
public class XmlHelkperTests
{
    [Fact]
    public void CreateIdGivesNcName()
    {
        // For every possible initial encoding point (6 bits), the result
        // should be a valid NCName.
        for (int i = 0; i < 0xff; i += 0x4)
        {
            var bytes = new[] { (byte)(i << 8) };

            var actual = XmlHelpers.FormatId(bytes);

            // Returns input on success, throws on failure.
            XmlConvert.VerifyNCName(actual).Should().Be(actual);
        }

        // For every possible later encoding point (6 bits), the result
        // should be a valid NCName.
        for (byte i = 0; i < 0x40; i++)
        {
            var bytes = new[] { (byte)1, (byte)1, i };

            var actual = XmlHelpers.FormatId(bytes);

            // Returns input on success, throws on failure.
            XmlConvert.VerifyNCName(actual).Should().Be(actual);
        }
    }
}
