using System;
using Moq;
using log4net;
using FligthNode.Common.Api.Controllers;
using System.Net.Http;
using System.Web.Http;

namespace FlightNode.DataCollection.Domain.UnitTests.Services.Controller
{
    public abstract class LoggingControllerBaseFixture<TController, TManager> : IDisposable
        where TController : LoggingController
        where TManager : class
    {
        protected MockRepository MockRepository = new MockRepository(MockBehavior.Strict);
        protected Mock<TManager> MockDomainManager;
        protected Mock<ILog> MockLogger;
        protected const string url = "http://some/where/";

        public LoggingControllerBaseFixture()
        {
            MockDomainManager = MockRepository.Create<TManager>();
            MockLogger = MockRepository.Create<ILog>();
        }

        protected TController BuildSystem()
        {
            var controller = Activator.CreateInstance(typeof(TController), MockDomainManager.Object) as TController;

            controller.Logger = MockLogger.Object;

            controller.Request = new HttpRequestMessage();
            controller.Request.RequestUri = new Uri(url);

            controller.Configuration = new HttpConfiguration();

            return controller;
        }

        public void Dispose()
        {
            MockRepository.VerifyAll();
        }



        protected void ExpectToLogToError()
        {
            MockLogger.Setup(x => x.Error(It.IsAny<Exception>()));
        }

        protected void ExpectToLogToDebug()
        {
            MockLogger.Setup(x => x.Debug(It.IsAny<Exception>()));
        }
    }
}
