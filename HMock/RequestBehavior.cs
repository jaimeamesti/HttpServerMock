namespace HttpServerMock
{
    using System;

    /// <summary>
    /// The request behavior.
    /// </summary>
    internal sealed class RequestBehavior : RequestMock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBehavior"/> class.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2234:PassSystemUriObjectsInsteadOfStrings", Justification = "Strings are more usable.")]
        public RequestBehavior(string requestUri)
            : base(requestUri)
        {
        }
    }
}