Kentor AuthServices Owin Middleware
========

The Kentor AuthServices Owin middleware is designed to be used with an Owin
authentication pipeline and is compatible with ASP.NET Identity. Kentor 
AuthServices provides external login in the same way as the built in
Google, Facebook and Twitter providers.

To use the Kentor AuthServices middleware, it need to be configured in
`Startup.Auth.Cs`.

    app.UseKentorAuthServicesAuthentication(new KentorAuthServicesAuthenticationOptions());

The options class only contains the owin specific configuration (such as the 
name used to identify the login provider). The rest of the configuration is
read from the web.config/app.config and [configured in the same way](Configuration.md) 
as when using the http module or the MVC controller.

##Selecting Idp
An Owin based application issues a AuthenticationResponseChallenge to ask the
middleware to begin the authentication procedure. In that challenge, there is
a properties dictionary. To use a specified idp, the entity id of the idp should
be entered in that dictionary under the key "idp".

In a typical MVC application that requires some changes to the generated code
to enable passing a property to the `AuthenticationProperties` dictionary.

Another, more simple way to pass a value is to put it directly in the Owin
environment dictionary under the key "KentorAuthServices.idp".

Here's an example of how to set the Owin environment value through ASP.NET MVC

    var context = HttpContext.GetOwinContext();
    context.Environment.Add("KentorAuthServices.idp", new EntityId(YOUR_IDP_ENTITY_ID));

