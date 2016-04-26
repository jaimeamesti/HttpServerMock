namespace HttpServerMock
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;

    /// <summary>
    /// Represents a server request which was not expected.
    /// </summary>
    public sealed class UnexpectedRequest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedRequest"/> class.
        /// </summary>
        /// <param name="method">The request method.</param>
        /// <param name="headers">The request headers.</param>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="content">The request content.</param>
        public UnexpectedRequest(System.Net.Http.HttpMethod method, HttpRequestHeaders headers, Uri requestUri, HttpContent content)
        {
            if (method == null)
            {
                throw new ArgumentNullException("method");
            }

            if (headers == null)
            {
                throw new ArgumentNullException("headers");
            }

            if (content == null)
            {
                throw new ArgumentNullException("content");
            }

            this.Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), method.Method);
            this.Headers = new Dictionary<string, IEnumerable<string>>();

            foreach (var httpRequestHeader in headers)
            {
                this.Headers.Add(httpRequestHeader.Key, httpRequestHeader.Value);
            }

            this.RequestUri = requestUri;
            this.Content = content.ReadAsStringAsync().Result;
        }

        /// <summary>
        ///  Gets the contents of the HTTP message.
        /// </summary>
        public string Content { get; private set; }

        /// <summary>
        ///  Gets the collection of HTTP request headers.
        /// </summary>
        public Dictionary<string, IEnumerable<string>> Headers { get; private set; }

        /// <summary>
        /// Gets the HTTP method used by the HTTP request message.
        /// </summary>
        public HttpMethod Method { get; private set; }

        /// <summary>
        /// Gets the request URI.
        /// </summary>
        public Uri RequestUri { get; private set; }
    }
}