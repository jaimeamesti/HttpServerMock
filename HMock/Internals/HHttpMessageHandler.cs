namespace HttpServerMock.Internals
{

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Linq;

    internal sealed class HHttpMessageHandler : HttpMessageHandler
    {
        private readonly ServerRequestsState serverRequestsState;

        public HHttpMessageHandler(ServerRequestsState serverRequestsState)
        {
            this.serverRequestsState = serverRequestsState;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpResponse = new HttpResponseMessage(this.serverRequestsState.DefaultRespondStatusCode);
            string requestContent = await request.Content.ReadAsStringAsync();

            RequestMock requestMock = this.FindExpectationForRequest(request, requestContent); // ?? this.FindBehaviorForRequest(request, requestContent);

            if (requestMock != null)
            {
                if (requestMock.IsRequestTimedOut)
                {
                    Thread.Sleep(120000);
                    return httpResponse;
                }

                BuildResponseFromRequestMock(requestMock, request, ref httpResponse);
                return httpResponse;
            }

            this.AddUnexpectedRequest(request);

            return httpResponse;
        }

        private RequestMock FindExpectationForRequest(HttpRequestMessage request, string requestContent)
        {
            foreach (var expectation in this.serverRequestsState.RequestExpectations)
            {
                // It checks if the request expectations has already reached the number of repetitions configured.
                if (expectation.NumberOfCallsPerformed >= expectation.Repeats)
                {
                    continue;
                }

                if (!IsMockForRequest(expectation, request, requestContent))
                {
                    continue;
                }

                expectation.IncrementRequestMockCall();
                return expectation;
            }

            return null;
        }

        ////private RequestMock FindBehaviorForRequest(HttpRequestMessage request, string requestContent)
        ////{
        ////    foreach (var behavior in this.serverRequestsState.RequestBehaviors)
        ////    {
        ////        if (!IsMockForRequest(behavior, request, requestContent))
        ////        {
        ////            continue;
        ////        }

        ////        behavior.IncrementRequestMockCall();
        ////        return behavior;
        ////    }

        ////    return null;
        ////}

        private static void BuildResponseFromRequestMock(RequestMock mock, HttpRequestMessage httpRequest, ref HttpResponseMessage httpResponse)
        {
            if (mock.Response.ResponseBuilder != null)
            {
                httpResponse = mock.Response.ResponseBuilder(httpRequest);
                return;
            }

            httpResponse.StatusCode = mock.Response.ResponseStatusCode;

            if (mock.Response.ResponseContent != null)
            {
                // TODO: In the future it must return any content type.

                if (Helper.IsJsonRequest(mock.Response.ResponseContentType))
                {
                    string responseContent;

                    if (mock.Response.ResponseContent is string)
                    {
                        responseContent = ((string)mock.Response.ResponseContent);
                    }
                    else
                    {
                        responseContent = JsonConvert.SerializeObject(mock.Response.ResponseContent);
                    }

                    httpResponse.Content = new StringContent(responseContent, Encoding.UTF8, ConvertFromHContentTypeToString(HttpRequestContentType.Json));
                }
                else if (Helper.IsXmlRequest(mock.Response.ResponseContentType))
                {
                    // TODO: Shit code. It must be improved. At this moment, it works but it's a shit.
                    // ----------------------------------------------------------------------------------------------------------------------
                    string responseContent;

                    if (mock.Response.ResponseContent is string)
                    {
                        responseContent = ((string)mock.Response.ResponseContent);
                    }
                    else
                    {
                        responseContent = JsonConvert.SerializeObject(mock.Response.ResponseContent);
                        responseContent = JsonConvert.DeserializeXNode(responseContent, "Root").ToString();
                    }

                    httpResponse.Content = new StringContent(responseContent, Encoding.UTF8, ConvertFromHContentTypeToString(HttpRequestContentType.Xml));
                    // ----------------------------------------------------------------------------------------------------------------------
                }
                else
                {
                    // TODO: Throw an exception???
                }
            }
            else
            {
                httpResponse.Content = new ByteArrayContent(new byte[0]);
            }

            foreach (var responseHeader in mock.Response.ResponseHeaders)
            {
                if (httpResponse.Headers.TryAddWithoutValidation(responseHeader.Key, responseHeader.Value) == false)
                {
                    if (httpResponse.Content != null && httpResponse.Content.Headers != null)
                    {
                        httpResponse.Content.Headers.TryAddWithoutValidation(responseHeader.Key, responseHeader.Value);
                    }
                }
            }
        }

        private static bool IsMockForRequest(RequestMock mock, HttpRequestMessage request, string requestContent)
        {
            // It checks if the request method is the expected.
            if (mock.RequestHttpMethod.ToString() != request.Method.Method)
            {
                return false;
            }

            // It validates the URI.
            if (mock.IsRequestUriARegex)
            {
                if (!mock.RequestRegex.IsMatch(request.RequestUri.PathAndQuery))
                {
                    return false;
                }
            }
            else
            {
                // https://msdn.microsoft.com/en-us/library/system.uri.pathandquery(v=vs.110).aspx
                var comparisonResult = Uri.Compare(
                    mock.RequestUri,
                    request.RequestUri,
                    UriComponents.PathAndQuery,
                    UriFormat.SafeUnescaped,
                    StringComparison.OrdinalIgnoreCase);

                if (comparisonResult != 0)
                {
                    return false;
                }

                // https://msdn.microsoft.com/en-us/library/system.uri.userinfo(v=vs.110).aspx
                comparisonResult = Uri.Compare(
                    mock.RequestUri,
                    request.RequestUri,
                    UriComponents.UserInfo,
                    UriFormat.SafeUnescaped,
                    StringComparison.OrdinalIgnoreCase);

                if (comparisonResult != 0)
                {
                    return false;
                }

                // https://msdn.microsoft.com/en-us/library/system.uri.fragment(v=vs.110).aspx
                comparisonResult = Uri.Compare(
                    mock.RequestUri,
                    request.RequestUri,
                    UriComponents.Fragment,
                    UriFormat.SafeUnescaped,
                    StringComparison.OrdinalIgnoreCase);

                if (comparisonResult != 0)
                {
                    return false;
                }
            }

            // It validates the request headers.
            foreach (var expectedHeader in mock.ExpectedRequestHeaders)
            {
                IEnumerable<string> headerValues;

                if (request.Headers.TryGetValues(expectedHeader.Key, out headerValues) || request.Content.Headers.TryGetValues(expectedHeader.Key, out headerValues))
                {
                    if (headerValues.Contains(expectedHeader.Value))
                    {
                        continue;
                    }
                }

                return false;
            }

            // It validates the request content type.
            if (string.IsNullOrWhiteSpace(mock.ExpectedRequestContentType) == false)
            {
                if (request.Content.Headers.ContentType == null)
                {
                    return false;
                }

                var expectedContentTypes = mock.ExpectedRequestContentType.Split(',');

                if (!expectedContentTypes.Contains(request.Content.Headers.ContentType.ToString()))
                {
                    return false;
                }

                ////if (!request.Content.Headers.ContentType.Equals(new MediaTypeHeaderValue(mock.ExpectedRequestContentType)))
                ////{
                ////    return false;
                ////}
            }

            // It validates the expected request content.
            // TODO: At this moment, it only compares JSON requests, in the future, it should be able to compare XML, FormUrlEncode and Text requests.
            if (mock.ExpectedRequestContent != null)
            {
                if (string.IsNullOrWhiteSpace(requestContent))
                {
                    return false;
                }

                if (Helper.IsJsonRequest(request.Content.Headers.ContentType.ToString()))
                {
                    var requestJson = JToken.Parse(requestContent);

                    JToken expectedJson;

                    if (mock.ExpectedRequestContent is string)
                    {
                        expectedJson = JToken.Parse(((string)mock.ExpectedRequestContent));
                    }
                    else
                    {
                        expectedJson = JToken.FromObject(mock.ExpectedRequestContent);
                    }

                    if (!JToken.DeepEquals(requestJson, expectedJson))
                    {
                        return false;
                    }
                }
                else if (Helper.IsXmlRequest(request.Content.Headers.ContentType.ToString()))
                {
                    // TODO: Shit code. It must be improved. At this moment, it works but it's a shit.
                    // ----------------------------------------------------------------------------------------------------------------------
                    JToken expectedJson;

                    if (mock.ExpectedRequestContent is string)
                    {
                        var xmlContent = XDocument.Parse((string)mock.ExpectedRequestContent);
                        expectedJson = JToken.Parse(JsonConvert.SerializeXNode(xmlContent.Root)).First.First;
                    }
                    else
                    {
                        var xmlContent = JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(mock.ExpectedRequestContent), "Root");
                        expectedJson = JToken.Parse(JsonConvert.SerializeXNode(xmlContent.Root)).First.First;
                    }

                    var requestJson = JToken.Parse(JsonConvert.SerializeXNode(XDocument.Parse(requestContent))).First.First;

                    if (!JToken.DeepEquals(requestJson, expectedJson))
                    {
                        return false;
                    }
                    // ----------------------------------------------------------------------------------------------------------------------
                }
                else
                {
                    if (mock.ExpectedRequestContent is string)
                    {
                        if (requestContent != mock.ExpectedRequestContent.ToString())
                        {
                            return false;
                        }
                    }

                    // TODO: Throw an exception???
                }

                ////case HContentType.FormUrlEncoded:
                ////    {
                ////        // TODO: Shit code. It must be improved. At this moment, it works but it's a shit.
                ////        // ----------------------------------------------------------------------------------------------------------------------
                ////        var urlEncodedContent = request.Content.ReadAsFormDataAsync().Result;

                ////        for (int i = 0; i < urlEncodedContent.Count; i++)
                ////        {

                ////        }

                ////        var json = JToken.Parse(JsonConvert.SerializeObject(urlEncodedContent));



                ////        break;
                ////        // ----------------------------------------------------------------------------------------------------------------------
                ////    }
            }

            // It executes the custom request validator configured by the user.
            if (mock.RequestValidator != null && !mock.RequestValidator(request))
            {
                return false;
            }

            return true;
        }

        private static string ConvertFromHContentTypeToString(HttpRequestContentType contentType)
        {
            switch (contentType)
            {
                case HttpRequestContentType.None:
                    {
                        return string.Empty;
                    }

                case HttpRequestContentType.Json:
                    {
                        return "application/json";
                    }

                case HttpRequestContentType.Xml:
                    {
                        return "application/xml";
                    }

                ////case HContentType.Text:
                ////    {
                ////        return "text/plain";
                ////    }

                ////case HContentType.FormUrlEncoded:
                ////    {
                ////        return "application/x-www-form-urlencoded";
                ////    }

                default:
                    {
                        throw new ArgumentOutOfRangeException("contentType");
                    }
            }
        }

        private void AddUnexpectedRequest(HttpRequestMessage request)
        {
            var unexpectedRequest = new UnexpectedRequest(
                request.Method,
                request.Headers,
                request.RequestUri,
                request.Content);

            this.serverRequestsState.UnexpectedRequests.Add(unexpectedRequest);
        }
    }
}