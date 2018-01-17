using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Owin.Tests
{
    class OwinStubFactory
    {
        internal static Saml2AuthenticationOptions CreateOwinOptions()
        {
            return (Saml2AuthenticationOptions)StubFactory.CreateOptions(
                sp => new Saml2AuthenticationOptions(false)
                {
                    SPOptions = sp,
                    SignInAsAuthenticationType = "AuthType",
                    DataProtector = new StubDataProtector()
                });
        }
    }
}
