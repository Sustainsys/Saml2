``<requestedAuthnContext>`` Element
===================================
This is an **optional** child element of the :doc:`sustainsys.saml2 <sustainsys-saml2>` element.

Attributes
----------
``classRef`` (Optional)
    Class reference for authentication context. Either specify a full URI to identify an authentication context 
    class, or a single word if using one of the predefined classes in the SAML2 Authentication context specification:

    * ``InternetProtocol``
    * ``InternetProtocolPassword``
    * ``Kerberos``
    * ``MobileOneFactorUnregistered``
    * ``MobileTwoFactorUnregistered``
    * ``MobileOneFactorContract``
    * ``MobileTwoFactorContract``
    * ``Password``
    * ``PasswordProtectedTransport``
    * ``PreviousSession``
    * ``X509``
    * ``PGP``
    * ``SPKI``
    * ``XMLDSig``
    * ``Smartcard``
    * ``SmartcardPKI``
    * ``SoftwarePKI``
    * ``Telephony``
    * ``NomadTelephony``
    * ``PersonalTelephony``
    * ``AuthenticatedTelephony``
    * ``SecureRemotePassword``
    * ``TLSClient``
    * ``TimeSyncToken``
    * ``unspecified``

``comparison`` (Optional)
    Comparison method for authentication context as signalled in AuthnRequests.
    Valid values are:

    * ``Exact`` (default)
    * ``Minimum``
    * ``Maximum``
    * ``Better``

    ``Minimum`` is an inclusive comparison, meaning the specified ``classRef`` or anything better is 
    accepted. ``Better`` is exclusive, meaning that the specified ``classRef`` is not accepted.