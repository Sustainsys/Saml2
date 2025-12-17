// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using FluentAssertions;
using Sustainsys.Saml2.Common;
using Sustainsys.Saml2.Metadata;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;

namespace Sustainsys.Saml2.Protocol.Plus.Tests.Serialization;

partial class SamlXmlWriterPlusTests
{


    [Fact]
    public void WriteEntityDescriptor()
    {
        EntityDescriptor metaData = new()
        {
            Id = "_ae48f365-7f3f-4b82-a17f-08eb31347e65",
            EntityId = "https://stubidp.sustainsys.com/Metadata",
            CacheDuration = TimeSpan.FromMinutes(15),
            ValidUntil = new DateTimeUtc(2025, 12, 16, 12, 07, 49),
            RoleDescriptors =
            {
                new IDPSSODescriptor
                {
                    SingleLogoutServices =
                    {
                        new Endpoint
                        {
                            Binding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect",
                            Location = "https://stubidp.sustainsys.com/Logout"
                        },
                        new Endpoint
                        {
                            Binding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
                            Location = "https://stubidp.sustainsys.com/Logout"
                        }
                    },
                    SingleSignOnServices =
                    {
                        new Endpoint
                        {
                            Binding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect",
                            Location = "https://stubidp.sustainsys.com/"
                        },

                        new Endpoint
                        {
                            Binding = "urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST",
                            Location = "https://stubidp.sustainsys.com/"
                        }
                    }
                }
            }
        };
        var subject = new SamlXmlWriterPlus();

        var actual = subject.Write(metaData);
        var expected = GetXmlDocument();

        actual.Should().BeEquivalentTo(expected);
    }
};