``<organization>`` Element
==========================
Optional child element of the :doc:`metadata`  element.

Provides information about the organization supplying the SAML2 entity (in plain English that means the organization 
that supplies the application that Saml2 is used in).

Attributes
----------
``name``
    The name of the organization.

``displayName``
    The display name of the organization.

``url``
    URL to the organization's web site.

``language``
    In the generated metadata, the ``name``, ``displayName`` and ``url`` attributes have a language specification. If none 
    is specified, the ``xml:lang`` attribute will be generated with an empty value.