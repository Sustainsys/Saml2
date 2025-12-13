// Copyright (c) Sustainsys AB. All rights reserved.
// Any usage requires a valid license agreement with Sustainsys AB

using Duende.IdentityServer.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sustainsys.Saml2.DuendeIdentityServer.Services;

internal class EndpointPathSetter
    (IOptionsMonitor<Saml2Options> optionsMonitor,
    IEnumerable<Endpoint> endpoints)
    : IHostedService
{
    private IDisposable? listener;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (listener != null)
        {
            // Already started.
            return Task.CompletedTask;
        }

        UpdateEndpoints(optionsMonitor.CurrentValue, endpoints);

        listener = optionsMonitor.OnChange((opt, name) =>
            UpdateEndpoints(opt, endpoints));

        return Task.CompletedTask;
    }

    private static void UpdateEndpoints(Saml2Options options, IEnumerable<Endpoint> endpoints)
    {
        foreach (Endpoint endpoint in endpoints)
        {
            var path =
            endpoint.Name switch
            {
                Saml2Constants.EndPoints.SingleSignonService => options.Endpoints.SingleSignOnServicePath,
                Saml2Constants.EndPoints.Metadata => options.Endpoints.MetadataPath,
                _ => null
            };

            if (path != null)
            {
                endpoint.Path = path;
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // Let's be on the safe side if someone would call this multiple times.
        listener?.Dispose();
        listener = null;

        return Task.CompletedTask;
    }
}
