ASP.NET CORE WORK IN PROGRESS
=============================

This branch contains work in progress on an ASP.NET Core version. If you want to help out,
please submit a PR against this branch. The build is not working yet, so don't worry if
your PR doesn't build either. As long as your work contains an improvement it will be accepted.


[![Build status](https://ci.appveyor.com/api/projects/status/ybu4ptb6tktg1kht?branch=aspnetcore&svg=true&passingText=aspnetcore%20-%20OK&failingText=aspnetcore%20-%20Failed!&pendingText=aspnetcore%20-%20Pending...)](https://ci.appveyor.com/project/KentorIT/AuthServices)
[![Coverage Status](https://coveralls.io/repos/KentorIT/authservices/badge.svg?branch=aspnetcore&service=github)](https://coveralls.io/github/KentorIT/authservices?branch=master)
[![Join the chat at https://gitter.im/KentorIT/authservices](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/KentorIT/authservices)
Kentor Authentication Services
=============

The Kentor Authentication services is a library that adds SAML2P support to ASP.NET and IIS
web sites, allowing the web site to act as a SAML2 Service Provider (SP).

Kentor.AuthServices is open sourced and contributions are welcome, please see 
[contributing guidelines](CONTRIBUTING.md) for info on coding standards etc.

##Using
The AuthServices library can be used through three different ways:

* An Http Module, loaded into the IIS pipeline. The module is compatible with ASP.NET web 
forms sites.
* An ASP.NET MVC Controller for better integration and error handling in ASP.NET Applications.
* An Owin Middleware to use with the Owin Pipeline or for integration with ASP.NET Identity.

Note that this last usage scenario enables SAML identity providers to be integrated within
[IdentityServer3](https://github.com/IdentityServer/IdentityServer3) package.  Review [this document](doc/IdentityServer3Okta.md) to see how to configure AuthServices
with IdentityServer3 and Okta to add Okta as an identity provider to an IdentityServer3 project.

There are four nuget packages available. The core 
[Kentor.AuthServices](https://www.nuget.org/packages/Kentor.AuthServices/) contains the core
functionality. The [Kentor.AuthServices.HttpModule](https://www.nuget.org/packages/Kentor.AuthServices.HttpModule/)
contains an IIS Http Module (previously this was included in the core package). 
The [Kentor.AuthServices.Mvc](https://www.nuget.org/packages/Kentor.AuthServices.Mvc/)
package contains the MVC controller and the [Kentor.AuthServices.Owin](https://www.nuget.org/packages/Kentor.AuthServices.Owin/)
package contains the Owin middleware.

Once installed the `web.config` of the application must be updated with configuration.
See [configuration](doc/Configuration.md) for details.

##Saml2AuthenticationModule
The Saml2AuthenticationModule is modeled after the WSFederationAuthenticationModule
to provide Saml2 authentication to IIS web sites. In many cases it should just be
[configured](doc/Configuration.md) in and work without any code written in the application 
at all (even though [providing an own ClaimsAuthenticationManager](doc/ClaimsAuthenticationManager.md)
for claims translation is highly recommended).

##Mvc Controller
The MVC package contains an MVC controller that will be accessible in your application just
by installing the package in the application. For MVC applications a controller is preferred
over using the authentication module as it integrates with MVC's error handling.

##Owin Middleware
The Owin middleware is modeled after the external authentication modules for social login
(such as Google, Facebook, Twitter). This allows easy integration with ASP.NET Identity 
for keeping application specific user and role information. See the 
[Owin Middleware](doc/OwinMiddleware.md) page for information on how to set up and use the middleware.

##Stub Idp
The solution also contains a stub (i.e. dummy) identity provider that can be used for testing.
Download the solution, or use the instance that's provided for free at http://stubidp.kentor.se.

##Protocol Classes
The protocol handling classes are available as a public API as well, making it possible to 
reuse some of the internals for writing your own service provider or identity provider.
