``<metadata>`` Element
======================
This is an **optional** child element of the :doc:`sustainsys.saml2 <sustainsys-saml2>` element.

The metadata part of the configuration can be used to tweak the generated metadata. These configuration options 
only affect how the metadata is generated, no other behavior of the code is changed.

Attributes
----------
``cacheDuration`` (Optional)
    Describes for how long in anyone should cache the metadata presented by the service provider before trying 
    to fetch a new copy. Defaults to one hour. 

    Examples of valid format strings:

    * 1 day, 2 hours: ``1.2:00:00``
    * 42 seconds: ``0:00:42``

``validDuration`` (Optional)
    Sets the maximum time that anyone may cache the generated metadata. If ``cacheDuration`` is specified, the 
    remote party should try to reload metadata after that time. If that refresh fails, ``validDuration`` determines 
    for how long the old metadata may be used before it must be discarded.

    In the metadata, the time is exposed as an absolute ``validUntil`` date and time. That absolute time is 
    calculated on metadata generation by adding the configured ``validDuration`` to the current time. 
    
    Examples of valid format strings:

    * 1 day, 2 hours: ``1.2:00:00``
    * 42 seconds: ``0:00:42``

``wantAssertionSigned`` (Optional)
    Signal to IDPs that we want the Assertions themselves signed and not only the SAML response. Saml2 supports 
    both, so for normal usage this shouldn't matter. If set to ``false`` the entire ``wantAssertionsSigned`` attribute 
    is dropped from the metadata as the default values is ``false``.

Elements
--------
The following are the possible children elements of the ``<metadata>`` element.  Each are provided as a 
link below with full explanations of each. 

* :doc:`organization <organization>`
* :doc:`contactPerson <contact-person>`
* :doc:`requestedAttributes <requested-attributes>`