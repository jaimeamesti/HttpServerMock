namespace HttpServerMock.ExtensionMethods
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines extension methods for requests.
    /// </summary>
    public static class RequestMockResponseExtensions
    {
        /// <summary>
        /// Add a HTTP header to the request response.
        /// </summary>
        /// <param name="response">The HTTP request response.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">The header value.</param>
        /// <returns>The HTTP request response.</returns>
        /// <exception cref="System.ArgumentNullException">response</exception>
        public static RequestMockResponse ResponseHeader(this RequestMockResponse response, string headerName, string headerValue)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (response.ResponseHeaders.ContainsKey(headerName))
            {
                response.ResponseHeaders[headerName] = headerValue;
                return response;
            }

            response.ResponseHeaders.Add(headerName, headerValue);
            return response;
        }

        /// <summary>
        /// Add a collection of HTTP headers to the request response.
        /// </summary>
        /// <param name="response">The HTTP request response.</param>
        /// <param name="headers">The response headers.</param>
        /// <returns>The HTTP request response.</returns>
        /// <exception cref="System.ArgumentNullException">response</exception>
        public static RequestMockResponse ResponseHeaders(this RequestMockResponse response, IDictionary<string, string> headers)
        {
            if (response == null)
            {
                throw new ArgumentNullException("response");
            }

            if (headers == null)
            {
                return response;
            }

            foreach (var responseHeader in headers)
            {
                response.ResponseHeader(responseHeader.Key, responseHeader.Value);
            }

            return response;
        }
    }
}