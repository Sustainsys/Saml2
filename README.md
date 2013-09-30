Kentor Authentication Services
=============

The Kentor Authentication services is a an http modules that adds 
SAML2P support to IIS web sites, allowing the web site to act as a
SAML2 Service Provider (SP).

##Saml2AuthenticationModule
The Saml2AuthenticationModule is modeled after the WSFederationAuthenticationModule
to provide Saml2 authentication to IIS web sites. In many cases it should just be
configured in and work without any code written in the application at all (even
though providing an own ClaimsAuthenticationModule for claims translation is
highly recommended).

###Protocol classes
The protocol handling classes that are used by the Saml2AuthenticationModule are available
as a public API as well, making it possible to reuse some of the internals for writing
an own service provider.
