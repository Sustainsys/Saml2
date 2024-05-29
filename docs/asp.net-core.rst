Asp.Net Core Handler
====================
The Sustainsys.Saml2.AspNetCore2 package is an Asp.Net Core Authentication handler for Saml2. The
Saml2 handler uses a cookie handler to establish and maintain a session once the authentication 
handshake is completed.

A minimal configuration is to add the Saml2 authentication scheme

.. code-block:: csharp

    builder.Services.AddAuthentication(opt =>
    {
        // Default scheme that maintains session is cookies.
        opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        // If there's a challenge to sign in, use the Saml2 scheme.
        opt.DefaultChallengeScheme = Saml2Defaults.Scheme;
    })
    .AddCookie()
    .AddSaml2(opt =>
    {
        // Set up our EntityId, this is our application.
        opt.SPOptions.EntityId = new EntityId("https://localhost:5001/Saml2");

        // Single logout messages should be signed according to the SAML2 standard, so we need
        // to add a certificate for our app to sign logout messages with to enable logout functionality.
        opt.SPOptions.ServiceCertificates.Add(new X509Certificate2("Sustainsys.Saml2.Tests.pfx"));

        // Add an identity provider.
        opt.IdentityProviders.Add(new IdentityProvider(
            // The identityprovider's entity id.
            new EntityId("https://stubidp.sustainsys.com/Metadata"),
            opt.SPOptions)
        {
            // Load config parameters from metadata, using the Entity Id as the metadata address.
            LoadMetadata = true
        });
    });

Configuration
-------------
The ``Saml2Options`` class contains the normal plumbing for the Asp.Net Core Authentication system. All of
the settings related to the upstream IdentityProvider(s) are located in the ``IdentityProviders`` class.
The settings that determine how our application works as a Saml2 Service Provider are in ``SPOptions``.
See :doc:`code based configuration <code-configuration>` for details.

Multiple Upstream Identity providers
------------------------------------
When working with multiple upstream Identity Providers two different approaches can be used:
1. A single Asp.Net Core Authentication Scheme that has multiple Identity Providers registered
2. One Asp.Net Core Authentication scheme per Identity Provider.

It is generally preferred to use the second option as that aligns better with how the rest of the Asp.Net Core
authentication system (including Asp.Net Identity) is designed.

Samples
-------
There are working samples available in our `Samples repo <https://github.com/Sustainsys/Saml2.Samples/tree/main/v2>`_.