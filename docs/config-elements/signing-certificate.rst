``<signingCertificate>`` Element
================================
Optional element of the :doc:`identityProvider <identity-providers>` element.

The certificate that the identity provider uses to sign its messages. The certificate can either be loaded from 
file if the ``fileName`` attribute is specified or from a certificate store if the other 
attributes are specified. If a ``fileName`` is specified that will take precedence and the other attributes will be ignored.

.. warning::
    File-based  certificates are only recommended for testing and during 
    development. In production environments it is better to use the certificate store.

Attributes
----------
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

    .. warning::
        There is a nasty bug when copying a serial number from the certificate info displayed by 
        certificate manager and the browser. There is a hidden character before the first hex digit that 
        will mess up the matching. Once pasted into the config, use the arrow keys to make sure that 
        there is not an additional invisible character at the start of the serial number string.
