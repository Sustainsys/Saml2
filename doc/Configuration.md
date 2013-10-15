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
  <section name="system.identityModel.services" type="System.IdentityModel.Services.Configuration.SystemIdentityModelServicesSection, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />   
  <section name="kentor.authServices" type="Kentor.AuthServices.Configuration.KentorAuthServicesSection, Kentor.AuthServices"/>
</configSections>
```

##kentor.authServices Section
The saml2AuthenticationModule section contains the configuration of the 
Kentor.AuthServices module.

```
<kentor.authServices assertionConsumerServiceUrl="http://localhost:17009/SamplePath/Saml2AuthenticationModule/acs"
							issuer="http://localhost:17009"
                            returnUri="http://localhost:17009/SamplePath/">
  <identityProviders>
    <identityProvider issuer ="https://idp.example.com" destinationUri="httpss://idp.example.com" binding="HttpRedirect">
      <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                          findValue="idp.example.com" x509FindType="FindBySubjectName" />
    </identityProvider>
  </identityProviders>
</kentor.authServices>
```

###`<kentor.authServices>` Element
*Child element of `<configuration>` element.*

Root element of the config section.

####Attributes
* [`assertionConsumerServiceUrl`](#assertionconsumerserviceurl-attribute)
* [`returnUri`](#returnuri-attribute)
* [`issuer`](#issuer-attribute)

####Elements
* [`<identityProviders>`](#identityproviders-element). Collection of 
[`<dentityProvider>` elements](#identityprovider-element)

####`assertionConsumerServiceUrl` Attribute
*Attribute of [`<kentor.authServices>` element](kentor-authservices-element)*

The assertionConsumerServiceUrl is the Url to which the Idp will post the 
Saml2 ticket. It should be the base Url of your application concatenated with 
`/Saml2AuthenticationModule/acs`. The relative Url is hard coded and cannot 
be changed.

####`issuer` Attribute
*Attribute of [`<kentor.authServices>` element](kentor-authservices-element)*

The name that this service provider will use for itself when sending
messages. The name will end up in the `issuer` field in outcoing authnRequests.

####`returnUri` Attribute
*Attribute of [`<kentor.authServices>` element](kentor-authservices-element)*

The Uri that you want users to be redirected to once the authentication is
complete. This is typically the start page of the application, or a special
signed in start page.

####`<identityProviders>` Element
*Child element of [`<kentor.authServices>` element](kentor-authservices-element)*

Only one identity provider is currently supported so only the first entry
will be considered.

####Elements
* [identityProvider](#identityProvider-Element)

###`<identityProvider>` Element
*Child element of [`<identityProviders>` element](#identityproviders-element)*

An identity provider that the Service Provider relies on for authentication.

####Attributes
* [`issuer`](#issuer-attribute-identityProvider)
* [`destinationUri`](#destinationuri-attribute)
* [`binding`](#binding-attribute)

####Elements
* [`<signingCertificate>`](#signingcertificate-element)

####`<issuer>` Attribute (identityProvider)
*Attribute of [`<identityProvider>`](