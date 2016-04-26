namespace HttpServerMock
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    internal static class Helper
    {
        internal static readonly IEnumerable<string> JsonContentTypes = new Collection<string>
                                                       {
                                                           "application/json", 
                                                           "application/x-javascript", 
                                                           "application/javascript", 
                                                           "text/javascript", 
                                                           "text/x-javascript", 
                                                           "text/x-json", 
                                                           "text/json"
                                                       };

        internal static readonly IEnumerable<string> XmlContentTypes = new Collection<string> { "application/xml", "text/xml" };
        internal static readonly IEnumerable<string> TextContentType = new Collection<string> { "text/plain" };
        internal static readonly IEnumerable<string> FormUrlEncodedContentType = new Collection<string> { "application/x-www-form-urlencoded" };

        internal static string ParseRequestContentType(HttpRequestContentType contentType)
        {
            switch (contentType)
            {
                case HttpRequestContentType.None:
                    return string.Empty;
                case HttpRequestContentType.Json:
                    return string.Join(",", JsonContentTypes);
                case HttpRequestContentType.Xml:
                    return string.Join(",", XmlContentTypes);
                default:
                    throw new ArgumentOutOfRangeException("contentType");
            }
        }

        internal static string ParseResponseContentType(HttpRequestContentType contentType)
        {
            switch (contentType)
            {
                case HttpRequestContentType.None:
                    return string.Empty;
                case HttpRequestContentType.Json:
                    return "application/json";
                case HttpRequestContentType.Xml:
                    return "application/xml";
                default:
                    throw new ArgumentOutOfRangeException("contentType");
            }
        }

        internal static bool IsJsonRequest(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return JsonContentTypes.Contains(contentType);
        }

        internal static bool IsXmlRequest(string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType))
            {
                return false;
            }

            return XmlContentTypes.Contains(contentType);
        }
    }
}