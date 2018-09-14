Configuration
=============
To use Sustainsys.Saml2 in an application and configure it in ``web.config``
(which is the default for the ``HttpModule`` and ``MVC`` libraries) it must be **enabled**
in the application's ``web.config``. The sample applications contains complete
working `web.config <https://github.com/Sustainsys/Saml2/blob/master/Samples/SampleHttpModuleApplication/Web.config>`_ examples. For 
ASP.NET MVC applications see `this working web.config <https://github.com/Sustainsys/Saml2/blob/master/Samples/SampleMvcApplication/Web.config>`_
example. 

.. note::

    Applications using the ``Owin`` library usually make their configuration
    in code and in that case no web.config changes are needed. If an Owin library
    is set up to use web.config (by passing ``true`` to the ``Saml2AuthenticationOptions`` 
    constructor) the information here applies.

Config Sections
---------------
Three new config sections are required. Add these under ``configuration/configSections``.  Each of the sections
will be a child element of the main ``configuration`` section and each is described below.

.. code-block:: xml

    <configSections>
        <!-- Add these sections below any existing. -->
        <section name="system.identityModel" type="System.IdentityModel.Configuration.SystemIdentityModelSection, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
        <section name="system.identityModel.services" type="System.IdentityModel.Services.Configuration.SystemIdentityModelServicesSection, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />   
        <section name="sustainsys.saml2" type="Sustainsys.Saml2.Configuration.SustainsysSaml2Section, Sustainsys.Saml2"/>
    </configSections>

Loading Modules
---------------
When using the ``HttpModule`` and the ``MVC`` controller, the ``SessionAuthenticationModule`` needs
to be loaded and if using the http module that needs to be loaded as well. The ``Owin`` package
does not need any http modules, please see the separate info on the :doc:`owin-middleware`:.

.. code-block:: xml

    <system.webServer>
        <modules>
            <!-- Add these modules below any existing. The SessionAuthenticatioModule
                must be loaded before the Saml2AuthenticationModule -->
            <add name="SessionAuthenticationModule" type="System.IdentityModel.Services.SessionAuthenticationModule, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/>
            <!-- Only add the Saml2AuthenticationModule if you're using the Sustainsys.Saml2.HttpModule
                library. If you are using Sustainsys.Saml2.Mvc you SHOULD NOT load this module.-->
            <add name="Saml2AuthenticationModule" type="Sustainsys.Saml2.HttpModule.Saml2AuthenticationModule, Sustainsys.Saml2.HttpModule"/>
        </modules>
    </system.webServer>


Sustainsys.Saml2 Section
------------------------
The ``sustainsys.saml2`` section contains the configuration of the Sustainsys.Saml2
library. It is required for the http module and the mvc controller. The Owin middleware can
read web.config, but can also be configured from code (see :doc:`Owin middleware <owin-middleware>`).

A sample section is shown below.  For full details and all avaialble options, see  
:doc:`sustainsys.saml2 <config-elements/sustainsys-saml2>`.

.. code-block:: xml

    <sustainsys.saml2 entityId="http://localhost:17009"
                        returnUrl="http://localhost:17009/SamplePath/"
                        discoveryServiceUrl="http://localhost:52071/DiscoveryService" 
                        authenticateRequestSigningBehavior="Always">
        <nameIdPolicy allowCreate="true" format="Persistent"/>
        <metadata cacheDuration="0:0:42" validDuration="7.12:00:00" wantAssertionsSigned="true">
            <organization name="Sustainsys AB" displayName="Sustainsys" url="https://www.Sustainsys.com" language="sv" />
            <contactPerson type="Other" email="info@Sustainsys.se" />
            <requestedAttributes>
            <add friendlyName ="Some Name" name="urn:someName" nameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:uri" isRequired="true" />
            <add name="Minimal" />
            </requestedAttributes>
        </metadata>
        <identityProviders>
            <add entityId="https://stubidp.sustainsys.com/Metadata" 
                signOnUrl="https://stubidp.sustainsys.com" 
                allowUnsolicitedAuthnResponse="true"
                binding="HttpRedirect"
                wantAuthnRequestsSigned="true">
            <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                                findValue="Sustainsys.Saml2.StubIdp" x509FindType="FindBySubjectName" />
            </add>
            <add entityId="example-idp"
                metadataLocation="https://idp.example.com/Metadata"
                allowUnsolicitedAuthnResponse="true" 
                loadMetadata = "true" />
        </identityProviders>
        <!-- Optional configuration for signed requests. Required for Single Logout. -->
        <serviceCertificates>
            <add fileName="~/App_Data/Sustainsys.Saml2.Tests.pfx" />
        </serviceCertificates>
        <!-- Optional configuration for fetching IDP list from a federation -->
        <federations>
            <add metadataLocation="https://federation.example.com/metadata.xml" allowUnsolicitedAuthnResponse = "false" />
        </federations>
    </sustainsys.saml2>

System.IdentityModel Section
----------------------------
There must be a ``<system.identityModel>`` section in the config file or there will be a runtime error. The section can be 
empty (use ``<system.identityModel />``).

.. code-block:: xml

    <system.identityModel />

System.IdentityModel.Services Section
-------------------------------------
The ``<system.identityModel.services>`` element configures the built in servies. For testing on non ssl sites, the 
requirement for ssl for the session authentication cookie must be disabled.

.. danger::  
    It is a severe security risk to leave the ``requireSsl`` setting as false in a production environment.

.. code-block:: xml

    <system.identityModel.services>
        <federationConfiguration>
            <cookieHandler requireSsl ="false"/>
        </federationConfiguration>
    </system.identityModel.services>
