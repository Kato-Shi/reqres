using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Entities;
using ReqResIntegratedApplication.Integration.ReqresIntegration.Services;

namespace ReqresIntegratedApplication.WebAPI.Services
{
    /// <summary>
    /// Provides access to ReqRes unknown resources and caches the latest page for reuse.
    /// </summary>
    public class ResourceService
    {
        private readonly ReqResClient _client;
        private Resource? _lastPage;
        private readonly Dictionary<int, ResourceData> _resourceCache = new();

        public ResourceService(ReqResClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Resource?> GetResourcesAsync(int page, int perPage)
        {
            _lastPage = await _client.GetResourcesAsync(page, perPage);
            if (_lastPage?.Data is { Count: > 0 })
            {
                foreach (var resource in _lastPage.Data)
                {
                    _resourceCache[resource.Id] = resource;
                }
            }

            return _lastPage;
        }

        public async Task<ResourceData?> GetResourceAsync(int id)
        {
            if (_resourceCache.TryGetValue(id, out var cached))
            {
                return cached;
            }

            var resource = await _client.GetResourceAsync(id);
            if (resource is not null)
            {
                _resourceCache[id] = resource;
            }

            return resource;
        }

        public IReadOnlyCollection<ResourceData> GetCachedResources() => _resourceCache.Values;
    }
}
