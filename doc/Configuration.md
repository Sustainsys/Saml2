kentor.AuthServices Configuration
=============
To use Kentor.AuthServices in an application it must be enabled in the 
application's `web.config`. The sample applications contains complete
working [`web.config`](../SampleApplication/Web.config) examples. For ASP.NET MVC applications see [`this working web.config`](../SampleMvcApplication/Web.config) example.

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
    <!-- Only add the Saml2AuthenticationModule if you're using the Kentor.AuthServices.HttpModule
		library. If you are using Kentor.AuthServices.Mvc you SHOULD NOT load this module.-->
	<add name="Saml2AuthenticationModule" type="Kentor.AuthServices.HttpModule.Saml2AuthenticationModule, Kentor.AuthServices.HttpModule"/>
  </httpModules>
</system.web>
```

##kentor.authServices Section
The saml2AuthenticationModule section contains the configuration of the Kentor.AuthServices
library. It is required for the http module, the mvc controller and the Owin middleware.

```
<kentor.authServices entityId="http://localhost:17009"
                     returnUrl="http://localhost:17009/SamplePath/"
                     discoveryServiceUrl="http://localhost:52071/DiscoveryService" >
  <metadata cacheDuration="0:15:00" >
    <organization name="Kentor IT AB" displayName="Kentor" url="http://www.kentor.se" language="sv" />
    <contactPerson type="Other" email="info@kentor.se" />
    <requestedAttributes>
      <add friendlyName ="Some Name" name="urn:someName" nameFormat="urn:oasis:names:tc:SAML:2.0:attrname-format:uri" isRequired="true" />
      <add name="Minimal" />
    </requestedAttributes>
  </metadata>
  <identityProviders>
    <add entityId="https://stubidp.kentor.se/Metadata" 
         destinationUrl="https://stubidp.kentor.se" 
         allowUnsolicitedAuthnResponse="true" binding="HttpRedirect">
      <signingCertificate storeName="AddressBook" storeLocation="CurrentUser" 
                          findValue="Kentor.AuthServices.StubIdp" x509FindType="FindBySubjectName" />
    </add>
    <add entityId="example-idp"
         metadataUrl="https://idp.example.com/Metadata"
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
* [`returnUrl`](#returnUrl-attribute)
* [`entityId`](#entityid-attribute)
* [`discoveryServiceUrl`](#discoveryserviceurl-attribute)
* [`modulePath`](#modulepath-attribute)

####Elements
* [`<metadata>`](#metadata-element)
* [`<identityProviders>`](#identityproviders-element)
* [`<federations>`](#federations-element)
* [`<serviceCertificate>`](#serviceCertificate-element)

####`entityId` Attribute
*Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

The name that this service provider will use for itself when sending
messages. The name will end up in the `Issuer` field in outcoing authnRequests.

The SAML standard requires the `entityId` to be an absolut URI. Typically it should
be the URL where the metadata is presented. E.g.
`http://sp.example.com/AuthServices/`.

####`returnUrl` Attribute
*Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

The Url that you want users to be redirected to once the authentication is
complete. This is typically the start page of the application, or a special
signed in start page.

####`discoveryServiceUrl` Attribute
*Optional Attribute of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

Optional attribute that specifies an idp discovery service to use if no idp
is specified when calling sign in. Without this attribute, the first idp known
will be used if none is specified.

####`modulePath` Attribute
*Optional Attribute of the [`<kentor.authServices>`](#modulePath-attribute) element.*

Optional attribute that indicates the base path of the AuthServices endpoints.
Defaults to `/AuthServices` if not specified. This can usually be left as the
default, but if several instances of AuthServices are loaded into the
same process they must each get a separate base path.

###`<metadata>` Element
*Optional child element of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

####Attributes
* [`cacheDuration`](#cacheduration-attribute)

####Elements
* [`<organization>`](#organization-element)
* [`<contactPerson>`](#contactperson-element)
* [`<requestedAttributes>`](#requestedattributes-element)

####`cacheDuration` Attribute
*Optional Attribute of the [`<metadata>`](#metadata-element) element.*

Optional attribute that describes for how long in anyone may cache the metadata 
presented by the service provider. Defaults to one hour. Examples of valid format strings:

* 1 day, 2 hours: `1.2:00:00`.
* 42 seconds: `0:00:42`.

###[`organization`] Element
*Optional child element of the [`<metadata>`](#metadata-element) element.*

Provides information about the organization supplying the SAML2 entity (in plain
English that means the organization that supplies the application that AuthServices
is used in).

####Attributes
* [`name`](#name-attribute-organization)
* [`displayName`](#displayname-attribute)
* [`url`](#url-attribute)
* [`langauge`](#language-attribute)

####`name` Attribute (organization)
*Attribute of the [`<organization>`](#organization-element) element.*

The name of the organization.

####`displayName` Attribute
*Attribute of the [`<organization>`](#organization-element) element.*

The display name of the organization.

####`url` Attribute
*Attribute of the [`<organization>`](#organization-element) element.*

Url to the organization's web site.

####`language` Attribute
*Optional Attribute of the [`<organization>`](#organization-element) element.*

In the generated metadata, the `name`, `displayName` and `url` attributes have
a language specification. If none is specified, the `xml:lang` attribute will
be generated with an empty value.

###`<contactPerson>` Element
*Optional child element of the [`<metadata>`](#metadata-element) element. Can
be repeated multiple times.*

####Attributes
* [`type`](#type-attribute)
* [`company`](#company-attribute)
* [`givenName`](#givenname-attribute)
* [`surname`](#surname-attribute)
* [`phoneNumber`](#phonenumber-attribute)
* [`email`](#email-attribute)

####`type` Attribute
*Attribute of the [`<contactPerson>`](#contactperson-element) element.*

The type attribute indicates the type of the contact and is picked from the
[`ContactType`](http://msdn.microsoft.com/en-us/library/system.identitymodel.metadata.contacttype(v=vs.110).aspx) 
enum. Valid values are:

* Administrative
* Billing
* Other
* Support
* Technical

####`company` Attribute
*Optional attribute of the [`<contactPerson>`](#contactperson-element) element.*

Name of the person's company.

####`givenName` Attribute
*Optional attribute of the [`<contactPerson>`](#contactperson-element) element.*

Given name of the person.

####`surname` Attribute
*Optional attribute of the [`<contactPerson>`](#contactperson-element) element.*

Surname of the person.

####`phoneNumber` Attribute
*Optional attribute of the [`<contactPerson>`](#contactperson-element) element.*

Phone number of the person. The SAML standard allows multiple phone number to
be specified. AuthServices supports that, but not through the configuration file.

####`email` Attribute
*Optional attribute of the [`<contactPerson>`](#contactperson-element) element.*

Email address of the person. The SAML standard allows multiple email addresses to
be specified. AuthServices supports that, but not through the configuration file.

###`<requestedAttributes>` Element
*Optional child element of the [`<metadata>`](#metadata-element) element.*

List of attributes that the SP requests to be included in the assertions 
generated by an identity provider.

Each attribute is added to the list with an `<add>` element.

####Attributes of the `<add>` element for `<requestedAttributes>`
* [`name`](#name-attribute-requestedattribute)
* [`friendlyName`](#friendlyname-attribute)
* [`nameFormat`](#nameformat-attribute)
* [`isRequired`](#isrequired-attribute)

####`name` Attribute (RequestedAttribute)
*Attribute of `<add>` for []`<requestedAttributes>`](requestedattributes-element)*

The name of the attribute. This is usually in the form of an urn/oid, e.g. 
urn:oid:1.2.3. The format of hte name should be specified in the 
[`nameFormat`](#nameformat-attribute) attribute.

####`friendlyName` Attribute
*Optional Attribute of `<add>` for []`<requestedAttributes>`](requestedattributes-element)*

An optional friendly (i.e. human readable) friendly name of the attribute that
will be included in the metadata. Please note that the SAML2 standard specifically
forbids the `friendlyName` to be used for anything else an information to a human.
All matching of attributes must use the `name`.

####`nameFormat` Attribute
*Optional Attribute of `<add>` for []`<requestedAttributes>`](requestedattributes-element)*

Format of the name attribute. Valid values are:

* urn:oasis:names:tc:SAML:2.0:attrname-format:uri
* urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified
* urn:oasis:names:tc:SAML:2.0:attrname-format:basic

####`isRequired` Attribute
*Optional Attribute of `<add>` for []`<requestedAttributes>`](requestedattributes-element)*

Is this attribute required by the service provider or is it just a request that
it would be nice if the Idp includes it?

###`<identityProviders>` Element
*Optional child element of the [`<kentor.authServices>`](#kentor-authservices-section) element.*

A list of identity providers known to the service provider.

####Elements
* [`<add>`](#add-identityprovider-element)

###`<add>` IdentityProvider Element
*Child element of the [`<identityProviders`](#identityproviders-element) element.*

####Attributes
* [`entityID`](#entityId-attribute-identityprovider)
* [`destinationUrl`](#destinationuri-attribute)
* [`allowUnsolicitedAuthnResponse`](#allowunsolicitedauthnresponse-attribute)
* [`binding`](#binding-attribute)
* [`loadMetadata`](#loadmetadata-attribute)
* [`metadataUrl`](#metadataurl-attribute-idp)

####Elements
* [`<signingCertificate>`](#signingcertificate-element)

####`entityId` Attribute (identityProvider)
*Attribute of the [`<add>`](#add-identityprovider-element) element*

The issuer name that the idp will be using when sending responses. When `<loadMetadata>`
is enabled, the `entityId` is treated as a URL to for downloading the metadata.

####`destinationUrl` Attribute
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

####`metadataUrl` Attribute (Idp)
*Optional attribute of the [`add`](#add-identityprovider-element) element*

The SAML2 metadata standard strongly suggests that the Entity Id of a SAML2 entity
is a URL where the metadata of the entity can be found. When loading metadata
for an idp, AuthServices normally interprets the EntityId as a url to the metadata.
If the metadata is located somewhere else it can be specified with this
configuration parameter.

###`<signingCertificate>` Element
*Optional child element of the [`<identityProvider>`](#identityprovider-element) element*

The certificate that the identity provider uses to sign its messages. The 
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

###`<serviceCertificate>` Element
*Optional child element of the `<kentor.authServices>`(#kentor-authservices-section) element.*

Specifies the certificate that the service provider uses for encrypted assertions. 
The public key of this certificate will be exposed in the metadata and the private
key will be used during decryption. 

Uses same options/attributes as [`<signingCertificate>`](#signingcertificate-element) for locating the certificate.

####Elements
* [`<add>`](#add-federation-element).

###`<add>` Federation Element
* Child element of the `<federations>`(#federations-element) element.*

Adds a known federation.

####Attributes
* [`metadataUrl`](#metadataUrl-attribute-federation).
* [`allowUnsolicitedAuthnResponse`](#allowunsolicitedauthnresponse-attribute-federation)

####`metadataUrl` Attribute (Federation)
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
