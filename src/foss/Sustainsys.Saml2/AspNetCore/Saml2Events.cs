// Copyright (c) Sustainsys AB. All rights reserved.
// Licensed under the MIT license. See LICENSE in the project root for license information.

using Microsoft.AspNetCore.Authentication;
using Sustainsys.Saml2.AspNetCore.Events;

namespace Sustainsys.Saml2.AspNetCore;

/// <summary>
/// Events represents the easiest and most straight forward way to customize the
/// behaviour of the Saml2 handler. An event can inspect and alter data.
/// </summary>
public class Saml2Events : RemoteAuthenticationEvents
{
    /// <summary>
    /// Invoked after the AuthnRequest has been generated. Can be used to modify the
    /// AuthnRequest before it is sent
    /// </summary>
    public Func<AuthnRequestGeneratedContext, Task> OnAuthnRequestGeneratedAsync
    { get; set; } = ctx => Task.CompletedTask;

    /// <summary>
    /// Invoked after the AuthnRequest has been generated. Can be used to modify the
    /// AuthnRequest before it is sent
    /// </summary>
    /// <param name="context">Event context</param>
    /// <returns>Task</returns>
    public virtual Task AuthnRequestGeneratedAsync(AuthnRequestGeneratedContext context) =>
        OnAuthnRequestGeneratedAsync(context);
}