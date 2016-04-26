namespace HttpServerMock.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception produced when the HTTP server mock request verification fails.
    /// </summary>
    public sealed class HttpServerCallsVerificationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerCallsVerificationException"/> class.
        /// </summary>
        public HttpServerCallsVerificationException()
            : base("The number of calls to the HTTP server were not the expected.")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerCallsVerificationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HttpServerCallsVerificationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpServerCallsVerificationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public HttpServerCallsVerificationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        private HttpServerCallsVerificationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}