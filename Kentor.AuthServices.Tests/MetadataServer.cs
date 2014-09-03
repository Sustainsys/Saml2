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
        
        [AssemblyInitialize]
        public static void Start(TestContext testContext)
        {
            host = WebApp.Start("http://localhost:13428", app =>
            {
                app.Use(async (ctx, next) =>
                {
                    if(ctx.Request.Path == new PathString("/idpmetadata"))
                    {
                        var metadataXml = string.Format(
 @"<EntityDescriptor xmlns=""urn:oasis:names:tc:SAML:2.0:metadata""
    entityID=""http://localhost:13428/idpmetadata"">
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

                        ctx.Response.Write(metadataXml);
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
