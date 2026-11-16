[![.NET Core](https://github.com/Sustainsys/Saml2/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Sustainsys/Saml2/actions/workflows/dotnet.yml)

Sustainsys.Saml2
=============

The Sustainsys.Saml2 library adds SAML2P support to ASP.NET web sites, allowing the web site
to act as a SAML2 Service Provider (SP). The library was previously named Kentor.AuthServices.

## Documentation
Usage documentation is available at [our documentation site](https://saml2.sustainsys.com).

There are samples in the `v1` and `v2` branches. Newer samples are available in the 
[samples repo](https://github.com/Sustainsys/Saml2.Samples).

## Supported Versions
* :warning: The version 1.x packages are end of life March 2027.
* :white_check_mark: The version 2.x packages are supported for production use.
* :construction: The `main` branch will eventually be released as version 3, but is not yet ready.

See our [Security Policy](SECURITY.md) for details.

## Commercial Options and a Sustainable model for Open Source
When I started the [Sustainsys company](https://sustainsys.com/), the idea was to try to find
a sustainable model for open source work. Maintaining an open source library takes time. The
issue and PR list and constantly requires work to keep it clean (I've not always succeeded, 
I know). Bug reports by e-mail of possible security vulnerabilities need to be evaluated
and handled - even if they often turn out to be incorrect.
Any pull request - especially for a security library - need to be carefully evaluated to make sure
it works and doesn't break any existing behaviour. All of this takes time and my idea was to create
commercial options that were attractive enough to fund actual working time on the library. It has
partly succeeded, but not to the extent that we can spend the time we want on the library.
With the upcoming v3 release we will move to an open core model. If you just want to connect
a single application to a SAML 2.0 Identity Provider, that will still be available as an Open
Source option. But anything more advanced: dynamic configuration, custom validation or high
security features such as encryption will be moved to a paid tier.

## Branches
There are three active branches in the repo.
* `main` is the version in active development (currently v3) that only supports ASP.NET Core.
* `v1` is a legacy version that is going out of support. It uses the `System.IdentityModel`
  library for token handling and supports HttpModule, Mvc, Owin and AspNetCore2 (only on full
  .NET Framework). This branch  will only receive security fixes or critical compatibility
  fixes for major browsers.
* `v2` is a supported version that uses the `Microsoft.IdentityModel` nuget packages
  for token handling, multi-targets and supports HttpModule, Mvc, Owin and AspNetCore2. 
  This branch will only receive security fixes or critical compatibility fixes for major 
  browsers. Exceptions for new features can also be done for paying customers with support contracts.
* `master` is deprecated and only kept around to ensure all old links pointing to it works.

## Development
The current development goals are to get a new, more flexible architecture.
* Reading XML is done with more strict validation, but found errors can be suppressed.
* Use the Asp.Net Core RemoteAuthenticationHandler as a base class.
* Better support for the AspNet Core configuration system.
* Federations will be an own Authentication handler type (`builder.AddSaml2` vs `builder.AddSaml2Federation`).

Sustainsys.Saml2 is open sourced and contributions are welcome, please see [contribution guidelines](CONTRIBUTING.md)
for info on coding standards etc.

