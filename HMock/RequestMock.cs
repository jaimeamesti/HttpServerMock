namespace HttpServerMock
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text.RegularExpressions;

    /// <summary>
    /// This is the base class for all the request mock classes of this library such as RequestExpectation and RequestBehavior;
    /// </summary>
    public abstract class RequestMock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestMock" /> class.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <exception cref="System.ArgumentNullException">requestUri</exception>
        /// <exception cref="System.ArgumentException">The give request URI has not a valid format. It only supports absolute URIs.;requestUri</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1057:StringUriOverloadsCallSystemUriOverloads", Justification = "Strings do the method more usable.")]
        protected RequestMock(string requestUri)
            : this()
        {
            if (requestUri == null)
            {
                throw new ArgumentNullException("requestUri");
            }

            this.RequestUrl = requestUri;

            if (!Uri.IsWellFormedUriString(requestUri, UriKind.RelativeOrAbsolute))
            {
                if (IsValidRegexString(requestUri))
                {
                    this.RequestRegex = new Regex(requestUri, RegexOptions.Singleline);
                    this.IsRequestUriARegex = true;
                    return;
                }

                throw new ArgumentException("The given request URI has not a valid format or it's not a valid regular expression.", "requestUri");
            }

            if (Uri.IsWellFormedUriString(requestUri, UriKind.Relative))
            {
                var baseUri = new Uri("http://localhost");
                this.RequestUri = new Uri(baseUri, requestUri);
                return;
            }

            this.RequestUri = new Uri(requestUri, UriKind.Absolute);
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="RequestMock" /> class from being created.
        /// </summary>
        private RequestMock()
        {
            this.ExpectedRequestHeaders = new Dictionary<string, string>();
            this.Response = new RequestMockResponse();
        }

        #region Request
        /// <summary>
        /// Gets or sets the request mock name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the expected request content type.
        /// </summary>
        public string ExpectedRequestContentType { get; set; }

        /// <summary>
        /// Gets or sets the request validator.
        /// </summary>
        public Func<HttpRequestMessage, bool> RequestValidator { get; set; }

        /// <summary>
        /// Gets or sets the request HTTP method.
        /// </summary>
        public HttpMethod RequestHttpMethod { get; set; }

        /// <summary>
        /// Gets the expected request headers.
        /// </summary>
        public Dictionary<string, string> ExpectedRequestHeaders { get; private set; }

        /// <summary>
        /// Gets or sets the expected content of the request.
        /// </summary>
        public object ExpectedRequestContent { get; set; }

        /// <summary>
        /// Gets the request Url.
        /// </summary>
        public string RequestUrl { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this request Uri given by the user is a Regular expression.
        /// </summary>
        /// <value>
        /// <c>true</c> if this request Uri given by the user is Regular expression; otherwise, <c>false</c>.
        /// </value>
        internal bool IsRequestUriARegex { get; private set; }

        /// <summary>
        /// Gets the request Regular expression.
        /// </summary>
        internal Regex RequestRegex { get; private set; }

        /// <summary>
        /// Gets the request Uri.
        /// </summary>
        internal Uri RequestUri { get; private set; }

        #endregion

        #region Response

        /// <summary>
        /// Gets or sets the response returned.
        /// </summary>
        public RequestMockResponse Response { get; private set; }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the request should produce a time out.
        /// </summary>
        public bool IsRequestTimedOut { get; set; }

        /// <summary>
        /// Gets the number of times that this mock was called.
        /// </summary>
        public int NumberOfCallsPerformed { get; private set; }

        /// <summary>
        /// Increments in one, the number of calls performed to the mock.
        /// </summary>
        internal void IncrementRequestMockCall()
        {
            this.NumberOfCallsPerformed += 1;
        }

        private static bool IsValidRegexString(string regex)
        {
            try
            {
                new Regex(regex, RegexOptions.Singleline);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }
}