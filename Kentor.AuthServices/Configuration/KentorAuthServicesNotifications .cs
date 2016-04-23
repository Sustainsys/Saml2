using System.Collections.Generic;
using Kentor.AuthServices.Saml2P;

namespace Kentor.AuthServices.Configuration
{
    public class KentorAuthServicesNotifications
    {
        public KentorAuthServicesNotifications()
        {
            AuthenticationRequestCreated = (request, provider, dictionary) => { };
        }
        public AuthenticationRequestCreatedNotification  AuthenticationRequestCreated { get; set; }
    }

    public delegate void AuthenticationRequestCreatedNotification(Saml2AuthenticationRequest saml2AuthenticationRequest, IdentityProvider identityProvider, IDictionary<string,string> dictionary);

    /*
    public class AuthenticationRequestCreatedNotification :
        KentorAuthServicesNotification<Saml2AuthenticationRequest, IdentityProvider, IDictionary<string, string>>
    {
        public AuthenticationRequestCreatedNotification(Action<Saml2AuthenticationRequest, IdentityProvider, IDictionary<string, string>> handler) : base(handler)
        {
        }
    }

    public class KentorAuthServicesNotification<T1,T2,T3>
    {
        private readonly Action<T1, T2, T3> _handler;

        protected KentorAuthServicesNotification(Action<T1, T2, T3> handler)
        {
            _handler = handler;
        }

        public void Invoke(T1 type1, T2 type2, T3 type3)
        {
            _handler(type1, type2, type3);
        }
    }
    */
}
