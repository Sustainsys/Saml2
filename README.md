[![.NET Core](https://github.com/Sustainsys/Saml2/workflows/.NET/badge.svg)](https://github.com/Sustainsys/Saml2/actions/workflows/dotnet.yml)
[![Join the chat at https://gitter.im/Susatinsys/Saml2](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Sustainsys/Saml2)

Sustainsys.Saml2
=============

The Sustainsys.Saml2 library adds SAML2P support to ASP.NET web sites, allowing the web site
to act as a SAML2 Service Provider (SP). The library was previously named Kentor.AuthServices.

## Documentation
Usage documentation is available at [our documentation site](https://saml2.sustainsys.com).

There are samples in the `v1` and `v2` branches. Newer samples are available in the 
[samples repo](https://github.com/Sustainsys/Saml2.Samples).

## Commercial Options and a Sustainable model for Open Source
When I started the Sustainsys company, the idea was to try to find a sustainable model for open
source work. Maintaining an open source library takes time. The issue and PR list and constantly
requires work to keep it clean (I've not always succeeded, I know). Bug reports by e-mail of possible
security vulnerabilities need to be evaluated and handled - even if they often turn out to be incorrect.
Any pull request - especially for a security library - need to be carefully evaluated to make sure
it works and doesn't break any existing behaviour. All of this takes time and my idea was to create
commercial options that were attractive enough to fund actual working time on the library. It has
partly succeeded, but not to the extent that I can spend the time I want on the library.
If you are using the Sustainsys.Saml2 package in a larger organisation, please sign up for a
commercial support package (mail to support@sustainsys.com for options). That will give your
organisation support - and also support maintenance and development of the library.

## Branches
There are three active branches in the repo.
* `develop` is development for a new version (will be released as v3 eventually) that only
  supports Asp.Net Core.
* `v1` is a supported version that uses the `System.IdentityModel` library for token handling
  and supports HttpModule, Mvc, Owin and AspNetCore2 (only on full. Net Framework). This branch 
  will only receive security fixes or critical compatibility fixes for major browsers.
* `v2` is a supported version that uses the `Microsoft.IdentityModel` nuget packages
  for tokeng handling, multi-targets and supports HttpModule, Mvc, Owin and AspNetCore2. 
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
