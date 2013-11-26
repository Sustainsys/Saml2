Thank you for downloading Kentor.AuthServices.Mvc!

Kentor.AuthServices is copyright Kentor and contributors 2013 and licensed 
under LGPL which basically means that it is free to use, including in closed 
source applications. If you change the code of the library itself you have to 
make those changes open source. 

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

<kentor.authServices assertionConsumerServiceUrl="http://localhost:17009/SamplePath/Saml2AuthenticationModule/acs"
                            issuer="http://localhost:17009"
                            returnUri="http://localhost:17009/SamplePath/">
  <identityProvider issuer ="https://idp.example.com" destinationUri="httpss://idp.example.com" binding="HttpRedirect">
    <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                          findValue="idp.example.com" x509FindType="FindBySubjectName" />
  </identityProvider>
</kentor.authServices>

<system.identityModel>
  <identityConfiguration>
    <claimsAuthenticationManager type="Kentor.AuthServices.Tests.ClaimsAuthenticationManagerStub, Kentor.AuthServices.Tests"/>
  </identityConfiguration>
</system.identityModel>

<system.identityModel.services>
  <federationConfiguration>
    <cookieHandler requireSsl ="false"/>
  </federationConfiguration>
</system.identityModel.services>