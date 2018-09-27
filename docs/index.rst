.. Sustainsys.Saml2 documentation master file, created by
   sphinx-quickstart on Fri Jan 26 05:33:50 2018.
   You can adapt this file completely to your liking, but it should at least
   contain the root `toctree` directive.

Welcome to Sustainsys.Saml2
===========================
The Sustainsys.Saml2 library adds SAML2P support to ASP.NET web sites, allowing the web site 
to act as a SAML2 Service Provider (SP). The library was previously named Kentor.AuthServices.
Sustainsys.Saml2 is open sourced and contributions are welcome, please see :doc:`contributing guidelines <contributing>` 
for info on coding standards etc.

Using Sustainsys.Saml2
----------------------
Using the Sustainsys.Saml2 library to add SAML2P support into your ASP.NET web applications is a two-step process:

#. Reference the Nuget package
#. Provide the necessary configuration information

The exact nature of these steps depends on the ASP.NET integration you're after.  
See :doc:`getting-started` for all the details.

Licensing
---------
The library is licensed under the `MIT license <https://github.com/Sustainsys/Saml2/blob/master/LICENSE>`_.

The library was previously (before version 2.0.0) licensed under the 
`GNU Lesser General Public License (LPGL) <https://www.gnu.org/licenses/lgpl-3.0.en.html>`_. Please note that
the license change only applies to the new versions, the old versions are still under the LGPL license.

.. toctree::
   :maxdepth: 2 
   :hidden:  
   :caption: Saml2

   getting-started
   configuration
   owin-middleware
   identity-server-3-okta
   troubleshooting
   contributing

.. toctree::
   :maxdepth: 2 
   :hidden:  
   :caption: web.config elements

   config-elements/sustainsys-saml2
   config-elements/name-id-policy
   config-elements/requested-authn-context
   config-elements/metadata
   config-elements/organization
   config-elements/contact-person
   config-elements/requested-attributes   
   config-elements/identity-providers
   config-elements/signing-certificate   
   config-elements/federations
   config-elements/service-certificates
   config-elements/compatibility

   

   

