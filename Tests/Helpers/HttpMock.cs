﻿using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Template.Tests.Helpers;

namespace Tests.Helpers
{
    public class HttpMock
    {
        public Mock<HttpRequestWrapper> HttpRequestMock
        {
            get;
            private set;
        }

        public HttpContextBase HttpContextBase
        {
            get;
            private set;
        }
        public Mock<IIdentity> IdentityMock
        {
            get;
            private set;
        }

        public HttpContext HttpContext
        {
            get;
            private set;
        }
        static HttpMock()
        {
            RouteTable.Routes
                .MapRoute(
                    "Default",
                    "{language}/{controller}/{action}/{id}",
                    new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "lt-LT" });

            RouteTable.Routes
                .MapRoute(
                    "DefaultLang",
                    "{controller}/{action}/{id}",
                    new { language = "en-GB", controller = "Home", action = "Index", id = UrlParameter.Optional },
                    new { language = "en-GB" });
        }

        public HttpMock()
        {
            var request = new HttpRequest(String.Empty, "http://localhost:19174/", String.Empty);
            var browserMock = new Mock<HttpBrowserCapabilities>() { CallBase = true };
            browserMock.SetupGet(mock => mock[It.IsAny<String>()]).Returns("true");
            request.Browser = browserMock.Object;

            var response = new HttpResponse(new StringWriter());
            HttpRequestMock = new Mock<HttpRequestWrapper>(request) { CallBase = true };
            HttpContext = new HttpContext(request, response);

            var httpContextBaseMock = new Mock<HttpContextWrapper>(HttpContext) { CallBase = true };
            httpContextBaseMock.Setup(mock => mock.Response).Returns(new HttpResponseWrapper(response));
            httpContextBaseMock.Setup(mock => mock.Request).Returns(HttpRequestMock.Object);
            httpContextBaseMock.Setup(mock => mock.Session).Returns(new HttpSessionStub());
            HttpContextBase = httpContextBaseMock.Object;

            IdentityMock = new Mock<IIdentity>();
            IdentityMock.Setup<String>(mock => mock.Name).Returns(ObjectFactory.TestId);

            var principalMock = new Mock<IPrincipal>();
            principalMock.Setup<IIdentity>(mock => mock.Identity).Returns(IdentityMock.Object);

            httpContextBaseMock.Setup(mock => mock.User).Returns(principalMock.Object);
            HttpContext.User = principalMock.Object;

            request.RequestContext.RouteData.Values["controller"] = "Controller";
            request.RequestContext.RouteData.Values["language"] = "en-GB";
            request.RequestContext.RouteData.Values["action"] = "Action";
            request.RequestContext.RouteData.Values["area"] = "Area";
        }
    }

    public class HttpSessionStub : HttpSessionStateBase
    {
        private readonly Dictionary<String, Object> _objects = new Dictionary<String, Object>();

        public override Object this[String name]
        {
            get { return (_objects.ContainsKey(name)) ? _objects[name] : null; }
            set { _objects[name] = value; }
        }

        public override void Add(String name, Object value)
        {
            _objects.Add(name, value);
            base.Add(name, value);
        }

        public override void Remove(String name)
        {
            _objects.Remove(name);
            base.Remove(name);
        }

        public override void Clear()
        {
            _objects.Clear();
            base.Clear();
        }

        public override void RemoveAll()
        {
            Clear();
        }
    }
}