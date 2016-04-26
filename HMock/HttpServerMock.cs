namespace HttpServerMock
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Web.Http.SelfHost;

    using global::HttpServerMock.Internals;

    /// <summary>
    /// HTTP server mock class.
    /// </summary>
    public sealed class HttpServerMock : IDisposable
    {
        private HttpSelfHostServer httpServer;

        private bool isDisposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerMock"/> class.
        /// </summary>
        /// <param name="port">The HTTP server port.</param>
        public HttpServerMock(uint port)
        {
            this.IpOrDns = Dns.GetHostName();
            this.Port = port;
            this.ServerRequestsState = new ServerRequestsState();

            this.StartServer();
        }

        /// <summary>
        /// Gets the HTTP server IP or DNS.
        /// </summary>
        public string IpOrDns { get; private set; }

        /// <summary>
        /// Gets the HTTP server port.
        /// </summary>
        public uint Port { get; private set; }

        /// <summary>
        /// Gets the current server requests state.
        /// </summary>
        public ServerRequestsState ServerRequestsState { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.Stop();
                this.httpServer.Dispose();
                this.isDisposed = true;
            }
        }

        /// <summary>
        /// Starts the HTTP server mock.
        /// </summary>
        public void Start()
        {
            this.httpServer.OpenAsync().Wait();
        }

        /// <summary>
        /// Stops this HTTP server mock.
        /// </summary>
        public void Stop()
        {
            this.httpServer.CloseAsync().Wait();
        }

        private void StartServer()
        {
            string baseAddress = string.Format(CultureInfo.InvariantCulture, "http://{0}:{1}", this.IpOrDns, this.Port);
            var config = new HttpSelfHostConfiguration(baseAddress);

            // It adds any route.
            config.Routes.Add("~", new HHttpRoute(this.ServerRequestsState));
            this.httpServer = new HttpSelfHostServer(config);

            this.httpServer.InnerHandler = new HHttpMessageHandler(this.ServerRequestsState);
            this.Start();
        }
    }
}