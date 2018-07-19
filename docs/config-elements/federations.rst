``<federations>`` Element
=========================
This is an **optional** child element of the :doc:`sustainsys.saml2 <sustainsys-saml2>` element.

This element contains a list of federations that the service provider knows and trusts.

As with some other elements, individual items are added via an ``<add>`` element inside this element,
so you'll end up with XML that looks like the following:

.. code-block:: xml

    <federations>
        <add metadataLocation="" allowUnsolicitedAuthnResponse="" />
        <add metadataLocation="" allowUnsolicitedAuthnResponse="" />
        ...
    </federations>

Attributes
----------
``metadataLocation``
    URL to the full metadata of the federation. Saml2 will download the metadata and add all identity 
    providers found to the list of known and trusted identity providers. The location can be a URL, an 
    absolute path to a local file or an app relative path (e.g. ``~/App_Data/IdpMetadata.xml``)

``allowUnsolicitedAuthnResponse``
    ``true`` or ``false`` value indicating whether unsolicited authn responses should be allowed from the 
    identity providers in the federation.