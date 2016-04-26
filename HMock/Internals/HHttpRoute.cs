namespace HttpServerMock.Internals
{
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Web.Http.Routing;

    internal sealed class HHttpRoute : IHttpRoute
    {
        private readonly ServerRequestsState serverRequestsState;

        public HHttpRoute(ServerRequestsState serverRequestsState)
        {
            this.serverRequestsState = serverRequestsState;
            this.Defaults = new Dictionary<string, object>();
            this.Constraints = new Dictionary<string, object>();
            this.DataTokens = new Dictionary<string, object>();
            this.Handler = new HHttpMessageHandler(this.serverRequestsState);
        }

        public IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request)
        {
            if (request == null)
            {
                return null;
            }

            var route = new HttpRouteData(this);

            return route;
        }

        public IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values)
        {
            return null;
        }

        public string RouteTemplate
        {
            get
            {
                return string.Empty;
            }
        }

        public IDictionary<string, object> Defaults { get; private set; }

        public IDictionary<string, object> Constraints { get; private set; }

        public IDictionary<string, object> DataTokens { get; private set; }

        public HttpMessageHandler Handler { get; private set; }
    }
}