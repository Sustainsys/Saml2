Thank you for downloading Kentor.AuthServices.Owin!

Kentor.AuthServices is copyright Kentor 2013 and licensed under LGPL which
basically means that it is free to use, including in closed source 
applications. If you change the code of the library itself you have to make
those changes open source too. 

Please see the full license for details:
https://github.com/KentorIT/authservices/blob/master/LICENSE

Getting started
===============
To get started, you have to include the relevant configuration in your 
web.config. A full reference to the documentation is available at
https://github.com/KentorIT/authservices/blob/master/doc/Configuration.md
but the important parts can be found below for quick cut and paste.

The start page of the complete documentation is the readme at the project's
Github repository:
https://github.com/KentorIT/authservices/blob/master/README.md

Config Samples
==============
<configSections>
  <!-- Add these sections below any existing. -->
  <section name="system.identityModel" type="System.IdentityModel.Configuration.SystemIdentityModelSection, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
  <section name="system.identityModel.services" type="System.IdentityModel.Services.Configuration.SystemIdentityModelServicesSection, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />   
  <section name="kentor.authServices" type="Kentor.AuthServices.Configuration.KentorAuthServicesSection, Kentor.AuthServices"/>
</configSections>

<kentor.authServices assertionConsumerServiceUrl="http://localhost:57294/SamplePath/Saml2AuthenticationModule/acs"
                            entityId="http://localhost:57294/AuthServices"
                            returnUri="http://localhost:57294/Account/ExternalLoginCallback">">
  <identityProvider issuer ="http://stubidp.kentor.se" destinationUri="https://stubidp.kentor.se" binding="HttpRedirect">
    <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                          findValue="stubidp.kentor.se" x509FindType="FindBySubjectName" />
  </identityProvider>
</kentor.authServices>

<system.identityModel>
  <identityConfiguration />
</system.identityModel>
