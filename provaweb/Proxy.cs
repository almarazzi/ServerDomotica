﻿//using Microsoft.Extensions.Primitives;
//using Yarp.ReverseProxy.Configuration;
//using Yarp.ReverseProxy.LoadBalancing;

/*namespace provaweb
{
    public class Proxy : IProxyConfigProvider
    {

        private CustomMemoryConfig _config;

        public Proxy()
        {
            // Load a basic configuration
            // Should be based on your application needs.
            var routeConfig = new RouteConfig
            {
                RouteId = "route",
                ClusterId = "cluster",
                Match = new RouteMatch
                {
                    Path = "/api/{**catch-all}"
                }
            };

            var routeConfigs = new[] { routeConfig };

            var clusterConfigs = new[]
            {
            new ClusterConfig
            {
                ClusterId = "cluster",
                LoadBalancingPolicy = LoadBalancingPolicies.RoundRobin,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    { "destination", new DestinationConfig { Address = "http://192.168.1.2/" } },
                }
            }
        };

            _config = new CustomMemoryConfig(routeConfigs, clusterConfigs);
        }

        public IProxyConfig GetConfig() => _config;

        /// <summary>
        /// By calling this method from the source we can dynamically adjust the proxy configuration.
        /// Since our provider is registered in DI mechanism it can be injected via constructors anywhere.
        /// </summary>
        public void Update(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            var oldConfig = _config;
            _config = new CustomMemoryConfig(routes, clusters);
            oldConfig.SignalChange();
        }

        private class CustomMemoryConfig : IProxyConfig
        {
            private readonly CancellationTokenSource _cts = new CancellationTokenSource();

            public CustomMemoryConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
            {
                Routes = routes;
                Clusters = clusters;
                ChangeToken = new CancellationChangeToken(_cts.Token);
            }

            public IReadOnlyList<RouteConfig> Routes { get; }

            public IReadOnlyList<ClusterConfig> Clusters { get; }

            public IChangeToken ChangeToken { get; }

            internal void SignalChange()
            {
                _cts.Cancel();
            }
        }
    }
}
*/