using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
        private int _nextId = 204;
        private static readonly ResourceData[] DemoResources =
        {
            new() { Id = 201, Name = "Demo Widget", Color = "#0099cc", Year = 2024, PantoneValue = "14-4121" },
            new() { Id = 202, Name = "Demo Crate", Color = "#cc3300", Year = 2023, PantoneValue = "18-1561" },
            new() { Id = 203, Name = "Demo Pallet", Color = "#669900", Year = 2022, PantoneValue = "16-0532" }
        };

        public ResourceService(ReqResClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<Resource?> GetResourcesAsync(int page, int perPage)
        {
            try
            {
                _lastPage = await _client.GetResourcesAsync(page, perPage);
                if (_lastPage?.Data is { Count: > 0 })
                {
                    foreach (var resource in _lastPage.Data)
                    {
                        _resourceCache[resource.Id] = resource;
                        _nextId = Math.Max(_nextId, resource.Id + 1);
                    }
                }

                return _lastPage ?? BuildLocalResourcePage();
            }
            catch (HttpRequestException)
            {
                return BuildLocalResourcePage();
            }
        }

        public async Task<ResourceData?> GetResourceAsync(int id)
        {
            if (_resourceCache.TryGetValue(id, out var cached))
            {
                return cached;
            }

            try
            {
                var resource = await _client.GetResourceAsync(id);
                if (resource is not null)
                {
                    _resourceCache[id] = resource;
                }

                return resource ?? GetLocalOrDemoResource(id);
            }
            catch (HttpRequestException)
            {
                return GetLocalOrDemoResource(id);
            }
        }

        public IReadOnlyCollection<ResourceData> GetCachedResources() => _resourceCache.Values;

        public ResourceData AddResource(ResourceData resource)
        {
            resource.Id = resource.Id == 0 ? _nextId++ : resource.Id;
            _resourceCache[resource.Id] = resource;
            return resource;
        }

        public ResourceData? UpdateResource(int id, ResourceData updated)
        {
            if (!_resourceCache.TryGetValue(id, out var existing))
            {
                existing = new ResourceData { Id = id };
                _resourceCache[id] = existing;
                _nextId = Math.Max(_nextId, id + 1);
            }

            existing.Name = updated.Name;
            existing.Color = updated.Color;
            existing.Year = updated.Year;
            existing.PantoneValue = updated.PantoneValue;

            return existing;
        }

        private Resource? BuildLocalResourcePage()
        {
            if (_resourceCache.Count == 0)
            {
                foreach (var resource in DemoResources)
                {
                    _resourceCache[resource.Id] = resource;
                    _nextId = Math.Max(_nextId, resource.Id + 1);
                }
            }

            return new Resource
            {
                Page = 1,
                PerPage = _resourceCache.Count,
                Total = _resourceCache.Count,
                TotalPages = 1,
                Data = _resourceCache.Values.ToList()
            };
        }

        private ResourceData? GetLocalOrDemoResource(int id)
        {
            if (_resourceCache.TryGetValue(id, out var cached))
            {
                return cached;
            }

            var demo = DemoResources.FirstOrDefault(r => r.Id == id);
            if (demo is not null)
            {
                _resourceCache[id] = demo;
                return demo;
            }

            return null;
        }
    }
}
