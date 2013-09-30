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

##Versioning
Kentor Authentication services uses semantic versioning as defined on http://semver.org/.

    Given a version number MAJOR.MINOR.PATCH, increment the:

    MAJOR version when you make incompatible API changes,
    MINOR version when you add functionality in a backwards-compatible manner, and
    PATCH version when you make backwards-compatible bug fixes.

Additionally *even* PATCH numbers are releases that corresponds to a tag in the 
repository. *Odd* PATCH numbers are development versions. This means that the 
current code in the repository will always have an *odd* PATCH number to denote that 
it is a development version.
