
namespace HttpServerMock.ExtensionMethods
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// Extension methods for RequestExpectation class.
    /// </summary>
    public static class RequestExpectationExtensions
    {
        /// <summary>
        /// Add a header to the HTTP request expectation.
        /// </summary>
        /// <param name="expectation">The HTTP request expectation.</param>
        /// <param name="headerName">Name of the header.</param>
        /// <param name="headerValue">The header value.</param>
        /// <returns>The HTTP request expectation.</returns>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static RequestExpectation ExpectedRequestHeader(this RequestExpectation expectation, string headerName, string headerValue)
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            if (expectation.ExpectedRequestHeaders.ContainsKey(headerName))
            {
                expectation.ExpectedRequestHeaders[headerName] = headerValue;
                return expectation;
            }

            expectation.ExpectedRequestHeaders.Add(headerName, headerValue);
            return expectation;
        }

        /// <summary>
        /// Add a collection of headers to the HTTP request expectation.
        /// </summary>
        /// <param name="expectation">The HTTP request expectation.</param>
        /// <param name="headers">The new headers.</param>
        /// <returns>The HTTP request expectation.</returns>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static RequestExpectation ExpectedRequestHeaders(this RequestExpectation expectation, IDictionary<string, string> headers)
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            if (headers == null)
            {
                return expectation;
            }

            foreach (var expectedHeader in headers)
            {
                expectation.ExpectedRequestHeader(expectedHeader.Key, expectedHeader.Value);
            }

            return expectation;
        }

        /// <summary>
        /// Expecteds the content.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectation">The expectation.</param>
        /// <param name="content">The content.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static RequestExpectation ExpectedContent<T>(this RequestExpectation expectation, T content, HttpRequestContentType contentType) where T : class
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            expectation.ExpectedRequestContent = content;
            expectation.ExpectedRequestContentType = Helper.ParseRequestContentType(contentType);

            return expectation;
        }

        /// <summary>
        /// Expecteds the type of the content.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <returns></returns>
        public static RequestExpectation ExpectedContentType(this RequestExpectation expectation, HttpRequestContentType contentType)
        {
            expectation.ExpectedRequestContentType = Helper.ParseRequestContentType(contentType);

            return expectation;
        }

        /// <summary>
        /// Timeds the out.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        public static void TimedOut(this RequestExpectation expectation)
        {
            expectation.IsRequestTimedOut = true;
        }

        /// <summary>
        /// Validators the specified expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="requestValidator">The request validator.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static RequestExpectation Validator(this RequestExpectation expectation, Func<HttpRequestMessage, bool> requestValidator)
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            expectation.RequestValidator = requestValidator;

            return expectation;
        }

        /// <summary>
        /// Expecteds the number of calls.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="repetitions">The repetitions.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static RequestExpectation ExpectedNumberOfCalls(this RequestExpectation expectation, uint repetitions)
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            expectation.Repeats = repetitions;

            return expectation;
        }

        /// <summary>
        /// Responses the specified expectation.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expectation">The expectation.</param>
        /// <param name="responseStatusCode">The response status code.</param>
        /// <param name="responseContentType">Type of the response content.</param>
        /// <param name="responseContent">Content of the response.</param>
        /// <param name="responseHeaders">The response headers.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static RequestMockResponse Response<T>(
           this RequestExpectation expectation,
           HttpStatusCode responseStatusCode,
           HttpRequestContentType responseContentType,
           T responseContent,
           IDictionary<string, string> responseHeaders = null) where T : class
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            return ConfigureRequestMockResponse(
                expectation,
                responseStatusCode,
                responseContentType,
                responseContent,
                responseHeaders,
                null);
        }

        /// <summary>
        /// Responses the specified expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="responseStatusCode">The response status code.</param>
        /// <param name="responseHeaders">The response headers.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static RequestMockResponse Response(
           this RequestExpectation expectation,
            HttpStatusCode responseStatusCode,
           IDictionary<string, string> responseHeaders = null)
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            return ConfigureRequestMockResponse(
                expectation,
                responseStatusCode,
                HttpRequestContentType.None,
                null,
                responseHeaders,
                null);
        }

        /// <summary>
        /// Responses the specified expectation.
        /// </summary>
        /// <param name="expectation">The expectation.</param>
        /// <param name="responseBuilder">The response builder.</param>
        /// <exception cref="System.ArgumentNullException">expectation</exception>
        public static void Response(
        this RequestExpectation expectation,
        Func<HttpRequestMessage, HttpResponseMessage> responseBuilder)
        {
            if (expectation == null)
            {
                throw new ArgumentNullException("expectation");
            }

            ConfigureRequestMockResponse(
                expectation,
                HttpStatusCode.NotImplemented,
                HttpRequestContentType.None,
                null,
                null,
                responseBuilder);
        }

        #region Helper methods
        private static RequestMockResponse ConfigureRequestMockResponse(
            this RequestExpectation requestExpectation,
           HttpStatusCode responStatusCode,
           HttpRequestContentType responseContentType,
           object responseContent,
           IDictionary<string, string> responseHeaders,
           Func<HttpRequestMessage, HttpResponseMessage> responseBuilder)
        {
            requestExpectation.Response.ResponseStatusCode = responStatusCode;
            requestExpectation.Response.ResponseContentType = Helper.ParseResponseContentType(responseContentType);
            requestExpectation.Response.ResponseContent = responseContent;
            requestExpectation.Response.ResponseBuilder = responseBuilder;

            if (responseHeaders == null)
            {
                return requestExpectation.Response;
            }

            foreach (var responseHeader in responseHeaders)
            {
                requestExpectation.Response.ResponseHeaders.Add(responseHeader.Key, responseHeader.Value);
            }

            return requestExpectation.Response;
        }
        #endregion

    }
}
