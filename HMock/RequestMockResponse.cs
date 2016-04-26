namespace HttpServerMock
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Defines the members of a request mock response.
    /// </summary>
    public sealed class RequestMockResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMockResponse"/> class.
        /// </summary>
        public RequestMockResponse()
        {
            this.ResponseHeaders = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the content type of the response.
        /// </summary>
        public string ResponseContentType { get; set; }

        /// <summary>
        /// Gets the response which will contain the headers.
        /// </summary>
        public Dictionary<string, string> ResponseHeaders { get; private set; }

        /// <summary>
        /// Gets or sets the content of the response.
        /// </summary>
        public object ResponseContent { get; set; }

        /// <summary>
        /// Gets or sets the response status code.
        /// </summary>
        public HttpStatusCode ResponseStatusCode { get; set; }

        /// <summary>
        /// Gets or sets a function which builds the request response.
        /// </summary>
        public Func<HttpRequestMessage, HttpResponseMessage> ResponseBuilder { get; set; }
    }
}