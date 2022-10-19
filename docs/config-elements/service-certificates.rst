``<serviceCertificates>`` Element
=================================
This is an **optional** child element of the :doc:`sustainsys.saml2 <sustainsys-saml2>` element.

Specifies the certificate(s) that the service provider uses for encrypted assertions (and for signed requests, once 
that feature is added). If neither of those features are used, this element can be ommitted.

The public key(s) will be exposed in the metadata and the private key(s) will be used during decryption/signing.

Individual certificates are added via an ``<add>`` element, so the resulting XML will be similar to the following:

.. code-block:: xml

    <serviceCertificates>
        <add use="" status="" metadataPublishOverride="" />
        <add use="" status="" metadataPublishOverride="" />
        ...
    </serviceCertificates>

Attributes
----------
``use``
    Indicates how the certificate will be used.  Options are:

    * ``Signing``
    * ``Encryption``
    * ``Both`` (default)

``status``
    Indicates whether the certificate is a current or future certificate -- used in key rollover scenarios.  Options are:

    * ``Current`` (default)
    * ``Future``

``metadataPublishOverride``
    By default the certificate will be used and published by the rules shown in the table below.  To 
    override this behavior choose one of the following options for this attribute:

    * ``None`` (Default) - published according to the rules in the table below.
    * ``PublishUnspecified``
    * ``PublishEncryption``
    * ``PublishSigning``
    * ``DoNotPublish``

    .. list-table:: 
        :widths: 10 10 50 30
        :header-rows: 1
        :class: tight-table

        * - Use
          - Status
          - Published in Metadata
          - Used by Saml2
        * - Both
          - Current
          - Unspecified *unless Future key exists*, then Signing 
          - Yes
        * - Both
          - Future
          - Unspecified
          - For decryption only
        * - Signing
          - Current
          - Signing
          - Yes 
        * - Signing
          - Future
          - Signing
          - No
        * - Encryption
          - Current
          - Encryption *unless Future key exists* then not published
          - Yes
        * - Encryption
          - Future
          - Encryption
          - Yes
          
``fileName``
    A file name to load the certificate from. The path is relative to the execution path of the application.  Make sure
    to heed the warning above -- *best to use store-based certificates for non-development environments.*

``storeName``
    Name of the certificate store to search for the certificate. It is recommended to keep the certificate 
    of the identity provider in the "Other People" store which is specified by the ``AddressBook`` enum value.
    Valid values are those from the ``System.Security.Cryptography.X509Certificates.StoreName`` enumeration.

``storeLocation``
    The location of the store to search for the certificate. On production services it is recommended 
    to use the LocalMachine value, while it makes more sense to use CurrentUser in development setups.
    Valid values are those from the ``System.Security.Cryptography.X509Certificates.StoreLocation`` enumeration.

``findValue``
    A search term to use to find the certificate. The value will be searched for in the field specified by 
    the ``x509FindType`` attribute.

``x509FindType``
    The field that will be seach for a match to the value in findValue. For security, it is recommended to 
    use ``FindBySerialNumber``.

    Valid values are those from the ``System.Security.Cryptography.X509Certificates.X509FindType`` enumeration.
