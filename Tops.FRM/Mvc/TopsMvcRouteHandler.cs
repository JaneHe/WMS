using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;

namespace Tops.FRM.Mvc
{
    public class TopsMvcRouteHandler : IRouteHandler
    {
        public System.Web.IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            string url = System.Web.HttpContext.Current.Request.Url.ToString();
            if (url.IndexOf(".js") != -1 || url.IndexOf(".ico") != -1) return null;

            string controllerName = requestContext.RouteData.GetRequiredString("controller");
            //if (controllerName.IndexOf(".ico") != -1 || controllerName.IndexOf(".jpg") != -1 || controllerName.IndexOf(".gif") != -1) return null;
            requestContext.RouteData.Values.Add("Page", "");
            requestContext.RouteData.Values.Add("CustController", "");
            string CustController = controllerName;
            string Module = requestContext.RouteData.GetRequiredString("action");
            IControllerFactory factory = ControllerBuilder.Current.GetControllerFactory();
            bool haveControllerMethod = false;
            IController controller;
            try
            {
                controller = factory.CreateController(requestContext, controllerName);
                var methods = controller.GetType().GetMethods().Where(m => m.Name.ToLower() == requestContext.RouteData.GetRequiredString("action").ToLower());
                haveControllerMethod = methods.Any();
                if (!haveControllerMethod)
                {

                    if (!methods.Any() && requestContext.RouteData.GetRequiredString("action").ToLower() != "index" && requestContext.RouteData.Values["id"].ToString() == "")
                    {
                        requestContext.RouteData.Values["id"] = requestContext.RouteData.GetRequiredString("action");
                        requestContext.RouteData.Values["action"] = "index";
                    }
                    //Module = requestContext.RouteData.Values["action"].ToString();
                    CustController = controllerName;
                }
            }
            catch
            {
                haveControllerMethod = false;
            }
            if (!haveControllerMethod)
            {
                if (requestContext.RouteData.GetRequiredString("action") != "Index" && requestContext.RouteData.Values["id"].ToString() == "")
                {
                    Module = requestContext.RouteData.GetRequiredString("action");
                }

                requestContext.RouteData.Values["controller"] = controllerName == "tools" ? "tools" : "Common";
                requestContext.RouteData.Values["action"] = "Index";
            }
            requestContext.RouteData.Values["CustController"] = CustController;
            requestContext.RouteData.Values["Page"] = Module;



            controller = factory.CreateController(requestContext, requestContext.RouteData.GetRequiredString("controller"));
            if (controller == null)
            {
                throw new System.InvalidOperationException();
            }
            return new TopsMvcHttpHandler(controller, factory, requestContext) { CustController = CustController, Page = Module };
        }
    }
}
