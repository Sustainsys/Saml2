using Kentor.AuthServices.TestHelpers;

namespace Kentor.AuthServices.Owin.Tests
{
    class OwinStubFactory
    {
        internal static KentorAuthServicesAuthenticationOptions CreateOwinOptions()
        {
            return (KentorAuthServicesAuthenticationOptions)StubFactory.CreateOptions(
                sp => new KentorAuthServicesAuthenticationOptions(false)
                {
                    SPOptions = sp,
                    SignInAsAuthenticationType = "AuthType",
                    DataProtector = new StubDataProtector()
                });
        }
    }
}
