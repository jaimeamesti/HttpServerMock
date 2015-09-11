# HttpServerMock
HTTP server mock is a library for testing which mocks HTTP requests.

You can find the libraries here: https://www.nuget.org/packages/HttpServerMock/

SOME EXAMPLES

 Delete
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpExpectation(HttpMethod.DELETE, "http://localhost:50000/user/23")
                    .ExpectedRequestHeader("test", "test1")
                    .Response(
                        HttpStatusCode.OK,
                        HttpRequestContentType.Json,
                        new ResponseTestClass { Name = "response", IsOld = true, Age = 12 });

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddJsonBody(new { Name = "test", Id = 23 });
                request.AddHeader("test", "test1");
                request.Method = Method.DELETE;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                hserver.VerifyAllRequestExpectationsAndUnexpectedRequests();
            }

			
 Post
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpPostExpectation("http://localhost:50000/user/23")
                    .ExpectedNumberOfCalls(1)
                    .ExpectedContent(new { Name = "test", Id = 23 }, HttpRequestContentType.Json)
                    .ExpectedRequestHeader("test", "test1")
                    .Response(HttpStatusCode.OK, HttpRequestContentType.Json, new ResponseTestClass { Name = "response", IsOld = true, Age = 12 });

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddJsonBody(new { Name = "test", Id = 23 });
                request.AddHeader("test", "test1");
                request.Method = Method.POST;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNotNull(response.Data, "The response is empty.");               
                Assert.AreEqual("response", response.Data.Name, "The response data is not the expected.");
                Assert.IsTrue(response.Data.IsOld, "The response data is not the expected.");
                Assert.AreEqual(12, response.Data.Age, "The response data is not the expected.");

                hserver.VerifyAllRequestExpectationsAndUnexpectedRequests();
            }
			

 Using Relatives Uris
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("user/23")
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23") { Method = Method.GET };

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
            }

			
 Setting up a default status code
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.ServerRequestsState.DefaultRespondStatusCode = HttpStatusCode.NotModified;

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.Method = Method.GET;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.NotModified, response.StatusCode, "The respond status code is not the expected.");
            }

			
 Using a request validator
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("http://localhost:50000/user/23")
                    .ExpectedNumberOfCalls(1)                    
                    .Validator(
                        req =>
                        {
                            return req.RequestUri.PathAndQuery == "/user/23";
                        })
                    .Response(HttpStatusCode.OK, HttpRequestContentType.Json, new ResponseTestClass { Name = "response", IsOld = true, Age = 12 });

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddJsonBody(new { Name = "test", Id = 23 });
                request.AddHeader("test", "test1");
                request.Method = Method.GET;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNotNull(response.Data, "The response is empty.");                
                Assert.AreEqual("response", response.Data.Name, "The response data is not the expected.");
                Assert.IsTrue(response.Data.IsOld, "The response data is not the expected.");
                Assert.AreEqual(12, response.Data.Age, "The response data is not the expected.");

                hserver.VerifyAllRequestExpectationsAndUnexpectedRequests();
            }

			
 Setting up a number of request repetitions
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpPostExpectation("http://localhost:50000/user/23")
                    .ExpectedNumberOfCalls(2)
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23") { Method = Method.POST };

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");

                response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");

                response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode, "The respond status code is not the expected.");

                hserver.VerifyAllRequestExpectationsAndUnexpectedRequests();
            }

			
 Http request header validation
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpPostExpectation("http://localhost:50000/user/23")
                    .ExpectedRequestHeaders(new Dictionary<string, string> { { "test", "value" } })
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddHeader("test", "value");
                request.Method = Method.POST;

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
            }

			
 Using plain Json to validate the request content
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpPostExpectation("http://localhost:50000/user/23")
                    .ExpectedContent("{\"Name\":\"test\", \"IsOld\":true, \"Id\":23}", HttpRequestContentType.Json)
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddJsonBody(new { Name = "test", Id = 23, IsOld = true });
                request.Method = Method.POST;

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
            }

			
 using plain XML to validate the request content
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpPostExpectation("user/23")
                    .ExpectedContent("<User><Name>test</Name><IsOld>true</IsOld><Age>23</Age></User>", HttpRequestContentType.Xml)
                    .Response(HttpStatusCode.OK, HttpRequestContentType.Xml, new { Name = "testres", Age = 25, IsOld = false });

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddXmlBody(new ResponseTestClass() { Name = "test", Age = 23, IsOld = true });
                request.Method = Method.POST;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
                Assert.AreEqual("testres", response.Data.Name, "The data returned by the server is not the expected.");
                Assert.AreEqual(false, response.Data.IsOld, "The data returned by the server is not the expected.");
                Assert.AreEqual(25, response.Data.Age, "The data returned by the server is not the expected.");
            }

			
 using dynamics to validate the XML request content
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpPostExpectation("user/23")
                    .ExpectedContent(new { Name = "test", IsOld = true, Age = 23 }, HttpRequestContentType.Xml)
                    .Response(HttpStatusCode.OK, HttpRequestContentType.Xml, new { Name = "testres", Age = 25, IsOld = false });

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddXmlBody(new ResponseTestClass() { Name = "test", Age = 23, IsOld = true });
                request.Method = Method.POST;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
                Assert.AreEqual("testres", response.Data.Name, "The data returned by the server is not the expected.");
                Assert.AreEqual(false, response.Data.IsOld, "The data returned by the server is not the expected.");
                Assert.AreEqual(25, response.Data.Age, "The data returned by the server is not the expected.");
            }

			
 Using regular expressions for URL validation
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("user/[0-9]{2}")
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23/data") { Method = Method.GET };

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
            }

    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("^/user/[0-9]{2}/data\\?name=paco$")
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23/data?name=paco") { Method = Method.GET };

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
            }

			
 Using a response builder function
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("http://localhost:50000/user/23").ExpectedNumberOfCalls(1).Response(
                    req =>
                    {
                        var resp = new HttpResponseMessage
                        {
                            Content =
                                new StringContent(
                                "{\"Name\":\"test\", \"IsOld\":false, \"Age\":1}")
                        };

                        resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                        return resp;
                    });

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddJsonBody(new { Name = "test", Id = 23 });
                request.AddHeader("test", "test1");
                request.Method = Method.GET;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNotNull(response.Data, "The response is empty.");                
                Assert.AreEqual("test", response.Data.Name, "The response data is not the expected.");
                Assert.IsFalse(response.Data.IsOld, "The response data is not the expected.");
                Assert.AreEqual(1, response.Data.Age, "The response data is not the expected.");

                hserver.VerifyAllRequestExpectationsAndUnexpectedRequests();
            }

			
 Adding special HTTP headers to the responses
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("http://localhost:50000/user/23")
                    .Response(HttpStatusCode.OK)
                    .ResponseHeader("Content-Type", "application/text");

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.Method = Method.GET;

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
                Assert.IsTrue(response.Headers.Any(h => h.Name == "Content-Type" && h.Value.ToString() == "application/text"), "The response does not contains the header.");
            }

			
 Request with URL encoded and not encoded
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("http://localhost:50000/user/age?q=%3E23")
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/age?q=%3E23") { Method = Method.GET };

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
            }

    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("http://localhost:50000/user/age?q=%3E23")
                    .Response(HttpStatusCode.OK);

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/age?q=>E23") { Method = Method.GET };

                var response = restClient.Execute(request);
                Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");
            }

			
 Creating a timed out request
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                hserver.SetUpGetExpectation("http://localhost:50000/usER/23")
                    .TimedOut();

                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23") { Method = Method.GET };

                var response = restClient.Execute(request);
                Assert.AreEqual(response.ErrorMessage, "The operation has timed out", "The request does not timed out.");
            }

			
 Verify all requests calls
 
    using (var hserver = new HttpServerMock(TestServerPort))
            {
                var restClient = new RestClient(this.serverBaseUrl);
                var request = new RestRequest("/user/23");
                request.AddJsonBody(new { Name = "test", Id = 23 });
                request.AddHeader("test", "test1");
                request.Method = Method.POST;

                var response = restClient.Execute<ResponseTestClass>(request);
                Assert.AreEqual(HttpStatusCode.NotImplemented, response.StatusCode, "The respond status code is not the expected.");
                Assert.IsNull(response.Data, "The response is not empty.");
                Assert.IsNull(response.ErrorException, "The request contains an exception.");

                hserver.VerifyAllRequestExpectationsAndUnexpectedRequests();
            }
