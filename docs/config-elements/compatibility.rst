``<compatibility>`` Element
===========================
This is an **optional** child element of the :doc:`sustainsys.saml2 <sustainsys-saml2>` element.

Enables overrides of default behaviour to increase compatibility with identity providers.

Attributes
----------
``unpackEntitiesDescriptorInIdentityProviderMetadata`` (Optional)
    If an ``EntitiesDescriptor`` element is found when loading metadata for an IdentityProvider, automatically 
    check inside it if there is a single ``EntityDescriptor`` and in that case use it.

``IgnoreAuthenticationContextInResponse``
    Do not read the ``AuthnContext`` element in Saml2Response. If you do not need these values to be present as 
    claims in the generated identity, using this option can prevent XML format 
    errors (``System.Xml.XmlException: ID0013: The value must be an absolute URI``), when value cannot parse 
    as absolute URI.