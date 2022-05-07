using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tops.FRM;
using System.Data;
using System.Data.SqlClient;
 

namespace Tops.FRM.Controllers
{
    public class SharedController : Controller
    {
        public ActionResult Error()
        {
            var msg = Request.QueryString["eMsg"]+"";
            ViewBag.ErrorMsg = msg;
            return View();
        }
    }
}
