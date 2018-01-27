Sustainsys Saml2 Owin Middleware
========

The Sustainsys Saml2 Owin middleware is designed to be used with an Owin
authentication pipeline and is compatible with ASP.NET Identity. Sustainsys 
Saml2 provides external login in the same way as the built in
Google, Facebook and Twitter providers.

To use the Sustainsys Saml2 middleware, it needs to be configured in
`Startup.Auth.Cs`.

    app.UseSaml2Authentication(new Saml2AuthenticationOptions());

The options class only contains the Owin-specific configuration (such as the 
name used to identify the login provider). The rest of the configuration is
read from the web.config/app.config and [configured in the same way](Configuration.md) 
as when using the http module or the MVC controller.

If you would like to provide the Saml2-related configuration in code, specify `false` for
the `loadConfiguration` constructor parameter and then build the options based on your own
logic. For example:

    var mySaml2Options = new Saml2AuthenticationOptions(false)
    // more logic to set SPOptions, etc.
    app.UseSaml2Authentication(mySaml2Options);
    
You can see a full example of this in the **SampleOwinApplication** project included in the
source code. See the [Startup.Auth.cs file](https://github.com/Sustainsys/Saml2/blob/master/Samples/SampleOwinApplication/App_Start/Startup.Auth.cs)

## Selecting Idp

An Owin-based application issues an AuthenticationResponseChallenge to ask the
middleware to begin the authentication procedure. In that challenge, there is
a properties dictionary. To use a specified idp, the entity id of the idp should
be entered in that dictionary under the key "idp".

In a typical MVC application that requires some changes to the generated code
to enable passing a property to the `AuthenticationProperties` dictionary.

Another, more simple way to pass a value is to put it directly in the Owin
environment dictionary under the key "saml2.idp".

Here's an example of how to set the Owin environment value through ASP.NET MVC

    var context = HttpContext.GetOwinContext();
    context.Environment.Add("saml2.idp", new EntityId(YOUR_IDP_ENTITY_ID));

