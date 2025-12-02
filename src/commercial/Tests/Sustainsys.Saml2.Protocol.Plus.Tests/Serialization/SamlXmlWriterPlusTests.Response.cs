// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using FluentAssertions;
using Sustainsys.Saml2.Saml;
using Sustainsys.Saml2.Samlp;
using Sustainsys.Saml2.Serialization;

namespace Sustainsys.Saml2.Protocol.Plus.Tests.Serialization;

partial class SamlXmlWriterPlusTests
{
    [Fact]
    public void WriteSamlResponse_MinimalErrorRequester()
    {
        Response response = new()
        {
            Id = "x123",
            IssueInstant = new(2025, 10, 07, 11, 13, 32),
            Status = new()
            {
                StatusCode = new()
                {
                    Value = Constants.StatusCodes.Requester
                }
            }
        };

        var subject = new SamlXmlWriterPlus();

        var actual = subject.Write(response);
        var expected = GetXmlDocument();

        actual.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public void WriteSamlResponse_CompleteSuccess()
    {
        Response response = new()
        {
            Id = "x123",
            InResponseTo = "x789",
            IssueInstant = new(2025, 10, 07, 13, 46, 32),
            Version = "2.0",
            Destination = "https://sp.example.com/Saml2/Acs",
            Issuer = new()
            {
                Format = "urn:oasis:names:tc:SAML:1.1:nameid-format:entity",
                Value = "https://idp.example.com/Metadata"
            },
            Status = new()
            {
                StatusCode = new()
                {
                    Value = Constants.StatusCodes.Success,
                },
            },
            Assertions =
            {
                new()
                {
                    Version="2.42",
                    Id="_0f9174fb-a286-43cf-93c8-197dfc6c58d2",
                    IssueInstant= new(2025, 10, 07, 13, 46, 33),
                    Issuer = new()
                    {
                        Value="https://idp.example.com/Metadata",
                    },
                    Subject= new()
                    {
                        NameId= new()
                        {
                            Format="urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified",
                            Value="x123456"
                        },
                        SubjectConfirmation = new()
                        {
                          Method = Constants.SubjectConfirmationMethods.Bearer,
                          SubjectConfirmationData = new()
                          {
                              NotOnOrAfter= new(2024,02,12,13,02,53),
                              Recipient= "https://sp.example.com/Saml2/Acs",
                              InResponseTo = "x789"
                          },
                        },
                    },
                    Conditions = new()
                    {
                        NotOnOrAfter = new(2025, 10, 07, 14, 46, 32),
                        AudienceRestrictions =
                        {
                            new()
                            {
                                Audiences = { "https://sp.example.com/Saml2" }
                            },
                        }
                    },
                    AuthnStatement = new()
                    {
                        AuthnInstant= new(2024,02,12,13,00,53),
                        SessionIndex="42",
                        AuthnContext=new()
                        {
                            AuthnContextClassRef= "urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified"
                        },
                    },
                      Attributes =
                    {
                         new SamlAttribute()
                        {
                            Name= "organisation",
                            Values= ["Sustainsys AB"],
                        },
                           new SamlAttribute()
                        {
                            Name= "email",
                            Values= [
                                "primary@example.com",
                                "secondary@example.com"
                            ],
                        },
                        new SamlAttribute()
                        {
                            Name= "NullAttribute",
                            Values= { null }
                        },
                        new SamlAttribute()
                        {
                            Name= "EmptyAttribute",
                            Values= { "" }
                        }
                    }
                  }
                }
        };
        var subject = new SamlXmlWriterPlus();

        var actual = subject.Write(response);
        var expected = GetXmlDocument();

        actual.Should().BeEquivalentTo(expected);
    }
};