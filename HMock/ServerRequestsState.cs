namespace HttpServerMock
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Net;

    /// <summary>
    /// Contains the Http mock server requests state.
    /// </summary>
    public sealed class ServerRequestsState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServerRequestsState"/> class.
        /// </summary>
        public ServerRequestsState()
        {
            this.RequestExpectations = new Collection<RequestExpectation>();
            ////this.RequestBehaviors = new Collection<RequestBehavior>();
            this.UnexpectedRequests = new Collection<UnexpectedRequest>();
            this.DefaultRespondStatusCode = HttpStatusCode.NotImplemented;
        }

        /// <summary>
        /// Gets the request expectations configured for the Http server mock.
        /// </summary>
        public ICollection<RequestExpectation> RequestExpectations { get; private set; }

        /////// <summary>
        /////// Gets the request behaviors configured for the Http server mock.
        /////// </summary>
        ////public ICollection<RequestBehavior> RequestBehaviors { get; private set; }

        /// <summary>
        /// Gets the unexpected requests which were managed by the Http server mock.
        /// </summary>
        public ICollection<UnexpectedRequest> UnexpectedRequests { get; private set; }

        /// <summary>
        /// Gets or sets the respond status code which will be returned by the server for those
        /// request which has not any expectation or behavior.
        /// </summary>
        public HttpStatusCode DefaultRespondStatusCode { get; set; }
    }
}