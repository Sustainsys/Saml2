kentor.AuthServices Configuration
=============
To use Kentor.AuthServices in an application it must be enabled in the 
application's `web.config`. The sample applications contains complete
working [`web.config`](../SampleApplication/Web.config) examples.

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
##Loading modules
When using the http module and the MVC controller, the `SessionAuthenticationModule` needs
to be loaded and if using the http module that needs to be loaded as well. The owin package
does not need any http modules, please see the separate info on the [Owin middleware](OwinMiddleware.md).

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
The saml2AuthenticationModule section contains the configuration of the Kentor.AuthServices
library. It is required for the http module, the mvc controller and the Owin middleware.

```
<kentor.authServices entityId="http://localhost:17009"
                     returnUri="http://localhost:17009/SamplePath/"
                     metadataCacheDuration="1:00:00">
  <identityProviders>
    <add entityId="https://stubidp.kentor.se/Metadata" 
         destinationUri="https://stubidp.kentor.se" 
         allowUnsolicitedAuthnResponse="true" binding="HttpRedirect">
      <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                          findValue="Kentor.AuthServices.StubIdp" x509FindType="FindBySubjectName" />
    </add>
    <add entityId="https://idp.example.com/Metadata" 
         allowUnsolicitedAuthnResponse="true" 
         loadMetadata = "true" />
  </identityProviders>
  <federations>
    <add metadataUrl="https://federation.example.com/metadata.xml" allowUnsolicitedAuthnResponse = "false" />
  </federations>
</kentor.authServices>
```

###`<kentor.authServices>` Element
*Child element of `<configuration>` element.*

Root element of the config section.

####Attributes
* [`returnUri`](#returnuri-attribute)
* [`entityId`](#entityid-attribute)
* [`metadataCacheDuration`](#metadatacacheduration-attribute)
* [`discoveryServiceUrl`](#discoveryserviceurl-attribute)

####Elements
* [`<identityProviders>`](#identityproviders-element)
* [`<federations>`](#federations-element)

####`entityId` Attribute
*Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

The name that this service provider will use for itself when sending
messages. The name will end up in the `Issuer` field in outcoing authnRequests.

The `entityId` should typically be the URL where the metadata is presented. E.g.
`http://sp.example.com/AuthServices/`.

####`returnUri` Attribute
*Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

The Uri that you want users to be redirected to once the authentication is
complete. This is typically the start page of the application, or a special
signed in start page.

####`metadataCacheDuration` Attribute
*Optional Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

Optional attribute that describes for how long in anyone may cache the metadata 
presented by the service provider. Defaults to one hour. Examples of valid format strings:

* 1 day, 2 hours: `1.2:00:00`.
* 42 seconds: `0:00:42`.

####`discoveryServiceUrl` Attribute
*Optional Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

Optional attribute that specifies an idp discovery service to use if no idp
is specified when calling sign in. Without this attribute, the first idp known
will be used if none is specified.

###`<identityProviders>` Element
*Optional child element of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

A list of identity providers known to the service provider.

####Elements
* [`<add>`](#add-identityprovider-element)

###`<add>` IdentityProvider Element
*Child element of the [`<identityProviders`](#identityproviders-element) element.*

####Attributes
* [`entityID`](#entityId-attribute-identityprovider)
* [`destinationUri`](#destinationuri-attribute)
* [`allowUnsolicitedAuthnResponse`](#allowunsolicitedauthnresponse-attribute)
* [`binding`](#binding-attribute)
* [`loadMetadata`](#loadmetadata-attribute)

####Elements
* [`<signingCertificate>`](#signingcertificate-element)

####`entityId` Attribute (identityProvider)
*Attribute of the [`<add>`](#add-identityprovider-element) element*

The issuer name that the idp will be using when sending responses. When `<loadMetadata>`
is enabled, the `entityId` is treated as a URL to for downloading the metadata.

####`destinationUri` Attribute
*Optional attribute of the [`<add>`](#add-identityprovider-element) element*

The uri where the identity provider listens for incoming requests. The 
uri has to be written in a way that the client understands, since it is
the client web browser that will be redirected to the uri. Specifically
this means that using a host name only uri or a host name that only resolves
on the network of the server won't work.

####`allowUnsolicitedAuthnResponse` Attribute
*Attribute of the [`<add>`](#add-identityprovider-element) element*

Allow unsolicited responses. That is InResponseTo is missing in the AuthnRequest.  
If true InResponseTo is not required. The IDP can initiate the authentication process.  
If false InResponseTo is required. The authentication process must be initiated by an AuthnRequest from this SP.  
Even though allowUnsolicitedAuthnResponse is true the InResponseTo must be valid if existing.

####`binding` Attribute
*Optional attribute of the [`<add>`](#add-identityprovider-element) element*

The binding that the services provider should use when sending requests
to the identity provider. One of the supported values of the `Saml2BindingType`
enum.

Currently supported values:

* `HttpRedirect`
* `HttpPost`

####`loadMetadata` Attribute
*Optional attribute of the [`<add>`](#add-identityprovider-element) element*

Load metadata from the idp and use that information instead of the configuration. It is
possible to use a specific certificate even though the metadata is loaded, in that case
the configured certificate will take precedence over any contents in the metadata.

###`<signingCertificate>` Element
*Optional child element of the [`<identityProvider>`](#identityprovider-element) element*

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

###`<federations>` Element
*Optional child element of the `<kentor.authServices>`(#kentor-authservices-section) element.*

Contains a list of federations that the service provider knows and trusts.

####Elements
* [`<add>`](#add-federation-element).

###`<add>` Federation Element
* Child element of the `<federations>`(#federations-element) element.*

Adds a known federation.

####Attributes
* [`metadataUrl`](#metadataUrl-attribute).
* [`allowUnsolicitedAuthnResponse`](#allowunsolicitedauthnresponse-attribute-federation)

####`metadataUrl` Attribute
*Attribute of [`<add>`](#add-federation-element)*

URL to the full metadata of the federation. AuthServices will download the metadata and
add all identity providers found to the list of known and trusted identity providers.

####`allowUnsolicitedAuthnResponse` Attribute (Federation)
*Attribute of [`<add>`](#add-federation-element)*

Decided whether unsolicited authn responses should be allowed from the identity providers
in the federation.

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