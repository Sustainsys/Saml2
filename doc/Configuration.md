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
  <identityProvider issuer ="https://idp.example.com" destinationUri="httpss://idp.example.com" binding="HttpRedirect">
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
* [`issuer`](#issuer-attribute)

####Elements
* [`<identityProvider>`](#identityprovider-element)

####`assertionConsumerServiceUrl` Attribute
*Attribute of [`<kentor.authServices>`](kentor-authservices-section) element*

The assertionConsumerServiceUrl is the Url to which the Idp will post the 
Saml2 ticket. It should be the base Url of your application concatenated with 
`/Saml2AuthenticationModule/acs`. The relative Url is hard coded and cannot 
be changed.

####`issuer` Attribute
*Attribute of [`<kentor.authServices>`](kentor-authservices-section) element*

The name that this service provider will use for itself when sending
messages. The name will end up in the `issuer` field in outcoing authnRequests.

####`returnUri` Attribute
*Attribute of [`<kentor.authServices>`](kentor-authservices-section) element*

The Uri that you want users to be redirected to once the authentication is
complete. This is typically the start page of the application, or a special
signed in start page.

###`<identityProvider>` Element
*Child element of [`<identityProviders>`](#identityproviders-element) element*

An identity provider that the Service Provider relies on for authentication.

####Attributes
* [`issuer`](#issuer-attribute-identityProvider)
* [`destinationUri`](#destinationuri-attribute)
* [`binding`](#binding-attribute)

####Elements
* [`<signingCertificate>`](#signingcertificate-element)

####`issuer` Attribute (identityProvider)
*Attribute of [`<identityProvider>`](#identityprovider-element) element*

The issuer name that the idp will be using when sending responses.

####`destinationUri` Attribute
*Attribute of [`<identityProvider>`](#identityprovider-element) element*

The uri where the identity provider listens for incoming requests. The 
uri has to be written in a way that the client understands, since it is
the client web browser that will be redirected to the uri. Specifically
this means that using a host name only uri or a host name that only resolves
on the network of the server won't work.

####`binding` Attribute
*Attribute of [`<identityProvider>`](#identityprovider-element) element*

The binding that the services provider should use when sending requests
to the identity provider. One of the supported values of the `Saml2BindingType`
enum.

Currently supported values:

* `HttpRedirect`

###`<signingCertificate>` Element
*Child element of [`<identityProvider>`](#identityprovider-element) element*

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
*Attribute of [`<signingCertificate>`] element*

A file name to load the certificate from. The path is relative to the execution
path of the application.

File based certificates are only recommended for testing and during 
development. In production environments it is better to use the certificate
storage.

####`storeName` Attribute
*Attribute of [`<signingCertificate>`] element*

Name of the certificate store to search for the certificate. It is recommended
to keep the certificate of the identity provider in the "Other People" store
which is specified by the `AddressBook` enum value.

Valid values are those from the 
[`System.Security.Cryptography.X509Certificates.StoreName`](http://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storename.aspx)
enumeration.

####`storeLocation` Attribute
*Attribute of [`<signingCertificate>`] element*

The location of the store to search for the certificate. On production services
it is recommended to use the `LocalMachine` value, while it makes more sense
to use `CurrentUser` in development setups.

Valid values are those from the
[`System.Security.Cryptography.X509Certificates.StoreLocation`](http://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.storelocation.aspx)
enumeration.

####`findValue` Attribute
*Attribute of [`<signingCertificate>`] element*

A search term to use to find the certificate. The value will be searched for in
the field specified by the [`x509FindType`](#x509findtype-attribute) attribute.

####`x509FindType` Attribute
*Attribute of [`<signingCertificate>`] element*

The field that will be seach for a match to the value in 
[`findValue`](#findvalue-attribute). For security, it is recommended to use 
the `FindBySerialNumber` entry.

*Note: There is a nasty bug when copying a serial number from the certificate info 
displayed by certificate manager and the browser. There is a hidden character 
before the first hex digit that will mess upp the matching. Once pasted into
the config, use the arrow keys to make sure that there is not an additional
invisible character at the start of the serial number string.*

Valid values are those from the
[`System.Security.Cryptography.X509Certificates.X509FindType`](http://msdn.microsoft.com/en-us/library/system.security.cryptography.x509certificates.x509findtype.aspx)
enumeration.