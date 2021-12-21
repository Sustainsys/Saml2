[![.NET Core](https://github.com/Sustainsys/Saml2/workflows/.NET%20Core/badge.svg)](https://github.com/Sustainsys/Saml2/actions?query=workflow%3A%22.NET+Core%22)
[![Docs status](https://readthedocs.org/projects/saml2/badge/?version=latest)](https://saml2.sustainsys.com)
[![Join the chat at https://gitter.im/Susatinsys/Saml2](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Sustainsys/Saml2)
Sustainsys.Saml2
=============

The Sustainsys.Saml2 library adds SAML2P support to ASP.NET web sites, allowing the web site
to act as a SAML2 Service Provider (SP). The library was previously named Kentor.AuthServices.

Sustainsys.Saml2 is open sourced and contributions are welcome, please see 
[contributing guidelines](CONTRIBUTING.md) for info on coding standards etc.

## Branches
There are three active branches in the repo
* v1 is a security-supported-only version that uses the `System.IdentityModel` library for token handling and supports HttpModule, Mvc, Owin and AspNetCore2 (only on full. Net Framework).
* v2 is a currently supported version that uses the `Microsoft.IdentityModel` nuget packages for toking handling, multi-targets and supports HttpModule, Mvc, Owin and AspNetCore2 
* develop is development for a new version (will be released as v3 eventually) that only supports Asp.Net Core.
* master is deprecated and only kept around to ensure all old links pointing to it works.

## Documentation
Complete documentation is available at [our documentation site](https://saml2.sustainsys.com).
