using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HttpServerMockTests
{
    using HttpServerMock;
    using System;

    [TestClass]
    public class RequestExpectationTest
    {
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Constructor_ValidRelativeUrl_Ok()
        {
            const string RelativeUrl = "/users/23/data";
            var requestMock = new RequestExpectation(RelativeUrl);
            Assert.IsNull(requestMock.RequestRegex, "The request Regex should be null.");
            Assert.IsFalse(requestMock.IsRequestUriARegex, "The request URI should not be a Regex.");
            Assert.IsNotNull(requestMock.RequestUri, "The request uri should not be null.");
            Assert.AreEqual(RelativeUrl, requestMock.RequestUrl, "The request Url is not the expected.");
            Assert.AreEqual(RelativeUrl, requestMock.RequestUri.AbsolutePath, "The request Url is not the expected.");
        }

        [TestMethod]
        public void Constructor_ValidAbsoluteUrl_Ok()
        {
            const string AbsoluteUrl = "http://192.168.1.1/users/23/data";
            var requestMock = new RequestExpectation(AbsoluteUrl);
            Assert.IsNull(requestMock.RequestRegex, "The request Regex should be null.");
            Assert.IsFalse(requestMock.IsRequestUriARegex, "The request URI should not be a Regex.");
            Assert.IsNotNull(requestMock.RequestUri, "The request uri should not be null.");
            Assert.AreEqual(AbsoluteUrl, requestMock.RequestUrl, "The request Url is not the expected.");
            Assert.AreEqual(AbsoluteUrl, requestMock.RequestUri.AbsoluteUri, "The request Url is not the expected.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_InvalidUrl_ArgumentException()
        {
            const string InvalidUrl = "users[\\]23/data";
            new RequestExpectation(InvalidUrl);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_NullUrl_ArgumentNullException()
        {
            new RequestExpectation(null);
        }

        [TestMethod]
        public void Constructor_ValidRegex_Ok()
        {
            const string RegexUrl = "^/users/[0-9]{2}/data$";
            var requestMock = new RequestExpectation(RegexUrl);
            Assert.IsNotNull(requestMock.RequestRegex, "The request Regex should not be null.");
            Assert.IsTrue(requestMock.IsRequestUriARegex, "The request URI should be a Regex.");
            Assert.IsNull(requestMock.RequestUri, "The request uri should be null.");
            Assert.AreEqual(RegexUrl, requestMock.RequestUrl, "The request Url is not the expected.");
            Assert.AreEqual(RegexUrl, requestMock.RequestRegex.ToString(), "The request regex is not the expected.");
        }
    }
}
