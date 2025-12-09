using ApplicationLayer.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.Services
{
    public class UrlService : IUrlService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UrlService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetFullUrl(string? path)
        {
            var serverUrl = GetServerUrl();
            if (string.IsNullOrEmpty(path))
            {
                return serverUrl;
            }
            return $"{serverUrl}/{path.TrimStart('/')}";
        }

        public string GetServerUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host}";
        }
    }
}
