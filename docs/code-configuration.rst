Configuration in code
=====================
The configuration is split in two different classes.

* The ``SPOptions`` class affects the behaviour of our application as a Saml2 Service Provider.
* The ``IdentityProvider`` class models an upstream Saml2 Identity Provider. One or more
  identity providers can be added.

``SPOptions``
-------------
This section only lists the most important properties of ``SPOptions``. All options are documented
using XML comments which gives intellisense information in most development environments.

``EntityId``
^^^^^^^^^^^^
An absolute URI that identifies our application as a service provider. This is not the
Entity Id of the Identity Provider. If multiple Saml2 schemes are registered, it is strongly
recommended that each one have their own unique Entity Id.

``ModulePath``
^^^^^^^^^^^^^^
The base path for the Saml2 endpoints. If multiple Saml2 schemes are registered, each one
must have a unique ModulePath.

``ServiceCertificates``
^^^^^^^^^^^^^^^^^^^^^^^
Certificate to use to sign outbound Saml2 messages and optionally to decrypt incoming
messages if encryption is enabled.

.. note::

    Single logout messages must be signed. To enable the single logout endpoints of the library
    there must be a configured service certificate.

``IdentityProvider``
--------------------
The ``IdentityProvider`` class represents an upstream Saml2 Identity Provider. This section
only lists the most important properties. All options are documented using XML comments
which gives intellisense information in most development environments.

``EntityId``
^^^^^^^^^^^^^
An absolute URI that identifiers the Identity Provider.

``LoadMetadata``
^^^^^^^^^^^^^^^^
Enable loding of Metadata to get the Identity Provider's configuration. By convention the
``EntityId`` is the address of the metadata.

``MetadataLocation``
^^^^^^^^^^^^^^^^^^^^
Set an explicit location for metadata. It can be a remote URL or a local file path. Setting
the ``MetadataLocation`` automatically enables ``LoadMetadata``