using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Net.Mail;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Timers;
using System.Net;
using System.Globalization;

namespace Tops.FRM.Mvc
{
    public class MvcHttpApplication : System.Web.HttpApplication
    {

        #region 字段属性
        /// <summary>
        /// 发送内容
        /// </summary>
        public string SendContent { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string AccptMobile { get; set; }
        /// <summary>
        /// 邮件地址
        /// </summary>
        public string AccptEmail { get; set; }
        /// <summary>
        /// 标记(超过天数范围)
        /// </summary>
        public int TimeOutDay { get; set; }
        /// <summary>
        /// 指定维护时间
        /// </summary>
        public string MSTime { get; set; }
        /// <summary>
        /// 班次ID
        /// </summary>
        public int MScheID { get; set; }
        /// <summary>
        /// 班次号
        /// </summary>
        public string MScheNo { get; set; }
        /// <summary>
        /// 任务ID
        /// </summary>
        public int DPMaintID { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string DPMaintName { get; set; }
        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 备件流水号
        /// </summary>
        public string NCount { get; set; }
        /// <summary>
        /// 备件名称
        /// </summary>
        public string DevicePartCName { get; set; }
        /// <summary>
        /// 安排调度人员
        /// </summary>
        public string ArrangementUNO { get; set; }
        /// <summary>
        /// 超过天数
        /// </summary>
        public int Delayday { get; set; }
        /// <summary>
        /// 维护员
        /// </summary>
        public string MaintMan { get; set; }
        /// <summary>
        /// 班次生成时间
        /// </summary>
        public string SchRTime { get; set; }
        /// <summary>
        /// 请求的页面
        /// </summary>
        public string RequestedPage { get; set; }
        /// <summary>
        /// 显示状态
        /// </summary>
        public string Tag { get; set; }
        #endregion

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
            routes.IgnoreRoute("Scripts/{*pathInfo}");
            routes.IgnoreRoute("Content/{*pathInfo}");
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });
            //Tools
            routes.MapRoute("ToolsAjaxPost", "Tools/AjaxPost/{BizName}", new
            {
                controller = "Tools",
                action = "AjaxPost",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.ToolsController" });

            routes.MapRoute("ToolsAjaxGet", "Tools/AjaxGet/{QueryName}", new
            {
                controller = "Tools",
                action = "AjaxGet",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("GET") }
            , new string[] { "TopsERP.Controllers.ToolsController" });

            routes.MapRoute("ToolsAjaxGridData", "Tools/AjaxGridData/{QueryName}", new
            {
                controller = "Tools",
                action = "AjaxGridData",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("GET") }
            , new string[] { "TopsERP.Controllers.ToolsController" });
            //private
            routes.MapRoute("AjaxPost", "AjaxPost/{BizName}", new
            {
                controller = "Common",
                action = "AjaxPost",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.CommonController" });

            /*Jane 离线Ajax请求*/
            routes.MapRoute("AjaxOffLinePost", "AjaxOffLinePost/{BizName}", new
            {
                controller = "Common",
                action = "AjaxOffLinePost",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.CommonController" });
            /*Jane 离线Ajax请求*/

            /*Jane Excel下载请求*/
            routes.MapRoute("ExcelHelp", "ExcelHelp/DownLoad", new
            {
                controller = "ExcelHelp",
                action = "DownLoad",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.ExcelHelpController" });
            /*Jane Excel下载请求*/



            /*Jane Redis请求*/
            routes.MapRoute("RedisGet", "Redis/RedisGet", new
            {
                controller = "Redis",
                action = "RedisGet",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("GET") }
            , new string[] { "Tops.FRM.Controllers.RedisController" });

            routes.MapRoute("RedisSet", "Redis/RedisSet", new
            {
                controller = "Redis",
                action = "RedisSet",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.RedisController" });

            routes.MapRoute("RedisDel", "Redis/RedisDel", new
            {
                controller = "Redis",
                action = "RedisDel",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.RedisController" });
            /*Jane Redis下载请求*/


            routes.MapRoute("Upload", "Upload/{PathAppSetting}", new
            {
                controller = "Common",
                action = "Upload",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.CommonController" });

            routes.MapRoute("UploadFile", "UploadFile/{PathAppSetting}", new
            {
                controller = "Common",
                action = "UploadFile",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("POST") }
            , new string[] { "Tops.FRM.Controllers.CommonController" });

            routes.MapRoute("AjaxGet", "AjaxGet/{QueryName}", new
            {
                controller = "Common",
                action = "AjaxGet",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("GET") }
            , new string[] { "TopsERP.Controllers.CommonController" });

            routes.MapRoute("AjaxGridData", "AjaxGridData/{QueryName}", new
            {
                controller = "Common",
                action = "AjaxGridData",
                BizName = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("GET") }
            , new string[] { "TopsERP.Controllers.CommonController" });
            //common
            routes.MapRoute("Query", "q/{ModuleID}", new
            {
                controller = "Common",
                action = "q",
                ModuleID = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("GET") }
            , new string[] { "TopsERP.Controllers.CommonController" });

            routes.MapRoute("DataObject", "d/{ModuleID}", new
            {
                controller = "Common",
                action = "d",
                ModuleID = UrlParameter.Optional
            },
            new { httpMethod = new HttpMethodConstraint("GET") }
            , new string[] { "TopsERP.Controllers.CommonController" });

            //default-private
            routes.MapRoute(
                "Default", // 路由名称
                "{controller}/{action}/{id}", // 带有参数的 URL
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // 参数默认值
                , new string[] { "TopsERP.Controllers.CommonController" }
            ).RouteHandler = new TopsMvcRouteHandler();

        }
        public static void RegisterViewEngine()
        {
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new TopsViewEngine());
        }



        protected void Application_Start()
        {

            //JaneHe 框架定时线程
            //System.Threading.Thread emailhelp = new System.Threading.Thread(new System.Threading.ThreadStart(EmailHelp.SendEmail));
            //emailhelp.Start();
             
            //初始化读取定时数据
            t1.Elapsed += new System.Timers.ElapsedEventHandler(LoadAutoOperationData);
            t1.AutoReset = true;//设置是执行一次（false）还是一直执行(true);
            t1.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 
 
            //自动执行定时存储过程
            AutoOperationEvent();

            AreaRegistration.RegisterAllAreas();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterViewEngine();
            RegisterRoutes(RouteTable.Routes);

        }


        //需实时执行的结果集集合
        List<tAutoAcqEventInfo> list = new List<tAutoAcqEventInfo>();

        //定时器 一小时间隔执行一次 3600000
        System.Timers.Timer t = new System.Timers.Timer(59000);
        System.Timers.Timer t1 = new System.Timers.Timer(3600000);

        /// <summary>
        /// 自动定时事件
        /// </summary>
        public void AutoOperationEvent()
        {
            LoadAutoOperationData(null,null);

            //初始化
            t.Elapsed += new System.Timers.ElapsedEventHandler(ImplementstoredProcedure);
            t.AutoReset = true;//设置是执行一次（false）还是一直执行(true);
            t.Enabled = true;//是否执行System.Timers.Timer.Elapsed事件； 

        }

        /// <summary>
        /// 加载定时数据
        /// </summary>
        public void LoadAutoOperationData(object obj, System.Timers.ElapsedEventArgs target)
        {

            //读取初始化数据
            DataTable dt = new Tops.FRM.DbHelperSQL(0).Query(@"select s_AutoAcqEventInfo_Procedure,
    d_AutoAcqEventInfo_SendTime,
    n_AutoAcqEventInfo_TimeTick,
    s_AutoAcqEventInfo_Remark from tAutoAcqEventInfo with(nolock) where n_AutoAcqEventInfo_state = 1").Tables[0];

            //初始化数据
            list = new List<tAutoAcqEventInfo>();

            if (dt.Rows.Count > 0)
            {
                for (int index = 0; index < dt.Rows.Count; index++)
                {
                    list.Add(new tAutoAcqEventInfo()
                    {
                        s_AutoAcqEventInfo_Procedure = dt.Rows[index]["s_AutoAcqEventInfo_Procedure"].ToString(),
                        d_AutoAcqEventInfo_SendTime = dt.Rows[index]["d_AutoAcqEventInfo_SendTime"].ToString(),
                        n_AutoAcqEventInfo_TimeTick = dt.Rows[index]["n_AutoAcqEventInfo_TimeTick"].ToString(),
                        s_AutoAcqEventInfo_Remark = dt.Rows[index]["s_AutoAcqEventInfo_Remark"].ToString(),
                    });
                }
            }
        }



        /// <summary>
        /// 执行存储过程
        /// </summary>
        public void ImplementstoredProcedure(object obj, System.Timers.ElapsedEventArgs target)
        {

            for (int index = 0; index < list.Count; index++)
            {
                try
                {
                    if (list[index].d_AutoAcqEventInfo_SendTime.Equals(string.Empty))
                    {
                        continue;
                    }

                    //获取小时差
                    int HourDiff = DateTime.Now.Hour < DateTime.Parse(list[index].d_AutoAcqEventInfo_SendTime).Hour ?
                        DateTime.Parse(list[index].d_AutoAcqEventInfo_SendTime).Hour - DateTime.Now.Hour :
                        DateTime.Parse(list[index].d_AutoAcqEventInfo_SendTime).Hour + 24 - DateTime.Now.Hour;


                    //小时间隔
                    int TimingExecutionHourTime = int.Parse(list[index].n_AutoAcqEventInfo_TimeTick);

                    //获取分钟差
                    int MinDiff = DateTime.Now.Minute - DateTime.Parse(list[index].d_AutoAcqEventInfo_SendTime).Minute <= 0 ?
                        DateTime.Parse(list[index].d_AutoAcqEventInfo_SendTime).Minute - DateTime.Now.Minute :
                        DateTime.Parse(list[index].d_AutoAcqEventInfo_SendTime).Minute + 60 - DateTime.Now.Minute;

                    //当剩余数为0时,说明是准时进行执行
                    if (HourDiff % TimingExecutionHourTime == 0 && MinDiff == 0)
                    {
                        new EmailErrorLog().writeInLog(DateTime.Now.ToString("yyyy-MM-dd") + " 执行间隔事件_" + "定时执行名称为:" + list[index].s_AutoAcqEventInfo_Procedure + "_" + list[index].n_AutoAcqEventInfo_TimeTick + "_" + list[index].d_AutoAcqEventInfo_SendTime + "_" + TimingExecutionHourTime + "_" + list[index].s_AutoAcqEventInfo_Remark);
                        new Tops.FRM.DbHelperSQL(0).ExecuteSql(list[index].s_AutoAcqEventInfo_Procedure);

                    }
                    //new EmailErrorLog().writeInLog(DateTime.Now.ToString("yyyy-MM-dd") + " 执行间隔事件_" + "定时执行名称为:" + list[index].s_AutoAcqEventInfo_Procedure + "_" + list[index].n_TimingExecutionHour + "_" + HourDiff + "_" + TimingExecutionHourTime);
                }
                catch (Exception ex)
                {
                    new EmailErrorLog().writeInLog(DateTime.Now.ToString("yyyy-MM-dd") + " 执行间隔事件异常_" + "定时执行名称为:" + list[index].s_AutoAcqEventInfo_Procedure + ",异常详细是:" + ex.Message);
                }
            }
        }

         

    }


    /// <summary>
    /// 初始化数据实体类
    /// </summary>
    public class tAutoAcqEventInfo
    {
        public string s_AutoAcqEventInfo_Procedure { get; set; }
        public string d_AutoAcqEventInfo_SendTime { get; set; }
        public string n_AutoAcqEventInfo_TimeTick { get; set; }
        public string s_AutoAcqEventInfo_Remark { get; set; }
    }
}