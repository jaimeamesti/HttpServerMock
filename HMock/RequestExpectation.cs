namespace HttpServerMock
{
    using System;

    /// <summary>
    /// Http server request expectation.
    /// </summary>
    public sealed class RequestExpectation : RequestMock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestExpectation"/> class.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <exception cref="System.ArgumentNullException">requestUri</exception>
        /// <exception cref="System.ArgumentException">The give request URI has not a valid format. It only supports absolute URIs.;requestUri</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Strings are more usable.")]
        public RequestExpectation(string requestUri)
            : base(requestUri)
        {
        }

        /// <summary>
        /// Gets or sets the number of times that this request will be repeated.
        /// </summary>
        public uint Repeats { get; set; }
    }
}