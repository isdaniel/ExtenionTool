using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ExtensionTool.Mvc;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Web.Mvc.Html;
using System.Web.WebSockets;
using Rhino.Mocks;

namespace ExtensionTool.Tests.Mvc
{
    public class ViewModel
    {
        public string Value { get; set; }
    }

    [TestFixture]
    public class HtmlHelperExtensionTest
    {
        [TestCase(@"<input id=""Value"" name=""Value"" readonly=""readonly"" type=""text"" value=""test"" />",true)]
        [TestCase(@"<input id=""Value"" name=""Value"" type=""text"" value=""test"" />", false)]
        public void TextBoxReadOnly_Test(string expectHtmlString,bool isReadOnly)
        {
            HtmlHelperMock mock = new HtmlHelperMock();

            ViewModel vm = new ViewModel()
            {
                Value = "test"
            };

            var htmlHelper = mock.GetHtmlHelperForModel(vm);
            
            var actHtmlString = htmlHelper
                .TextBoxFor(x => x.Value, isReadOnly, null).ToHtmlString();

            Assert.AreEqual(expectHtmlString, actHtmlString);
        }

        [TestCase(@"<input id=""Value"" name=""Value"" readonly=""readonly"" type=""text"" value=""test"" zz=""test"" />", true)]
        [TestCase(@"<input id=""Value"" name=""Value"" type=""text"" value=""test"" zz=""test"" />", false)]
        public void TextBoxReadOnlyWithAttribute(string expectHtmlString, bool isReadOnly) {
            HtmlHelperMock mock = new HtmlHelperMock();

            ViewModel vm = new ViewModel()
            {
                Value = "test"
            };

            var htmlHelper = mock.GetHtmlHelperForModel(vm);

            var actHtmlString = htmlHelper
                .TextBoxFor(x => x.Value, isReadOnly, new { zz="test"}).ToHtmlString();

            Assert.AreEqual(expectHtmlString, actHtmlString);
        }
    }

    public class HtmlHelperMock
    {
        public HtmlHelper GetHtmlHelper(
            string viewName, 
            string routeController = "",
            string routeAction = "")
        {
            var routeData = new RouteData();
            routeData.Values["controller"] = routeController;
            routeData.Values["action"] = routeAction;

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var viewContext = MockRepository.GenerateStub<ViewContext>();
            var httpRequest = MockRepository.GenerateStub<HttpRequestBase>();
            var httpResponse = MockRepository.GenerateStub<HttpResponseBase>();

            httpContext.Stub(c => c.Request).Return(httpRequest).Repeat.Any();
            httpContext.Stub(c => c.Response).Return(httpResponse).Repeat.Any();
            httpResponse.Stub(r => r.ApplyAppPathModifier(Arg<string>.Is.Anything))
                        .Return(string.Format("/{0}/{1}", routeController, routeAction));

            viewContext.HttpContext = httpContext;
            viewContext.RequestContext = new RequestContext(httpContext, routeData);
            viewContext.RouteData = routeData;
            viewContext.ViewData = MockRepository.GenerateStub<ViewDataDictionary>();
            viewContext.ViewData.Model = null;
            viewContext.TempData = MockRepository.GenerateStub<TempDataDictionary>();
            viewContext.View = SetupView(viewName);

            return new HtmlHelper(viewContext, new ViewPage());
        }

        private IView SetupView(string viewName)
        {
            var view = MockRepository.GenerateStub<IView>();
            var engine = MockRepository.GenerateStub<IViewEngine>();
            var viewEngineResult = new ViewEngineResult(view, engine);
            engine.Stub(x => x.FindPartialView(null, viewName, false)).IgnoreArguments().Return(viewEngineResult);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(engine);
            return view;
        }

        public HtmlHelper<TModel> GetHtmlHelperForModel<TModel>(TModel inputDictionary, string routeController = "",
            string routeAction = "", string viewName = "")
        {
            var viewData = new ViewDataDictionary<TModel>(inputDictionary);
            var routeData = new RouteData();
            routeData.Values["controller"] = routeController;
            routeData.Values["action"] = routeAction;

            var httpContext = MockRepository.GenerateStub<HttpContextBase>();
            var viewContext = MockRepository.GenerateStub<ViewContext>();
            var httpRequest = MockRepository.GenerateStub<HttpRequestBase>();
            var httpResponse = MockRepository.GenerateStub<HttpResponseBase>();

            httpContext.Stub(c => c.Request).Return(httpRequest).Repeat.Any();
            httpContext.Stub(c => c.Response).Return(httpResponse).Repeat.Any();
            httpResponse.Stub(r => r.ApplyAppPathModifier(Arg<string>.Is.Anything))
                        .Return(string.Format("/{0}/{1}", routeController, routeAction));

            viewContext.HttpContext = httpContext;
            viewContext.RequestContext = new RequestContext(httpContext, routeData);
            viewContext.RouteData = routeData;
            viewContext.ViewData = MockRepository.GenerateStub<ViewDataDictionary>();
            viewContext.ViewData.Model = inputDictionary;
            viewContext.TempData = MockRepository.GenerateStub<TempDataDictionary>();
            viewContext.View = SetupView(viewName);

            return new HtmlHelper<TModel>(viewContext, GetViewDataContainer(viewData));
        }

        public IViewDataContainer GetViewDataContainer(ViewDataDictionary viewData)
        {
            var mockContainer = new Moq.Mock<IViewDataContainer>();
            mockContainer
                .Setup(x => x.ViewData)
                .Returns(viewData);
            return mockContainer.Object;
        }
    }
}
