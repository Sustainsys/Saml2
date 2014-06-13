kentor.AuthServices Configuration
=============
To use Kentor.AuthServices in an application it must be enabled in the
application's `web.config`. The sample application contains a complete
working [`web.config`](../SampleApplication/Web.config).

##Config Sections
Three new config sections are required. Add these under `configuration/configSections`:
```
<configSections>
  <!-- Add these sections below any existing. -->
  <section name="system.identityModel" type="System.IdentityModel.Configuration.SystemIdentityModelSection, System.IdentityModel, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />
  <section name="system.identityModel.services" type="System.IdentityModel.Services.Configuration.SystemIdentityModelServicesSection, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=B77A5C561934E089" />   
  <section name="kentor.authServices" type="Kentor.AuthServices.Configuration.KentorAuthServicesSection, Kentor.AuthServices"/>
</configSections>
```
###Loading modules
Http modules need to be loaded. The `SessionAuthenticationModule` is always required.
The `Saml2AuthenticationModule` is required if you use the bare Kentor.AuthServices
library. If you are using Kentor.AuthServices.Mvc it should not be loaded.

```
<system.web>
  <httpModules>
	<!-- Add these modules below any existing. -->
    <add name="SessionAuthenticationModule" type="System.IdentityModel.Services.SessionAuthenticationModule, System.IdentityModel.Services, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"/>
    <!-- Only add the Saml2AuthenticationModule if you're ranning the bare Kentor.AuthServices
		library. If you are using Kentor.AuthServices.Mvc you SHOULD NOT load this module.-->
	<add name="Saml2AuthenticationModule" type="Kentor.AuthServices.Saml2AuthenticationModule, Kentor.AuthServices"/>
  </httpModules>
</system.web>
```

##kentor.authServices Section
The saml2AuthenticationModule section contains the configuration of the 
Kentor.AuthServices module.

```
<kentor.authServices assertionConsumerServiceUrl="http://localhost:17009/SamplePath/Saml2AuthenticationModule/acs"
							entityId="http://localhost:17009"
                            returnUri="http://localhost:17009/SamplePath/">
  <identityProvider entityId ="https://idp.example.com" destinationUri="https://idp.example.com" 
                    allowUnsolicitedAuthnResponse="true" binding="HttpRedirect">
    <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                          findValue="idp.example.com" x509FindType="FindBySubjectName" />
  </identityProvider>
</kentor.authServices>
```

###`<kentor.authServices>` Element
*Child element of `<configuration>` element.*

Root element of the config section.

