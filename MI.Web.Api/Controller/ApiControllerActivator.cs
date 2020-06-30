using MI.Core.Dependency;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace MI.Web.Api.Controller
{
    /// <summary>
    ///     使用<see cref="IIocResolver"/>来创建<see cref="ApiController"/>的激活器
    /// </summary>
    class ApiControllerActivator : IHttpControllerActivator
    {
        private readonly IIocResolver _iocResolver;

        public ApiControllerActivator(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controllerWrapper = _iocResolver.ResolveAsDisposable<IHttpController>(controllerType);
            request.RegisterForDispose(controllerWrapper);
            return controllerWrapper.Object;
        }
    }
}
