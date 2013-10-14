Kentor.AuthServices Configuration
=============
To use Kentor.AuthServices in an application it must be enabled in the
application's `web.config`. The sample application contains a complete
working [`web.config`](../SampleApplication/Web.config).

##Config Sections
Three new confic sections are required. Add these under `configuration/configSections`:
```
<configSections>
  <!-- Add these sections below any existing. -->
  <section name="system.identityModel" type="System.IdentityModel.Configuration.SystemIdentityModelSection, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
  <section name="system.identityModel.services" type="System.IdentityModel.Services.Configuration.SystemIdentityModelServicesSection, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />   <section name="saml2AuthenticationModule" type="Kentor.AuthServices.Configuration.Saml2AuthenticationModuleSection, Kentor.AuthServices"/>
</configSections>
```

##saml2AuthenticationModule Section
The saml2AuthenticationModule section contains the configuration of the 
Kentor.AuthServices module.

```
<saml2AuthenticationModule assertionConsumerServiceUrl="http://localhost:17009/SamplePath/Saml2AuthenticationModule/acs"
                            returnUri="http://localhost:17009/SamplePath/">
  <identityProviders>
    <add issuer ="https://idp.example.com" destinationUri="httpss://idp.example.com" binding="HttpRedirect">
      <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                          findValue="idp.example.com" x509FindType="FindBySubjectName" />
    </add>
  </identityProviders>
</saml2AuthenticationModule>
```

###`<saml2AuthenticationModule>` element
Root element of the config section.

####Attributes
* [assertionConsumerServiceUrl](#assertionConsumerServiceUrl)
* [returnUri](#returnUri)

####Elements
* [identityProviders](#identityProviders)

####assertionConsumerServiceUrl
The assertionConsumerServiceUrl is the Url to which the Idp will post the 
Saml2 ticket. It should be the base Url of your application concatenated with 
`/Saml2AuthenticationModule/acs`. The relative Url is hard coded and cannot 
be changed.

####returnUri
The Uri that you want users to be redirected to once the authentication is
complete. This is typically the start page of the application, or a special
signed in start page.