####Attributes
* [`assertionConsumerServiceUrl`](#assertionconsumerserviceurl-attribute)
* [`returnUri`](#returnuri-attribute)
* [`entityId`](#entityid-attribute)

####Elements
* [`<identityProvider>`](#identityprovider-element)

####`assertionConsumerServiceUrl` Attribute
*Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element*

The assertionConsumerServiceUrl is the Url to which the Idp will post the 
Saml2 ticket. It should be the base Url of your application concatenated with 
`/Saml2AuthenticationModule/acs`. The relative Url is hard coded and cannot 
be changed.

####`entityId` Attribute
*Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element*

The name that this service provider will use for itself when sending
messages. The name will end up in the `entityId` field in outcoing authnRequests.

The `entityId` should typically be the URL where the metadata is presented. E.g.
`http://sp.example.com/AuthServices/`.

####`returnUri` Attribute
*Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element*

The Uri that you want users to be redirected to once the authentication is
complete. This is typically the start page of the application, or a special
signed in start page.

###`<identityProvider>` Element
*Child element of the [`<kentor.authServices>`](#kentor-authservices-section) element*

An identity provider that the Service Provider relies on for authentication.

####Attributes
* [`entityID`](#entityId-attribute-identityprovider)
* [`destinationUri`](#destinationuri-attribute)
* [`allowUnsolicitedAuthnResponse`](#allowunsolicitedauthnresponse-attribute)
* [`binding`](#binding-attribute)

####Elements
* [`<signingCertificate>`](#signingcertificate-element)

####`entityID` Attribute (identityProvider)
*Attribute of the [`<identityProvider>`](#identityprovider-element) element*

The issuer name that the idp will be using when sending responses.

####`destinationUri` Attribute
*Attribute of the [`<identityProvider>`](#identityprovider-element) element*

The uri where the identity provider listens for incoming requests. The 
uri has to be written in a way that the client understands, since it is
the client web browser that will be redirected to the uri. Specifically
this means that using a host name only uri or a host name that only resolves
on the network of the server won't work.

####`allowUnsolicitedAuthnResponse` Attribute
*Attribute of the [`<identityProvider>`](#identityprovider-element) element*

The allowUnsolicitedAuthnResponse attribtue is required.

Allow unsolicited responses. That is InResponseTo is missing in the AuthnRequest.  
If true InResponseTo is not required. The IDP can initiate the authentication process.  
If false InResponseTo is required. The authentication process must be initiated by an AuthnRequest from this SP.  
Even though allowUnsolicitedAuthnResponse is true the InResponseTo must be valid if existing.

####`binding` Attribute
*Attribute of the [`<identityProvider>`](#identityprovider-element) element*

The binding that the services provider should use when sending requests
to the identity provider. One of the supported values of the `Saml2BindingType`
enum.

Currently supported values:

* `HttpRedirect`

###`<signingCertificate>` Element
*Child element of the [`<identityProvider>`](#identityprovider-element) element*

The certificate that the identity provider uses to sign it's messages. The 
certificate can either be loaded from file if the `fileName` attribute is
specified or from a certificate store if the other attributes are specified.
If a `fileName` is specified that will take precedence and the other attributes
will be ignored.

###Attributes
* [`fileName`](#filename-attribute)
* [`storeName`](#storename-attribute)
* [`storeLocation`](#storelocation-attribute)
* [`findValue`](#findvalue-attribute)
* [`x509FindType`](#x509findtype-attribute)

####`fileName` Attribute
*Attribute of the [`<signingCertificate>`](#signingcertificate-element) element*

A file name to load the certificate from. The path is relative to the execution
path of the application.

File based certificates are only recommended for testing and during 
development. In production environments it is better to use the certificate
storage.

####`storeName` Attribute
*Attribute of the [`<signingCertificate>`](#signingcertificate-element) element*

Name of the certificate store to search for the certificate. It is recommended
to keep the certificate of the identity provider in the "Other People" store
which is specified by the `AddressBook` enum value.

Valid values are those from the 
[`System.Security.Cryptography.X509Certificates.StoreName`](http://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storename.aspx)
enumeration.

####`storeLocation` Attribute
*Attribute of the [`<signingCertificate>`](#signingcertificate-element) element*

The location of the store to search for the certificate. On production services
it is recommended to use the `LocalMachine` value, while it makes more sense
to use `CurrentUser` in development setups.

Valid values are those from the
[`System.Security.Cryptography.X509Certificates.StoreLocation`](http://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storelocation.aspx)
enumeration.

####`findValue` Attribute
*Attribute of the [`<signingCertificate>`](#signingcertificate-element) element*

A search term to use to find the certificate. The value will be searched for in
the field specified by the [`x509FindType`](#x509findtype-attribute) attribute.

####`x509FindType` Attribute
*Attribute of the [`<signingCertificate>`](#signingcertificate-element) element*

The field that will be seach for a match to the value in 
[`findValue`](#findvalue-attribute). For security, it is recommended to use 
`FindBySerialNumber`.

*Note: There is a nasty bug when copying a serial number from the certificate info 
displayed by certificate manager and the browser. There is a hidden character 
before the first hex digit that will mess upp the matching. Once pasted into
the config, use the arrow keys to make sure that there is not an additional
invisible character at the start of the serial number string.*

Valid values are those from the
[`System.Security.Cryptography.X509Certificates.X509FindType`](http://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509findtype.aspx)
enumeration.

##`<system.identityModel>` Section
*Child element of `<configuration>` element.*

There must be a [`<system.identityModel>`](http://msdn.microsoft.com/en-us/library/hh568638.aspx)
section in the config file or there will be a runtime error. The section can 
be empty (use `<system.identityModel />`).

```
<system.identityModel>
  <identityConfiguration>
    <claimsAuthenticationManager type="Kentor.AuthServices.Tests.ClaimsAuthenticationManagerStub, Kentor.AuthServices.Tests"/>
  </identityConfiguration>
</system.identityModel>
```

###`<claimsAuthenticationManager>` Element
*Child element of the `<identityConfiguration>` element.*

Specifies the type of a custom [`ClaimsAuthenticationManager`](ClaimsAuthenticationManager.md) for the 
application. The default implementation just passes through the identity.


###`<system.IdentityModelServices>` Section
*Child element of `<configuration>` element.*

The [`<system.identityModel.services>`](http://msdn.microsoft.com/en-us/library/hh568674.aspx)
element configures the built in servies. For testing on non ssl sites, the
requirement for ssl for the session authentication cookie must be disabled.

***Note: It is a severe security risk to leave the `requireSsl` setting as 
`false` in a production environment.***

```
<system.identityModel.services>
  <federationConfiguration>
    <cookieHandler requireSsl ="false"/>
  </federationConfiguration>
</system.identityModel.services>
```