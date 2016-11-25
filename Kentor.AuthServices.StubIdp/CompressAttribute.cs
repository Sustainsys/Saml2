// From http://stackoverflow.com/a/3802424/401728
using System;
using System.IO.Compression;
using System.Web.Mvc;

/// <summary>
/// Attribute for compressing with deflate or gzip depending on request Accept-Encoding header
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class CompressAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {

        var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
        if (string.IsNullOrEmpty(encodingsAccepted)) return;

        encodingsAccepted = encodingsAccepted.ToLowerInvariant();
        var response = filterContext.HttpContext.Response;

        if (encodingsAccepted.Contains("deflate"))
        {
            response.AppendHeader("Content-encoding", "deflate");
            response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
        }
        else if (encodingsAccepted.Contains("gzip"))
        {
            response.AppendHeader("Content-encoding", "gzip");
            response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
        }
    }
}