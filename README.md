[![.NET Core](https://github.com/Sustainsys/Saml2/workflows/.NET%20Core/badge.svg)](https://github.com/Sustainsys/Saml2/actions?query=workflow%3A%22.NET+Core%22)
[![Docs status](https://readthedocs.org/projects/saml2/badge/?version=latest)](https://saml2.sustainsys.com)
[![Join the chat at https://gitter.im/Susatinsys/Saml2](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Sustainsys/Saml2)
Sustainsys.Saml2
=============

The Sustainsys.Saml2 library adds SAML2P support to ASP.NET web sites, allowing the web site
to act as a SAML2 Service Provider (SP). The library was previously named Kentor.AuthServices.

Sustainsys.Saml2 is open sourced and contributions are welcome, please see 
[contributing guidelines](docs/contributiong.rst) for info on coding standards etc.

## Branches
There are three active branches in the repo
* v1 is a supported version that uses the `System.IdentityModel` library for token handling and supports HttpModule, Mvc, Owin and AspNetCore2 (only on full. Net Framework). This branch will only receive security fixes or critical compatibility fixes for major browsers.
* v2 is a currently supported version that uses the `Microsoft.IdentityModel` nuget packages for toking handling, multi-targets and supports HttpModule, Mvc, Owin and AspNetCore2. This branch will only receive security fixes or critical compatibility fixes for major browsers. Exceptions for new features can also be done for paying customers with support contracts.
* develop is development for a new version (will be released as v3 eventually) that only supports Asp.Net Core.
* master is deprecated and only kept around to ensure all old links pointing to it works.

## Current Development Goals
The current development goals are to get a new, more flexible architecture.
* The Metadata handling is broken out to a separate library that can be reused seprately. Some further work is probably needed to the structure.
* All `Web.config` code should be removed.
* The `RequestData` class should be removed - with Asp.Net Core as the only target we can take a direct dependency and use the `HttpContext` and `HttpRequest` directly.
* The `CommandResult` can probably be simplified, although having the result of a command in an abstract form before updating the `HttpResponse` is probably a good idea.

## Documentation
Usage documentation is available at [our documentation site](https://saml2.sustainsys.com).

## Commercial Options and Sustainable model for Open Source
When I started the Sustainsys company, the idea was to try to find a sustainable model for open source work. Maintaining an open source library takes time. The issue and PR list and constantly requires work to keep it clean (I've not always succeeded, I know). Bug reports by e-mail of possible security vulnerabilities need to be evaluated and handled - even if they often turn out to be incorrect. Any pull request - especially for a security library - need to be carefully evaluated to make sure it works and doesn't break any existing behaviour. All of this takes time and my idea was to create commercial options that were attractive enough to fund actual working time on the library. It has partly succeeded, but not to the extent that I can spend the time I want on the library.
If you are using the Sustainsys.Saml2 package in a larger organisation, please sign up for a commercial support package (mail to support@sustainsys.com for options). That will give your organisation support - and also support maintenance and development of the library.
