using Sustainsys.Saml2.TestHelpers;

namespace Sustainsys.Saml2.Owin.Tests
{
    class OwinStubFactory
    {
        internal static SustainsysSaml2AuthenticationOptions CreateOwinOptions()
        {
            return (SustainsysSaml2AuthenticationOptions)StubFactory.CreateOptions(
                sp => new SustainsysSaml2AuthenticationOptions(false)
                {
                    SPOptions = sp,
                    SignInAsAuthenticationType = "AuthType",
                    DataProtector = new StubDataProtector()
                });
        }
    }
}
