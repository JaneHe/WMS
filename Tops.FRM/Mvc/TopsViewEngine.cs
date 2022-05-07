using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.IO;

namespace Tops.FRM.Mvc
{
    public class TopsViewEngine : RazorViewEngine
    {
        #region Ctor
        public TopsViewEngine()
            : this(null)
        {
        }

        public TopsViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
            AreaPartialViewLocationFormats =
                    new string[]
                    {
                        "~/TopsLib/Views/{1}/Shared/{0}.cshtml",
                        "~/TopsLib/Views/{1}/{0}.cshtml",
                        "~/TopsLib/Views/Shared/{0}.cshtml",

                        "~/Views/{1}/Shared/{0}.cshtml",
                        "~/Views/{1}/{0}.cshtml",
                        "~/Views/Shared/{0}.cshtml"
                    };

            AreaViewLocationFormats =
                    new string[]
                    {
                        "~/TopsLib/Views/{1}/Shared/{0}.cshtml",
                        "~/TopsLib/Views/Shared/{0}.cshtml",

                        "~/Views/{1}/Shared/{0}.cshtml",
                        "~/Views/Shared/{0}.cshtml"
                    };

            PartialViewLocationFormats =
                    new string[] 
                    {  
                        "~/TopsLib/Views/{1}/Shared/{0}.cshtml",
                        "~/TopsLib/Views/Shared/{0}.cshtml",

                        "~/Views/{1}/Shared/{0}.cshtml",
                        "~/Views/Shared/{0}.cshtml"
                    };

            ViewLocationFormats =
                    new string[] 
                    { 
                        "~/TopsLib/Views/{1}/Shared/{0}.cshtml",
                        "~/TopsLib/Views/{1}/{0}.cshtml",
                        "~/TopsLib/Views/Shared/{0}.cshtml",

                        "~/Views/{1}/Shared/{0}.cshtml",
                        "~/Views/{1}/{0}.cshtml",
                        "~/Views/Shared/{0}.cshtml"
                    };
        }
        #endregion

        #region Override Find PartialView and Find View
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }
        #endregion

    }
}
