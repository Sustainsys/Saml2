Getting Started
===============
See the sections below which contain information that will help you get started adding SAML2P support into 
your flavor of ASP.NET.

If you have gotten the appropriate Nuget package installed and then completed the configuration 
described below and are having any trouble, make sure to check out the :doc:`Troubleshooting <troubleshooting>` for assistance.

A sample SAML identity provider is available to further assist you in getting started if you don't already 
have a SAML identity provider that you can test with.  You can access it directly at https://stubidp.sustainsys.com, 
or you can download the solution to run it locally yourself (it's a project within the 
`Sustainsys.Saml2 github repository <https://github.com/Sustainsys/Saml2>`_).

ASP.NET Web Forms
-----------------
The ``Saml2AuthenticationModule`` provides Saml2 authentication to IIS web sites. In many cases it should just be 
:doc:`configured <configuration>` in the ``web.config`` file and work without any code written in the application at all 

Nuget Package to use: `Sustainsys.Saml2.HttpModule <https://www.nuget.org/packages/Sustainsys.Saml2.HttpModule/>`_

See :doc:`configuration` for information about how to configure the ``web.config`` file.

ASP.NET MVC
-----------
The ``MVC`` package contains an MVC controller that will be accessible
in your application just by installing the package in the 
application. For MVC applications a controller is preferred 
over using the authentication module as it integrates with MVC's 
error handling.

Nuget Package to use: `Sustainsys.Saml2.Mvc <https://www.nuget.org/packages/Sustainsys.Saml2.Mvc/>`_

See :doc:`configuration` for information about how to configure the ``web.config`` file.

Owin Middleware
---------------
The ``Owin`` middleware is modeled after the external 
authentication modules for social login (such as Google, Facebook, 
Twitter). This allows easy integration with ASP.NET Identity for 
keeping application specific user and role information. 

Nuget Package to use: `Sustainsys.Saml2.Owin <https://www.nuget.org/packages/Sustainsys.Saml2.Owin/>`_

See the :doc:`Owin Middleware <owin-middleware>` page for 
information on how to set up and use the middleware.

ASP.NET Core 2 Handler
----------------------
The ASP.NET Core 2 Handler is compatbile with the ASP.NET Core 2.0 
authentication model.

Nuget Package to use: `Sustainsys.Saml2.AspNetCore2 <https://www.nuget.org/packages/Sustainsys.Saml2.AspNetCore2/>`_

HOW TO CONFIGURE ASP.NET CORE 2 -- owin middleware doc?  somewhere else?

IdentityServer[3/4] Integration
-------------------------------
If you're using ``IdentityServer`` (v3 or later), you may want to 
configure SAML identity providers like Okta or Ping as external
identity providers within your IdentityServer implementation.

The ``Owin`` & ``ASP.NET Core2`` modules enable SAML identity 
providers to be integrated within `IdentityServer3 <https://github.com/IdentityServer/IdentityServer3>`_ and 
`IdentityServer4 <https://github.com/IdentityServer/IdentityServer4>`_ packages. 

Nuget Package to use for IdentityServer3: `Sustainsys.Saml2.Owin <https://www.nuget.org/packages/Sustainsys.Saml2.Owin/>`_
Nuget Package for IdentityServer4: `Sustainsys.Saml2.AspNetCore2 <https://www.nuget.org/packages/Sustainsys.Saml2.AspNetCore2/>`_

Review :doc:`this document <identity-server-3-okta>` to see how to configure Saml2 with 
IdentityServer3 and Okta to add Okta as an 
identity provider to an IdentityServer3 project. There is 
also a SampleIdentityServer3 project in the Saml2 repository.

.. note:: 

    There is also a Sustainsys.Saml2 Nuget package, but this only contains functionality shared 
    across the packages above and is not meant to be referenced directly in other projects.

.. note:: 

    The protocol handling classes are available as a public API as well, making it possible to reuse some of the 
    internals for writing your own service provider or identity provider.