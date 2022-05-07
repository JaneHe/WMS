using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Tops.FRM.Mvc
{
    public class Filters
    {
        public class LoginFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                //url = string.IsNullOrEmpty(url) ? Server.HtmlEncode("/Shared/Error/页面不存在，请联系技术人员！") : url;
                var route = filterContext.Controller.ControllerContext.RouteData;
                var CustController = route.Values["CustController"] == null ? "" : route.Values["CustController"].ToString();
                var Page = route.Values["Page"] == null ? "" : route.Values["Page"].ToString();
                route.Values["controller"] = CustController;
                route.Values["action"] = Page;
                //filterContext.HttpContext.Session["loginType"].ToString();

                //用于session跨域共享解决方式
                #region 用于session跨域共享解决方式
                SqlParameter[] ps = {  
                                new SqlParameter("@IPAddress",SqlDbType.VarChar),
                                new SqlParameter("@UserAgent",SqlDbType.VarChar)
                                };
                ps[0].Value = Tops.FRM.TopsUser.IpAddress;
                ps[1].Value = Tops.FRM.TopsUser.UserAgent;
                BizResult br2 = Biz.Execute("SelSessionState", ps, BizType.Query);
                List<DataTable> list = br2.Data as List<DataTable>;
                if (list[0].Rows.Count > 0)
                {
                    string LoginState = list[0].Rows[0]["LoginState"].ToString();
                    string UserNO = list[0].Rows[0]["UserNO"].ToString();
                    string UserName = list[0].Rows[0]["UserName"].ToString();

                    if (LoginState.Equals("0") && filterContext.HttpContext.Session["UserNo"] == null)
                    {
                        filterContext.HttpContext.Session["UserNo"] = UserNO;
                        filterContext.HttpContext.Session["UserName"] = UserName;
                        filterContext.HttpContext.Session["Powers"] = new TopsUser().GetPowers();
                        filterContext.HttpContext.Session["Roles"] = new TopsUser().GetRoles();

                        //string transferUrl = "/Home/Index";
                        string transferUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
                        filterContext.HttpContext.Response.Redirect(transferUrl);
                    }
                     
                }
                #endregion

                if (filterContext.HttpContext.Session["UserNO"] == null)
                {

                    if (TopsUser.UserAgent.IndexOf("Android") != -1 || TopsUser.UserAgent.IndexOf("iPhone") != -1)
                    {
                        string Surl = "/SiteLogin/Index?backUrl=";
                        //string Surl = "/Account/Login?backUrl=";
                        filterContext.HttpContext.Response.Redirect(Surl + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri));
                    }
                    else
                    {
                        if ((!route.Values["controller"].ToString().Equals("Home")) && (filterContext.RequestContext.HttpContext.Request.Headers["X-Requested-With"] != null))
                        {
                            BizResult br = new BizResult();
                            br.IsSuccess = false;
                            br.Msgs.Add("[F001]-用户登录超时，请重新登录");
                            ContentResult result = new ContentResult();
                            result.Content = JsonConvert.SerializeObject(br, Newtonsoft.Json.Formatting.Indented);
                            filterContext.Result = result;
                        }
                        else
                        {
                            string Surl = "/Account/Login?backUrl=";
                            if (filterContext.HttpContext.Request.Browser.MajorVersion <= 6)
                            {
                                Surl = "/Account/mLogin?backUrl=";
                            }
                            filterContext.HttpContext.Response.Redirect(Surl + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri));
                        }
                    }


                }
            }

        }
        public enum PowerType { ComModule = 0, CusModule = 1, Query = 2, Biz = 3 }
        public class PowerFilter : ActionFilterAttribute
        {
            public PowerType PowerType { get; set; }
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                //url = string.IsNullOrEmpty(url) ? Server.HtmlEncode("/Shared/Error/页面不存在，请联系技术人员！") : url;
                var route = filterContext.Controller.ControllerContext.RouteData;
                if (PowerType == PowerType.ComModule || PowerType == PowerType.CusModule)
                {
                    var PowerName = route.Values["CustController"] == null ? "" : route.Values["CustController"].ToString();
                    if (PowerType == PowerType.ComModule)
                        PowerName = route.Values["ModuleID"] + "";
                    //1判断访问的模块（通过权限验证）是否为公开模块对应的权限，含有：通过，不含有：判断是否登录
                    if (!TopsUser.HasPower(PowerName))
                    {
                        //用于session跨域共享解决方式
                        #region 用于session跨域共享解决方式
                        SqlParameter[] ps = {  
                                new SqlParameter("@IPAddress",SqlDbType.VarChar),
                                new SqlParameter("@UserAgent",SqlDbType.VarChar)
                                };
                        ps[0].Value = Tops.FRM.TopsUser.IpAddress;
                        ps[1].Value = Tops.FRM.TopsUser.UserAgent;
                        BizResult br = Biz.Execute("SelSessionState", ps, BizType.Query);
                        List<DataTable> list = br.Data as List<DataTable>;
                        if (list[0].Rows.Count > 0)
                        {
                            string LoginState = list[0].Rows[0]["LoginState"].ToString();
                            string UserNO = list[0].Rows[0]["UserNO"].ToString();
                            string UserName = list[0].Rows[0]["UserName"].ToString();

                            if (LoginState.Equals("0") && filterContext.HttpContext.Session["UserNo"] == null)
                            {
                                filterContext.HttpContext.Session["UserNo"] = UserNO;
                                filterContext.HttpContext.Session["UserName"] = UserName;
                                filterContext.HttpContext.Session["Powers"] = new TopsUser().GetPowers();
                                filterContext.HttpContext.Session["Roles"] = new TopsUser().GetRoles();

                                //string transferUrl = "/Home/Index";
                                string transferUrl = filterContext.HttpContext.Request.Url.AbsoluteUri;
                                filterContext.HttpContext.Response.Redirect(transferUrl);
                            }  
                        }
                        #endregion

                        if (filterContext.HttpContext.Session["UserNo"] == null)
                        {
                            if (TopsUser.UserAgent.IndexOf("Android") != -1 || TopsUser.UserAgent.IndexOf("iPhone") != -1)
                            {
                                filterContext.HttpContext.Response.Redirect("/SiteLogin/Index");
                                //filterContext.HttpContext.Response.Redirect("/Account/Login");
                            }
                            else
                            {
                                string Surl = "/Account/Login?backUrl=";
                                if (filterContext.HttpContext.Request.Browser.MajorVersion <= 6)
                                {
                                    Surl = "/Account/mLogin?backUrl=";
                                }

                                ////IP地址跳转
                                //string ip = GetLocalIPAddress();
                                //if (ip.Equals("192.168.3.14"))
                                //{
                                //    //Surl = "http://58.47.128.48:8889/Account/Login?backUrl=";
                                //}

                                if (filterContext.HttpContext.Session["UserNO"] == null)
                                    filterContext.HttpContext.Response.Redirect(Surl + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri));
                            }
                        }
                    }
                }
                else
                {
                    var PowerName = PowerType == PowerType.Biz ? (route.Values["BizName"] + "") : (route.Values["QueryName"] + "");
                    if (!TopsUser.HasPower(PowerName))
                    {
                        BizResult br = new BizResult();
                        br.IsSuccess = false;
                        if (filterContext.HttpContext.Session["UserNO"] == null)
                        {
                            br.Msgs.Add("[F001]-用户登录超时，请重新登录");
                        }
                        else
                        {
                            br.Msgs.Add("[Error-P001]你没有权限执行该操作,请联系管理人员。");
                        }
                        ContentResult result = new ContentResult();
                        result.Content = JsonConvert.SerializeObject(br, Newtonsoft.Json.Formatting.Indented);
                        filterContext.Result = result;
                    }
                }
            }

        }
        public class ToolAdminFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                var hasToolsPower = System.Configuration.ConfigurationManager.AppSettings["ToolsPower"];
                if (!"true".Equals(hasToolsPower))
                    filterContext.HttpContext.Response.Redirect("/Shared/error?eMsg=" + filterContext.HttpContext.Server.UrlEncode("[005]-非法操作！"));
            }

        }
        public class ExceptionFilter : ActionFilterAttribute, IExceptionFilter
        {
            public void OnException(ExceptionContext filterContext)
            {
                var route = filterContext.Controller.ControllerContext.RouteData;
                if (!filterContext.ExceptionHandled)
                {
                    if (filterContext.RequestContext.HttpContext.Request.Headers["X-Requested-With"] != null)
                    {
                        BizResult br = new BizResult();
                        br.IsSuccess = false;
                        br.Msgs.Add("[Error]003-：" + filterContext.Exception.Message);
                        ContentResult result = new ContentResult();
                        result.Content = JsonConvert.SerializeObject(br, Newtonsoft.Json.Formatting.Indented);
                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;
                    }
                    else
                        filterContext.HttpContext.Response.Redirect("/Shared/error?eMsg=" + filterContext.HttpContext.Server.UrlEncode("[004]-异常："
                            + filterContext.Exception.Message));



                    //错误写入日志
                    Log.WriteLog(new DateTime().ToString("yyyy-MM") + ".log", "错误日志（" + new DateTime().ToString("yyyy-MM-dd hh:mm:ss") + "）：" + filterContext.Exception + "/r/n堆栈：" + filterContext.Exception.StackTrace);
                }
            }
        }

        /// <summary>
        /// tools工具登录验证
        /// </summary>
        public class ToolsFilter : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                //url = string.IsNullOrEmpty(url) ? Server.HtmlEncode("/Shared/Error/页面不存在，请联系技术人员！") : url;
                var route = filterContext.Controller.ControllerContext.RouteData;
                var CustController = route.Values["CustController"] == null ? "" : route.Values["CustController"].ToString();
                var Page = route.Values["Page"] == null ? "" : route.Values["Page"].ToString();
                route.Values["controller"] = CustController;
                route.Values["action"] = Page;
                //filterContext.HttpContext.Session["loginType"].ToString();


                if (filterContext.HttpContext.Session["ToolsUserNO"] == null)
                {

                    if ((!route.Values["controller"].ToString().Equals("tools")) && (filterContext.RequestContext.HttpContext.Request.Headers["X-Requested-With"] != null))
                    {
                        BizResult br = new BizResult();
                        br.IsSuccess = false;
                        br.Msgs.Add("[F001]-用户登录超时，请重新登录");
                        ContentResult result = new ContentResult();
                        result.Content = JsonConvert.SerializeObject(br, Newtonsoft.Json.Formatting.Indented);
                        filterContext.Result = result;
                    }
                    else
                    {
                        string Surl = "/Account/ToolsLogin?backUrl="; 
                        filterContext.HttpContext.Response.Redirect(Surl + filterContext.HttpContext.Server.UrlEncode(filterContext.HttpContext.Request.Url.AbsoluteUri));
                    }

                }
            }

        }

        /// <summary>
        /// 获取本机IP地址
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIPAddress()
        {
            string resultIP = string.Empty;
            System.Net.IPAddress[] ips = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).AddressList;
            foreach (System.Net.IPAddress ip in ips)
            {
                if (IsCorrentIP(ip.ToString()))
                {
                    resultIP = ip.ToString();
                    break;
                }
            }
            return resultIP;
        }

        /// <summary>
        /// 验证IP地址是否有效
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IsCorrentIP(string ip)
        {
            string pattrn = @"(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])";
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, pattrn))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
