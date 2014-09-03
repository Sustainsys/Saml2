using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Kentor.AuthServices.TestHelpers;

namespace Kentor.AuthServices.Tests
{
    [TestClass]
    public class MetadataServer
    {
        private static IDisposable host;

        static IDictionary<PathString, Func<IOwinContext, Task>> actions =
            new Dictionary<PathString, Func<IOwinContext, Task>>()
            {
                { new PathString("/idpMetadata"), async ctx =>
                    {
                        var metadataXml = string.Format(
 @"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://localhost:13428/idpMetadata"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <KeyDescriptor use=""signing"">
        {0}
      </KeyDescriptor>
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST""
        Location=""http://localhost:13428/acs""/>
    </IDPSSODescriptor>
  </EntityDescriptor>
", SignedXmlHelper.KeyInfoXml);

                        await ctx.Response.WriteAsync(metadataXml);
                    }
                },
                { new PathString("/idpMetadataNoCertificate"), async ctx =>
                    {
                        var metadataXml =
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://localhost:13428/idpMetadataNoCertificate"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://localhost:13428/acs""/>
    </IDPSSODescriptor>
  </EntityDescriptor>";
                        await ctx.Response.WriteAsync(metadataXml);
                    }
                },
                { new PathString("/idpMetadataWrongEntityId"), async ctx =>
                    {
                        var metadataXml =
@"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://wrong.entityid.example.com"">
    <IDPSSODescriptor
      protocolSupportEnumeration=""urn:oasis:names:tc:SAML:2.0:protocol"">
      <SingleSignOnService
        Binding=""urn:oasis:names:tc:SAML:2.0:bindings:HTTP-Redirect""
        Location=""http://wrong.entityid.example.com/acs""/>
    </IDPSSODescriptor>
  </EntityDescriptor>";
                        await ctx.Response.WriteAsync(metadataXml);
                    }
                }
            };

        [AssemblyInitialize]
        public static void Start(TestContext testContext)
        {
            host = WebApp.Start("http://localhost:13428", app =>
            {
                app.Use(async (ctx, next) =>
                {
                    Func<IOwinContext, Task> action;
                    if(actions.TryGetValue(ctx.Request.Path, out action))
                    {
                        await action(ctx);
                    }
                    else
                    {
                        await next.Invoke();
                    }
                });
            });
        }

        [AssemblyCleanup]
        public static void Stop()
        {
            host.Dispose();
        }
    }
}
