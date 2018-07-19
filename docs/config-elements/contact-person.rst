``<contactPerson>`` Element
===========================
Optional child element of the :doc:`metadata`  element.

Attributes
----------
``type`` 
    The type attribute indicates the type of the contact and is picked from the ``ContactType`` enum. Valid values are:
    
    * ``Administrative``
    * ``Billing``
    * ``Other``
    * ``Support``
    * ``Technical``

``company`` (Optional)
    Name of the person's company.

``givenName`` (Optional)
    Given name of the contact person.

``surname`` (Optional)
    Surname of the contact person.

``phoneNumber`` (Optional)
    Phone number of the contact person. The SAML standard allows multiple phone number to be specified. Saml2 supports 
    that, but not through the configuration file.

``email`` (Optional)
    Email address of the person. The SAML standard allows multiple email addresses to be specified. Saml2 
    supports that, but not through the configuration file.