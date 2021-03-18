using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using OrchardCore.Environment.Shell;

namespace OrchardCore.Setup.Core
{
    public class UrlService : IUrlService
    {
        private readonly HttpContextAccessor _httpContextAccessor;
        public UrlService(HttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public string GetEncodedUrl(ShellSettings shellSettings, Dictionary<string, string> queryParams = null)
        {
            string baseUrl = string.Empty;
            var httpContext = _httpContextAccessor.HttpContext;
            HostString hostString;

            var tenantUrlHost = shellSettings.RequestUrlHost?.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            if (string.IsNullOrEmpty(tenantUrlHost))
            {
                hostString = httpContext.Request.Host;
            }
            else
            {
                hostString = new HostString(tenantUrlHost);
            }

            var pathString = httpContext.Features.Get<ShellContextFeature>().OriginalPathBase;
            if (!String.IsNullOrEmpty(shellSettings.RequestUrlPrefix))
            {
                pathString = pathString.Add('/' + shellSettings.RequestUrlPrefix);
            }


            baseUrl = $"{httpContext.Request.Scheme}://{hostString.ToUriComponent() + pathString.ToUriComponent()}";

            if (queryParams != null)
            {
                var queryString = QueryString.Create(queryParams);
                baseUrl += queryString.ToUriComponent();
            }

            return baseUrl;
        }
    }
}
