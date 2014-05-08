Kentor Authentication Services
=============

The Kentor Authentication services is an http modules that adds 
SAML2P support to IIS web sites, allowing the web site to act as a
SAML2 Service Provider (SP).

Kentor.AuthServices is open sourced and contributions are welcome, please see 
[contributing guidelines](doc/Contributing.md) for info on coding standards etc.

##Using
There's a [Nuget package](https://www.nuget.org/packages/Kentor.AuthServices/) available 
for simple installation.

> `PM> Install-Package Kentor.AuthServices`

Once installed the `web.config` of the application must be updated to load and configure
the http module and the System.IdentityModel.Services that it is using. See [configuration]
(doc/Configuration.md) for details.

For MVC projects there's an even easier zero coding 
[Nuget package](https://www.nuget.org/packages/Kentor.AuthServices.Mvc/) that will add a 
new controllerto your project, containing `SignIn` and `SignOut` actions.

> `PM> Install-Package Kentor.AuthServices.Mvc`

See [configuration](doc/Configuration.md) for info on how to add the needed sections to `web.config`.

##Saml2AuthenticationModule
The Saml2AuthenticationModule is modeled after the WSFederationAuthenticationModule
to provide Saml2 authentication to IIS web sites. In many cases it should just be
[configured](doc/Configuration.md) in and work without any code written in the application 
at all (even though [providing an own ClaimsAuthenticationManager](doc/ClaimsAuthenticationManager.md)
for claims translation is highly recommended).

###Protocol Classes
The protocol handling classes that are used by the Saml2AuthenticationModule are available
as a public API as well, making it possible to reuse some of the internals for writing
an own service provider.

##Mvc Controller
The MVC package contains an MVC controller that will be accessible in your application just
by installing the package in the application. For MVC applications a controller is preferred
over using the authentication module as it integrates with MVC's error handling.

