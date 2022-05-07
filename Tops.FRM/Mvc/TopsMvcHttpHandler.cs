using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace Tops.FRM.Mvc
{
    public class TopsMvcHttpHandler : IHttpHandler, IRequiresSessionState
    {
        public string CustController { get; set; }
        public string Page { get; set; }
        public IController Controller
        {
            get;
            private set;
        }
        public System.Web.Routing.RequestContext RequestContext
        {
            get;
            private set;
        }
        public IControllerFactory ControllerFactory
        {
            get;
            private set;
        }
        public virtual bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public TopsMvcHttpHandler(IController controller, IControllerFactory controllerFactory, RequestContext requestContext)
        {
            this.Controller = controller;
            this.ControllerFactory = controllerFactory;
            this.RequestContext = requestContext;
        }
        public virtual void ProcessRequest(System.Web.HttpContext context)
        {
            try
            {
                this.Controller.Execute(this.RequestContext);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally 
            {
                this.ControllerFactory.ReleaseController(this.Controller);
            }
        }
    }
}
