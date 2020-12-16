using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;

namespace SampleAesGcmNetCore31
{
  public class Startup
  {
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
      CryptoConfig.AddAlgorithm(typeof(AesGcmAlgorithm), AesGcmAlgorithm.AesGcm128Identifier);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseRouting();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapGet("/", async context =>
        {
          var aesGcm = CryptoConfig.CreateFromName(AesGcmAlgorithm.AesGcm128Identifier);
          await context.Response.WriteAsync(
            $"Resolved '{AesGcmAlgorithm.AesGcm128Identifier}' to {aesGcm.GetType().FullName}");
        });
      });
    }
  }
}
