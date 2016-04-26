namespace HttpServerMock
{
    /// <summary>
    /// Available request content types.
    /// </summary>
    public enum HttpRequestContentType
    {
        /// <summary>
        /// Undefined content type.
        /// </summary>
        None = 0,

        /// <summary>
        /// Json content type.
        /// </summary>
        Json = 1,

        /// <summary>
        /// XML content type.
        /// </summary>
        Xml = 2,

        /////// <summary>
        /////// The text
        /////// </summary>
        ////Text = 3,

        /////// <summary>
        /////// The form URL encoded
        /////// </summary>
        ////FormUrlEncoded = 4
    }
}