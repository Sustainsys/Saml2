Kentor Authentication Services
==============================

The Kentor Authentication services is a couple of http modules that adds 
SAML2P support to IIS web sites, allowing the web site to act as a
SAML2 Service Provider (SP).

##Saml2AuthenticationModule
The Saml2AuthenticationModule is modeled after the WSFederationAuthenticationModule
to provide Saml2 authentication to IIS web sites. In many cases it should just be
configured in and work without any code written in the application at all (even
though providing an own ClaimsAuthenticationModule for claims translation is
highly recommended).

##SlidingExpirationSessionAuthenticationModule
The SlidingExpirationSessionAuthenticationModule is a bonus, it adds sliding expiration
to sessions just like FormsAuthentication works out of the box.
