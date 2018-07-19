``<nameIdPolicy>`` Element
==========================
This is an **optional** child element of the :doc:`sustainsys.saml2 <sustainsys-saml2>` element.

This element controls the generation of ``NameIDPolicy`` element in AuthnRequests. The element 
is only created if either ``allowCreate`` or ``format`` are set to a non-default value.

Attributes
----------
``allowCreate`` (Optional)
    Default value is empty, which means that the attribute is not included in generated AuthnRequests.
    Supported values are ``true`` or ``false``.

``format`` (Optional)
    Sets the requested format of ``NameIDPolicy`` for generated authnRequests.
    
    Supported values (see section 8.3 in the `SAML2 Core specification <http://docs.oasis-open.org/security/saml/v2.0/saml-core-2.0-os.pdf>`_ for explanations of the values).

    * ``Unspecified``
    * ``EmailAddress``
    * ``X509SubjectName``
    * ``WindowsDomainQualifiedName``
    * ``KerberosPrincipalName``
    * ``EntityIdentifier``
    * ``Persistent``
    * ``Transient``

    If no value is specified, no format is specified in the generated AuthnRequests. If ``Transient`` is specified, it 
    is not permitted to specify ``allowCreate`` (see 3.4.1.1 in the `SAML2 Core spec <http://docs.oasis-open.org/security/saml/v2.0/saml-core-2.0-os.pdf>`_).
