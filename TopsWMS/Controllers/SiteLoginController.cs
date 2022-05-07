using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tops.FRM;
using System.Data.SqlClient;

namespace TopsHNAH.Controllers
{
    public class SiteLoginController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var numCode = 2;
                var userNo = Session["UserNo"];
                TopsModel tm = new TopsModel(ControllerContext);
                SqlParameter[] ps = {
                                    new SqlParameter("@NumCode",numCode),
                                    new SqlParameter("@UserNo",userNo)
                                };
                tm.RunBiz("UpdateRespondTime", ps);
            }
            catch (Exception e)
            {
                TopsUser.Logout();
                return View();
                //throw new Exception("登陆超时，请重新登陆" + e.Message);
            }
            finally
            {
            }
            return View();
        }

    }
}
