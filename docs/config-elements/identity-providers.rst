``<identityProviders>`` Element
===============================
This is an **optional** child element of the :doc:`sustainsys.saml2 <sustainsys-saml2>` element.

It indicates a list of identity providers known to the service provider.

Each identity provider is added as an ``<add>`` element to the ``<identityProviders>`` element and 
the element will end up looking something like what is shown below.  Note the possible child element 
of the ``<signingCertifcate>`` which is shown in the second added identity provider element below.

.. code-block:: xml

    <identityProviders>
        <add entityId="" signOnUrl="" logoutUrl="" allowUnsolicitedAuthnResponse="" binding="" 
            wantAuthnRequestsSigned="" loadMetadata="" metadataLocation="" disableOutboundLogoutRequests="" outboundSigningAlgorithm=""/>
        <add entityId="" signOnUrl="" logoutUrl="" allowUnsolicitedAuthnResponse="" binding="" 
            wantAuthnRequestsSigned="" loadMetadata="" metadataLocation="" disableOutboundLogoutRequests="" outboundSigningAlgorithm="">
            <signingCertificate storeName="" storeLocation="" findValue="" x509FindType="" />
        </add>
        ...
    </identityProviders>

Attributes
----------
``entityId``
    The issuer name that the idp will be using when sending responses. When ``<loadMetadata>`` is enabled, the entityId 
    is treated as a URL to for downloading the metadata.

``signOnUrl`` (Optional)
    The url where the identity provider listens for incoming sign on requests. The url has to be written in a 
    way that the client understands, since it is the client web browser that will be redirected to the url. Specifically, 
    this means that using a host name only url or a host name that only resolves on the network of the server won't work.

``logoutUrl`` (Optional)
    The url where the identity provider listens for incoming logout requests and responses. To enable single logout 
    behaviour there must also be a service certificate configured in Saml2 as all logout messages must be signed.

``allowUnsolicitedAuthnResponse``
    Allow unsolicited responses. That is, Idp initiated sign on where there was no prior AuthnRequest. If true 
    ``InResponseTo`` is not required and the IDP can initiate the authentication process. If false ``InResponseTo`` is 
    required and the authentication process must be initiated by an AuthnRequest from this SP. Note that if the 
    authentication was SP-intiatied, ``RelayState`` and ``InResponseTo`` must be present and valid.

``binding`` (Optional)
    The binding that the services provider should use when sending requests to the identity provider. One of the supported 
    values of the ``Saml2BindingType`` enum.

    * ``HttpRedirect``
    * ``HttpPost``
    * ``Artifact``

``wantAuthnRequestsSigned`` (Optional)
    Specifies whether the Identity provider wants the AuthnRequests signed. Defaults to ``false``.

``loadMetadata`` (Optional)
    Load metadata from the idp and use that information instead of the configuration. It is possible to use a 
    specific certificate even though the metadata is loaded, in that case the configured certificate will take 
    precedence over any contents in the metadata.

``metadataLocation`` (Optional)
    The SAML2 metadata standard strongly suggests that the ``Entity Id`` of a SAML2 entity is a URL where the 
    metadata of the entity can be found. When loading metadata for an idp, Saml2 normally interprets the 
    EntityId as a url to the metadata. If the metadata is located somewhere else it can be specified with this 
    configuration parameter. The location can be a URL, an absolute path to a local file or an app relative 
    path (e.g. ``~/App_Data/IdpMetadata.xml``)

``disableOutboundLogoutRequests`` (Optional)
    Disable outbound logout requests to this idp, even though Saml2 is configured for single logout and the idp 
    supports it. This setting might be usable when adding SLO to an existing setup, to ensure that everyone is 
    ready for SLO before activating.

``outboundSigningAlgorithm`` (Optional)
    By default Saml2 uses SHA256 signatures if running on .NET 4.6.2 or later and otherwise SHA1 signatures. Set this 
    to set the signing algorithm for any outbound messages for this identity provider. Possible values:

    * ``SHA1``
    * ``SHA256``
    * ``SHA384``
    * ``SHA512``

Elements
--------
The following are the possible children elements of the ``<identityProviders>`` element.  Each are provided as a 
link below with full explanations of each. 

* :doc:`signingCertificate <signing-certificate>`